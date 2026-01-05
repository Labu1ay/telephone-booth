using System;
using TelephoneBooth.Enemy.Services;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace TelephoneBooth.Enemy.Components
{
  [RequireComponent(typeof(NavMeshAgent))]
  public class EnemyMovement : MonoBehaviour
  {
    [Inject] private readonly IEnemyStateService _enemyStateService;
    
    [SerializeField] private NavMeshAgent _agent;

    public event Action Finished;
    private bool _isChecking;

    private IDisposable _disposable;

    private void OnValidate()
    {
      _agent ??= GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 destination)
    {
      _agent.destination = destination;
      _agent.isStopped = false;

      CheckFinishPosition();
    }

    private void CheckFinishPosition()
    {
      if(_isChecking) return;
      _isChecking = true;
      
      _disposable = Observable.EveryUpdate().Subscribe(_ =>
      {
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
          _isChecking = false;
          _agent.isStopped = true;
          Finished?.Invoke();
          _disposable?.Dispose();
        }
      });
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}