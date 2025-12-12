using Zenject;

namespace TelephoneBooth.Player.Factory
{
  public class PlayerFactoryInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle();
    }
  }
}