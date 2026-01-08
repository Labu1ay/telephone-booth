using TelephoneBooth.Enemy.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.Components
{
  public class EnemyAnimatorStateHandler : MonoBehaviour
  {
    [Inject] private readonly IEnemyStateService _enemyStateService;

    [SerializeField] private EnemyAnimator _animator;
    
    private void Start()
    {
      _enemyStateService.EnemyStateStarted += EnemyStateStarted;
    }
    
    private void EnemyStateStarted(EnemyStateType stateType)
    {
      switch (stateType)
      {
        case EnemyStateType.Look: _animator.SetAnimation(EnemyAnimationType.Idle); break;
        case EnemyStateType.Moving: _animator.SetAnimation(EnemyAnimationType.Move); break;
        case EnemyStateType.Chase: _animator.SetAnimation(EnemyAnimationType.Chase); break;
      }
    }
    
    private void OnDestroy()
    {
      _enemyStateService.EnemyStateStarted -= EnemyStateStarted;
    }
    
  }
}