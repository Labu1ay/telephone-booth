using System;
using UniRx;

namespace TelephoneBooth.Enemy.Services
{
  public interface IEnemyStateService
  {
    event Action<EnemyStateType> EnemyStateStarted;
    event Action<EnemyStateType> EnemyStateFinished;
    ReadOnlyReactiveProperty<EnemyStateType> EnemyState { get; }
    void SetEnemyState(EnemyStateType enemyState);
  }
}