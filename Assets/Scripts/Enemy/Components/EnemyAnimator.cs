using System.Collections.Generic;
using UnityEngine;

namespace TelephoneBooth.Enemy.Components
{
  [RequireComponent(typeof(Animator))]
  public class EnemyAnimator : MonoBehaviour
  {
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

    public void SetAnimation(EnemyAnimationType animationType)
    {
      _animator.SetTrigger(_animations[animationType]);
    }
  }
}