using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Enemy.Services;
using TelephoneBooth.Game;
using TelephoneBooth.Player.Factory;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace TelephoneBooth.Enemy.Components
{
  public class EnemyCatcher : MonoBehaviour
  {
    private const float ROTATE_DURATION = 0.25f;
    
    [Inject] private readonly IEnemyStateService _enemyStateService;
    [Inject] private readonly IPlayerFactory _playerFactory;
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly ICameraHitService _cameraHitService;
    [Inject] private readonly IPlayerCameraProvider _playerCameraProvider;

    [SerializeField] private EnemyAnimator _animator;
    [SerializeField] private Transform _eyesPoint;
    
    private Transform _playerTransform;
    private Transform _cameraTransform;

    private Sequence _sequence;

    private async void Start()
    {
      _playerTransform = (await _playerFactory.GetPlayerAsync()).transform;
      _cameraTransform = (await _playerCameraProvider.GetCameraAsync()).transform;
      
      _enemyStateService.EnemyStateStarted += EnemyStateStarted;
    }

    private async void EnemyStateStarted(EnemyStateType stateType)
    {
      if(stateType != EnemyStateType.CatchPlayer) return;
      
      _gameStateService.SetGameState(GameStateType.DEATH);

      await RotateToHit();

      _animator.SetAnimation(EnemyAnimationType.Attack);
      
      _cameraHitService.Hit(EnemyAnimationType.Attack, async () =>
      {
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      });
    }

    private async UniTask RotateToHit()
    {
      Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
      Quaternion RotationToPlayer = Quaternion.LookRotation(directionToPlayer);
      
      Vector3 directionToEnemy = (transform.position - _playerTransform.position).normalized;
      Quaternion RotationToEnemy = Quaternion.LookRotation(directionToEnemy);
      
      Vector3 directionToEyes = (_eyesPoint.position - _cameraTransform.parent.parent.position).normalized;
      Quaternion RotationToEyes = Quaternion.LookRotation(directionToEyes);
      
      _sequence = DOTween.Sequence();
      _sequence.Append(transform.DORotateQuaternion(RotationToPlayer, ROTATE_DURATION));
      _sequence.Join(_playerTransform.DORotateQuaternion(RotationToEnemy, ROTATE_DURATION));
      _sequence.Join(_cameraTransform.parent.parent.DORotateQuaternion(RotationToEyes, ROTATE_DURATION));

      await _sequence.ToUniTask();
    }

    private void OnDestroy()
    {
      _enemyStateService.EnemyStateStarted -= EnemyStateStarted;
    }
  }
}