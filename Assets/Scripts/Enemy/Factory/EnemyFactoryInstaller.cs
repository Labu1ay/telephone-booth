using Zenject;

namespace TelephoneBooth.Enemy.Factory
{
  public class EnemyFactoryInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<EnemyFactory>().AsSingle();
    }
  }
}