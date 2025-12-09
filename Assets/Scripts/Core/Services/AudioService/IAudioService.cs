using System;

namespace TelephoneBooth.Core.Services
{
  public interface IAudioService
  {
    void PlaySound(string audioClipName, float volume = 1f, bool isLoop = false, float pitch = 1f, Action callback = null);
    void StopSound(string audioClipName, Action callback = null);
    void SmoothStopSound(string audioClipName, float smoothDuration = 1f, Action callback = null);
    void ReleaseAudioSources();
  }
}