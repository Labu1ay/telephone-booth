using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Enemy.Services;
using TelephoneBooth.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy
{
  public class BaseEnemy : MonoBehaviour
  {
    private const float ROTATE_DURATION = 0.5f;
    
    [Inject] private readonly IEnemyStateService _enemyStateService;

    [SerializeField] private Transform _enemyHeadTransform;

    private Sequence _sequence;

    public void SetIdle() => _enemyStateService.SetEnemyState(EnemyStateType.Idle);

    public async UniTask RotateTo(Vector3 target, bool withHead = false)
    {
      Vector3 directionToTarget = (target.SetY(0f) - transform.position).normalized;
      Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

      _sequence = DOTween.Sequence();
      _sequence.Append(transform.DORotateQuaternion(rotationToTarget, ROTATE_DURATION));
      
      if (withHead)
      {
        Vector3 directionHeadToTarget = (target - transform.position).normalized;
        Quaternion rotationHeadToTarget = Quaternion.LookRotation(directionHeadToTarget);
        
        _sequence.Join(_enemyHeadTransform.DORotateQuaternion(rotationHeadToTarget, ROTATE_DURATION));
      }
      
      await _sequence.ToUniTask();
    }
  }
}