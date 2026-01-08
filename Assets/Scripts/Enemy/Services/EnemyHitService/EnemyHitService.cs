using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Enemy.Components;
using TelephoneBooth.Game;
using TelephoneBooth.Utils.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace TelephoneBooth.Enemy.Services
{
  public class EnemyHitService : IEnemyHitService, IInitializable, ILateDisposable
  {
    private const float ROTATE_DURATION = 0.25f;

    private readonly IGameStateService _gameStateService;
    private readonly ICameraHitService _cameraHitService;
    private readonly IPlayerCameraProvider _playerCameraProvider;

    private readonly EnemyAnimator _animator;
    private readonly Transform _eyesPoint;
    private readonly Transform _enemyTransform;

    private Transform _cameraRootTransform;

    private Sequence _sequence;

    [Inject]
    public EnemyHitService(
      IGameStateService gameStateService,
      ICameraHitService cameraHitService,
      IPlayerCameraProvider playerCameraProvider,
      EnemyAnimator animator,
      [Inject(Id = "EyesPoint")] Transform eyesPoint,
      [Inject(Id = "EnemyTransform")] Transform enemyTransform)
    {
      _gameStateService = gameStateService;
      _cameraHitService = cameraHitService;
      _playerCameraProvider = playerCameraProvider;
      _animator = animator;
      _eyesPoint = eyesPoint;
      _enemyTransform = enemyTransform;
    }
    
    public async void Initialize()
    {
      await UniTask.WaitWhile(() => _playerCameraProvider.CameraRootTransform == null);
      _cameraRootTransform = _playerCameraProvider.CameraRootTransform;
    }

    public async UniTask Hit()
    {
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
      await UniTask.WaitWhile(() => _playerCameraProvider.CameraRootTransform == null);
      
      Vector3 directionToPlayer = (_cameraRootTransform.position.SetY(0f) - _enemyTransform.position).normalized;
      Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);

      Vector3 directionToEyes = (_eyesPoint.position - _cameraRootTransform.position).normalized;
      Quaternion rotationToEyes = Quaternion.LookRotation(directionToEyes);

      _sequence = DOTween.Sequence();
      _sequence.Append(_enemyTransform.DORotateQuaternion(rotationToPlayer, ROTATE_DURATION));
      _sequence.Join(_cameraRootTransform.DORotateQuaternion(rotationToEyes, ROTATE_DURATION));

      await _sequence.ToUniTask();
    }

    public void LateDispose()
    {
      _sequence?.Kill();
    }
  }
}