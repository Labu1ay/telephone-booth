using TelephoneBooth.Enemy.Components;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.Services
{
  public class EnemyHitServiceInstaller : MonoInstaller
  {
    [SerializeField] private Transform _eyesPoint;
    [SerializeField] private Transform _enemyTransform;
    [SerializeField] private EnemyAnimator _enemyAnimator;
    public override void InstallBindings()
    {
      Container.Bind<Transform>().WithId("EyesPoint").FromInstance(_eyesPoint);
      Container.Bind<Transform>().WithId("EnemyTransform").FromInstance(_enemyTransform);
      Container.Bind<EnemyAnimator>().FromInstance(_enemyAnimator);
      
      Container.BindInterfacesAndSelfTo<EnemyHitService>().AsSingle();
    }
  }
}