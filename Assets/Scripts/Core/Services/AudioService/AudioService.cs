using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Core.Configs;
using TelephoneBooth.Utils;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace TelephoneBooth.Core.Services
{
  public class AudioService : IAudioService, IInitializable, ILateDisposable
  {
    private const string PATH_AUDIO_SOURCE = "AudioSource";

    private readonly IAssetService _assetService;
    private readonly AudioClipConfig _audioClipConfig;

    private Dictionary<AudioSource, string> _playingAudioSources = new Dictionary<AudioSource, string>();

    private Dictionary<AudioSource, CancellationTokenSource> _playingCancellationTokenSources =
      new Dictionary<AudioSource, CancellationTokenSource>();

    private ObjectPool<AudioSource> _audioSourcePool;

    private AudioSource _audioSourcePrefab;
    private Transform _content;

    private Tween _tween;

    public AudioService(IAssetService assetService, AudioClipConfig audioClipConfig)
    {
      _assetService = assetService;
      _audioClipConfig = audioClipConfig;
    }

    public void Initialize()
    {
      _content = new GameObject("AudioSources").transform;
      Object.DontDestroyOnLoad(_content);

      _audioSourcePrefab = _assetService.Load<AudioSource>(PATH_AUDIO_SOURCE);

      _audioSourcePool = new ObjectPool<AudioSource>(_audioSourcePrefab, _content, 3);
    }

    public async void PlaySound(string audioClipName, float volume = 1f, bool isLoop = false, float pitch = 1f,
      Action callback = null)
    {
      var audioSource = _audioSourcePool.Instantiate(_audioSourcePrefab, _content);

      AudioClip audioClip = _audioClipConfig.GetAudioClip(audioClipName);

      if (!audioClip) return;

      audioSource.clip = audioClip;
      audioSource.loop = isLoop;
      audioSource.volume = volume;
      audioSource.pitch = pitch;

      audioSource.Play();
      _playingAudioSources.Add(audioSource, audioClipName);

      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      _playingCancellationTokenSources.Add(audioSource, cancellationTokenSource);

      if (isLoop) return;

      await UniTask.Delay(TimeSpan.FromSeconds(audioClip.length), cancellationToken: cancellationTokenSource.Token);
      StopSound(audioSource, callback);
    }

    public void StopSound(string audioClipName, Action callback = null)
    {
      if (!_playingAudioSources.ContainsValue(audioClipName)) return;

      _playingAudioSources
        .Where(pair => pair.Value == audioClipName)
        .Select(pair => pair.Key)
        .ToList()
        .ForEach(i => StopSound(i));

      callback?.Invoke();
    }

    public void SmoothStopSound(string audioClipName, float smoothDuration = 1f, Action callback = null)
    {
      if (!_playingAudioSources.ContainsValue(audioClipName)) return;

      var audioSource = _playingAudioSources.FirstOrDefault(i => i.Value == audioClipName).Key;

      _tween = audioSource
        .DOFade(0f, smoothDuration)
        .SetEase(Ease.Linear)
        .OnComplete(() => StopSound(audioSource, callback));
    }

    public void ReleaseAudioSources() => _audioSourcePool.Release();

    private void StopSound(AudioSource audioSource, Action callback = null)
    {
      callback?.Invoke();

      _audioSourcePool.Destroy(audioSource);
      _playingAudioSources.Remove(audioSource);

      if (_playingCancellationTokenSources.TryGetValue(audioSource,
            out CancellationTokenSource cancellationTokenSource))
      {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
      }

      _playingCancellationTokenSources.Remove(audioSource);
    }

    public void LateDispose()
    {
      _tween?.Kill();
    }
  }
}