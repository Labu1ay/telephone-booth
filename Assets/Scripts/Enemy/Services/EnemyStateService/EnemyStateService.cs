using System;
using UniRx;

namespace TelephoneBooth.Enemy.Services
{
  public class EnemyStateService : IEnemyStateService
  {
    private ReactiveProperty<EnemyStateType> _enemyState = new ReactiveProperty<EnemyStateType>();
    public ReadOnlyReactiveProperty<EnemyStateType> EnemyState => _enemyState.ToReadOnlyReactiveProperty();

    public event Action<EnemyStateType> EnemyStateStarted;
    public event Action<EnemyStateType> EnemyStateFinished;

    public void SetEnemyState(EnemyStateType enemyState)
    {
      EnemyStateFinished?.Invoke(_enemyState.Value);
      _enemyState.Value = enemyState;
      EnemyStateStarted?.Invoke(_enemyState.Value);
    }
  }
}