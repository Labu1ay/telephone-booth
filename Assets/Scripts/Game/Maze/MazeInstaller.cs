using Zenject;

namespace TelephoneBooth.Game.Maze
{
  public class MazeInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.Bind<IMazeService>()
        .To<MazeService>()
        .AsSingle()
        .NonLazy();
    }
  }
}