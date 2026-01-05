using Zenject;

namespace TelephoneBooth.Enemy.Services
{
  public class EnemyServicesInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<EnemyStateService>().AsSingle();
      Container.BindInterfacesAndSelfTo<EnemyPatrolService>().AsSingle();
    }
  }
}