using System;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Enemy.Services;
using TelephoneBooth.Game;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.Components
{
  public class EnemyObservation : MonoBehaviour
  {
    [Inject] private readonly IPlayerCameraProvider _playerCameraProvider;
    [Inject] private readonly IEnemyStateService _enemyStateService;
    [Inject] private readonly IGameStateService _gameStateService;

    [SerializeField] private Transform _eyesTransform;
    
    [SerializeField] private float _viewDistance = 15f;
    [SerializeField] private float _viewAngle = 170f;
    
    private Transform _cameraTransform;
    private float _viewDistanceSqr;
    private bool _playerInView;
    
    private IDisposable _disposable;

    private async void Start()
    {
      _cameraTransform = (await _playerCameraProvider.GetCameraAsync()).transform;
      _viewDistanceSqr = _viewDistance * _viewDistance;
      
      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate());
    }

    private void EveryUpdate()
    {
      if (CanCurrentlySeePlayer())
      {
        if (_playerInView) return;
        
        _playerInView = true;
        _enemyStateService.SetEnemyState(EnemyStateType.Chase);
      }
      else
      {
        if (_enemyStateService.EnemyState.Value != EnemyStateType.Chase) return;
        
        _playerInView = false;
        _enemyStateService.SetEnemyState(EnemyStateType.LosePlayer);
      }
    }
    
    private bool CanCurrentlySeePlayer()
    {
      if(_gameStateService.GameState.Value == GameStateType.INTERACTIVE) return false;
      
      var toPlayer = _cameraTransform.position - _eyesTransform.position;
      var sqrDist = toPlayer.sqrMagnitude;

      if (sqrDist > _viewDistanceSqr)
        return false;

      var angle = Vector3.Angle(_eyesTransform.forward, toPlayer);
      if (angle > _viewAngle * 0.5f)
        return false;

      if (Physics.Raycast(_eyesTransform.position, toPlayer.normalized, out RaycastHit hit, Mathf.Sqrt(sqrDist)))
        return hit.collider.gameObject.layer == LayerMask.NameToLayer(Constants.PLAYER_LAYER);

      return false;
     }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}