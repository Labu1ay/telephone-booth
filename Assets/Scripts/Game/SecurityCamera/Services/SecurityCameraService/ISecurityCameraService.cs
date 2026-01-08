using Cysharp.Threading.Tasks;
using TelephoneBooth.Game.SecurityCamera.Data;
using UnityEngine;

namespace TelephoneBooth.Game.SecurityCamera.Services
{
  public interface ISecurityCameraService
  {
    Camera CurrentCamera { get; }
    void Init(SecurityCameraData data);
    void EnableMonitor();
    UniTask DisableMonitor();
  }
}