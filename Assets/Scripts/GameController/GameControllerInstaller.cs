using Zenject;

namespace TelephoneBooth.Game.GameController
{
  public class GameControllerInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
    }
  }
}