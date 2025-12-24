using System;
using TelephoneBooth.Core.Services;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class InteractiveCameraService : IInteractiveCameraService, IInitializable, ILateDisposable
  {
    private const float LOOK_SPEED = 2f;
    
    private readonly IInputService _inputService;
    private readonly IPlayerCameraProvider _playerCameraProvider;
    private readonly IGameStateService _gameStateService;
    
    private Transform _cameraTransform;
    
    private float _lookVertical;
    private float _lookHorizontal;
    
    private float _rotationX;
    private float _rotationY;
    
    private float _baseX;
    private float _baseY;

    private IDisposable _disposable;

    [Inject]
    public InteractiveCameraService(
      IInputService inputService, 
      IPlayerCameraProvider playerCameraProvider,
      IGameStateService gameStateService)
    {
      _inputService = inputService;
      _playerCameraProvider = playerCameraProvider;
      _gameStateService = gameStateService;
    }

    public async void Initialize()
    {
      _cameraTransform = (await _playerCameraProvider.GetCameraAsync()).transform;
    }

    public void AddHandleCamera(float lookXLimit = 10f, float lookYLimit = 25f)
    {
      var euler = _cameraTransform.rotation.eulerAngles;
      _baseX = NormalizeAngle(euler.x);
      _baseY = NormalizeAngle(euler.y);

      _rotationX = 0f;
      _rotationY = 0f;

      _disposable?.Dispose();
      _disposable = Observable.EveryUpdate().Subscribe(_ =>
      {
        if(_gameStateService.GameState.Value == GameStateType.PAUSE) return;
        
        _rotationX = Mathf.Clamp(_rotationX + (-_inputService.MouseAxis.y) * LOOK_SPEED, -lookXLimit, lookXLimit);
        _rotationY = Mathf.Clamp(_rotationY + (_inputService.MouseAxis.x) * LOOK_SPEED, -lookYLimit, lookYLimit);

        _cameraTransform.rotation = Quaternion.Euler(_baseX + _rotationX, _baseY + _rotationY, 0f);
      });
    }

    public void RemoveHandleCamera()
    {
      _disposable?.Dispose();
    }

    private float NormalizeAngle(float angle)
    {
      angle %= 360f;
      if (angle > 180f) angle -= 360f;
      return angle;
    }
    
    public void LateDispose()
    {
      RemoveHandleCamera();
    }
  }
}