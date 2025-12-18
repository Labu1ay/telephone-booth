using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TelephoneBooth.Core.Configs
{
  [CreateAssetMenu(fileName = "AudioClipConfig", menuName = "configs/AudioClipConfig", order = 0)]
  public class AudioClipConfig : SerializedScriptableObject
  {
    [OdinSerialize] Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public AudioClip GetAudioClip(string audioClipName) =>
      _audioClips.ContainsKey(audioClipName) ? _audioClips[audioClipName] : null;
  }
}