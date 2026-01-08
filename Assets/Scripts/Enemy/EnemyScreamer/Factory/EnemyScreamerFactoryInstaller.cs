using Zenject;

namespace TelephoneBooth.Enemy.EnemyScreamer.Factory
{
  public class EnemyScreamerFactoryInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<EnemyScreamerFactory>().AsSingle();
    }
  }
}