using System.Collections.Generic;
using TelephoneBooth.Enemy.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.Components
{
  [RequireComponent(typeof(Animator))]
  public class EnemyAnimator : MonoBehaviour
  {
    [Inject] private readonly IEnemyStateService _enemyStateService;
    
    [SerializeField] private Animator _animator;

    private Dictionary<EnemyAnimationType, int> _animations = new ()
    {
      { EnemyAnimationType.Idle , Animator.StringToHash("Idle") },
      { EnemyAnimationType.Move , Animator.StringToHash("Move") },
      { EnemyAnimationType.Chase , Animator.StringToHash("Chase") },
      { EnemyAnimationType.Attack , Animator.StringToHash("Attack") }
    };

    private void OnValidate()
    {
      _animator ??= GetComponent<Animator>();
    }

    private void Start()
    {
      _enemyStateService.EnemyStateStarted += EnemyStateStarted;
    }

    private void EnemyStateStarted(EnemyStateType stateType)
    {
      switch (stateType)
      {
        case EnemyStateType.Idle: SetAnimation(EnemyAnimationType.Idle); break;
        case EnemyStateType.Moving: SetAnimation(EnemyAnimationType.Move); break;
        case EnemyStateType.Chase: SetAnimation(EnemyAnimationType.Chase); break;
      }
    }

    public void SetAnimation(EnemyAnimationType animationType)
    {
      _animator.SetTrigger(_animations[animationType]);
    }

    private void OnDestroy()
    {
      _enemyStateService.EnemyStateStarted -= EnemyStateStarted;
    }
  }
}