using Zenject;

namespace TelephoneBooth.Core.Services
{
  public class CoreServicesInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<AssetService>().AsSingle();
      Container.BindInterfacesAndSelfTo<SavingService>().AsSingle();
      Container.BindInterfacesAndSelfTo<AudioService>().AsSingle();
      Container.BindInterfacesAndSelfTo<SceneLoaderService>().AsSingle();
      Container.BindInterfacesAndSelfTo<GameStateService>().AsSingle();
    }
  }
}