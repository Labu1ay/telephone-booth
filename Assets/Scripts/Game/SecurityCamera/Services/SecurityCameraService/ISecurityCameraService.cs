using System;
using Cysharp.Threading.Tasks;
using TelephoneBooth.Game.SecurityCamera.Data;

namespace TelephoneBooth.Game.SecurityCamera.Services
{
  public interface ISecurityCameraService
  {
    event Action<bool> CameraApplied;
    CameraSecurity CurrentSecurityCamera { get; }
    void Init(SecurityCameraData data);
    void EnableMonitor();
    UniTask DisableMonitor();
  }
}