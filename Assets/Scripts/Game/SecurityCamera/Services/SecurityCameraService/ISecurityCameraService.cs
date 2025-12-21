using Cysharp.Threading.Tasks;
using TelephoneBooth.Game.SecurityCamera.Data;

namespace TelephoneBooth.Game.SecurityCamera.Services
{
  public interface ISecurityCameraService
  {
    void Init(SecurityCameraData data);
    void EnableMonitor();
    UniTask DisableMonitor();
  }
}