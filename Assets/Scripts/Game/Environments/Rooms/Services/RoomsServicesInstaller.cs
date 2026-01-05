using Zenject;

namespace TelephoneBooth.Game.Environments.Rooms.Services
{
  public class RoomsServicesInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<RoomsService>().AsSingle();
    }
  }
}