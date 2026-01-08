using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TelephoneBooth.Enemy.Services;
using TelephoneBooth.Player.Factory;
using TelephoneBooth.Player.Services;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.Components
{
  public class EnemyBehaviour : MonoBehaviour
  {
    [Inject] private readonly IEnemyStateService _enemyStateService;
    [Inject] private readonly IEnemyPatrolService _enemyPatrolService;
    [Inject] private readonly IPlayerFactory _playerFactory;
    [Inject] private readonly IPlayerVisibleService _playerVisibleService;
    
    [SerializeField] private EnemyMovement _enemyMovement;

    private Transform _playerTransform;
    private CancellationTokenSource _cts;
    
    private IDisposable _disposable;

    private async void Start()
    {
      _playerTransform = (await _playerFactory.GetPlayerAsync()).transform;
      
      _enemyStateService.EnemyStateStarted += EnemyStateStarted;
      _enemyStateService.EnemyStateFinished += EnemyStateFinished;
      _enemyMovement.Finished += MovementFinished;
      
      _enemyStateService.SetEnemyState(EnemyStateType.Look);
    }

    private void EnemyStateStarted(EnemyStateType enemyStateType)
    {
      switch (enemyStateType)
      {
        case EnemyStateType.Idle: SetEnemyIdle(); break;
        case EnemyStateType.Look: SetEnemyWaiting(); break;
        case EnemyStateType.Moving: SetEnemyMovementPoint(); break;
        case EnemyStateType.Chase: SetEnemyMovementTarget(); break;
        case EnemyStateType.LosePlayer: SetEnemyLostPlayer(); break;
      }
    }

    private void EnemyStateFinished(EnemyStateType enemyStateType)
    {
      
    }
    
    private void MovementFinished()
    {
      switch (_enemyStateService.EnemyState.Value)
      {
        case EnemyStateType.LosePlayer:
        case EnemyStateType.Moving: _enemyStateService.SetEnemyState(EnemyStateType.Look); break;
        case EnemyStateType.Chase: _enemyStateService.SetEnemyState(EnemyStateType.CatchPlayer); break;
        
      }
    }
    
    private void SetEnemyIdle()
    {
      _enemyMovement.MoveTo(transform.position);
    }

    private void SetEnemyWaiting() => 
      SetDelayBeforeAction(5f, () => _enemyStateService.SetEnemyState(EnemyStateType.Moving)).Forget();

    private void SetEnemyMovementPoint() => 
      _enemyMovement.MoveTo(_enemyPatrolService.GetRandomPatrolPosition());

    private void SetEnemyMovementTarget()
    {
      Cancel();
      _disposable?.Dispose();
      
      _playerVisibleService.SetVisibleStatus(true);
      
      _disposable = Observable.EveryUpdate().Subscribe(_ =>
      {
        _enemyMovement.MoveTo(_playerTransform.position);
      });
    }
    
    private void SetEnemyLostPlayer() => 
      SetDelayBeforeAction(1f, () =>
      {
        _disposable?.Dispose();
        _playerVisibleService.SetVisibleStatus(false);
      }).Forget();

    private async UniTaskVoid SetDelayBeforeAction(float delay, Action action)
    {
      _cts ??= new CancellationTokenSource();
      var cancellationHandler = await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _cts.Token).SuppressCancellationThrow();
      
      if(cancellationHandler) return;
      action.Invoke();
    }
    
    private void Cancel()
    {
      _cts?.Cancel();
      _cts?.Dispose();
      _cts = null;
    }

    private void OnDestroy()
    {
      _enemyStateService.EnemyStateStarted -= EnemyStateStarted;
      _enemyMovement.Finished -= MovementFinished;
      
      Cancel();

      _disposable?.Dispose();
    }
  }
}