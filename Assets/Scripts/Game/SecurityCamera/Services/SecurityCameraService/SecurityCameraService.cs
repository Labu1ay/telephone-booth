using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Game.SecurityCamera.Data;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace TelephoneBooth.Game.SecurityCamera.Services
{
  public class SecurityCameraService : ISecurityCameraService, ILateDisposable
  {
    private const int TEXTURE_WIDTH = 512;
    private const int TEXTURE_HEIGHT = 288;
    private const int DEPTH = 16;
    private const float FADE_DURATION = 0.25f;
    
    private readonly IInputService _inputService;

    private RenderTexture _renderTexture;
    private int _index;

    private Camera[] _securityCameras;
    private MeshRenderer _monitorScreenRenderer;
    private CanvasGroup _fadeMonitorGroup;

    private Tween _tween;
    private IDisposable _disposable;

    private Camera _currentCamera =>
      _securityCameras != null && _securityCameras.Length > 0 ? _securityCameras[_index] : null;

    [Inject]
    private SecurityCameraService(IInputService inputService)
    {
      _inputService = inputService;
    }

    public void Init(SecurityCameraData data)
    {
      _securityCameras = data.SecurityCameras;
      _monitorScreenRenderer = data.MonitorScreenRenderer;
      _fadeMonitorGroup = data.FadeMonitorGroup;

      if (_securityCameras == null) return;

      foreach (var securityCamera in _securityCameras)
        if (securityCamera != null)
          securityCamera.enabled = false;

      _inputService.LeftHandler += LeftHandler;
      _inputService.RightHandler += RightHandler;
    }

    private void LeftHandler() => ApplyCamera(Wrap(_index - 1));
    private void RightHandler() => ApplyCamera(Wrap(_index + 1));


    public void EnableMonitor()
    {
      _renderTexture = new RenderTexture(TEXTURE_WIDTH, TEXTURE_HEIGHT, DEPTH);

      _renderTexture.Create();
      _monitorScreenRenderer.material.mainTexture = _renderTexture;

      ApplyCamera(_index);

      _fadeMonitorGroup.DOFade(0f, FADE_DURATION);
      
    }

    private int Wrap(int i)
    {
      int n = _securityCameras.Length;
      i %= n;
      if (i < 0) i += n;
      return i;
    }

    private void ApplyCamera(int newIndex)
    {
      var oldCamera = _currentCamera;
      if (oldCamera != null)
      {
        oldCamera.targetTexture = null;
        oldCamera.enabled = false;
      }

      _index = newIndex;

      var newCamera = _currentCamera;
      if (newCamera != null)
      {
        newCamera.enabled = true;
        newCamera.targetTexture = _renderTexture;
        newCamera.aspect = (float)TEXTURE_WIDTH / TEXTURE_HEIGHT;
      }
    }

    public async UniTask DisableMonitor()
    {
      _disposable?.Dispose();

      _tween = _fadeMonitorGroup.DOFade(1f, FADE_DURATION);
      await _tween.ToUniTask();

      DisposeMonitorAndCameras();
    }

    private void DisposeMonitorAndCameras()
    {
      _disposable?.Dispose();

      if (_securityCameras != null)
      {
        foreach (var securityCamera in _securityCameras)
        {
          if (securityCamera == null) continue;
          securityCamera.targetTexture = null;
          securityCamera.enabled = false;
        }
      }

      if (_renderTexture != null)
      {
        _renderTexture.Release();
        Object.Destroy(_renderTexture);
        _renderTexture = null;
      }
    }

    public void LateDispose()
    {
      DisposeMonitorAndCameras();
      
      _inputService.LeftHandler -= LeftHandler;
      _inputService.RightHandler -= RightHandler;
    }
  }
}