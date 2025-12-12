using Zenject;

namespace TelephoneBooth.Game
{
  public class PlayerCameraProviderInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<PlayerCameraProvider>().AsSingle();
    }
  }
}