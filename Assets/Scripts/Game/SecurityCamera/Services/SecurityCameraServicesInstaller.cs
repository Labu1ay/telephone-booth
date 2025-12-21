using Zenject;

namespace TelephoneBooth.Game.SecurityCamera.Services
{
  public class SecurityCameraServicesInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<SecurityCameraService>().AsSingle();
    }
  }
}