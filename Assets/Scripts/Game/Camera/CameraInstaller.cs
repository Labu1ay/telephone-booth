using Zenject;

namespace TelephoneBooth.Game
{
  public class CameraInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<PlayerCameraProvider>().AsSingle();
      Container.BindInterfacesAndSelfTo<CameraMovementService>().AsSingle();
      Container.BindInterfacesAndSelfTo<InteractiveCameraService>().AsSingle();
    }
  }
}