using SerializableDictionary.Scripts;
using UnityEngine;

namespace TelephoneBooth.Core.Configs
{
  [CreateAssetMenu(fileName = "AudioClipConfig", menuName = "configs/AudioClipConfig", order = 0)]
  public class AudioClipConfig : ScriptableObject {

    [SerializeField] private SerializableDictionary<string, AudioClip> _audioClips;

    public AudioClip GetAudioClip(string audioClipName) {
      return _audioClips.ContainsKey(audioClipName) ? _audioClips.Dictionary[audioClipName] : null;
    }
  }
}