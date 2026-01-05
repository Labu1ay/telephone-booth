using Player.Configs;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Game;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Components
{
  public class PlayerController : MonoBehaviour
  {
    [Inject] private readonly IInputService _inputService;
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly IPlayerCameraProvider _playerCameraProvider;
    [Inject] private readonly PlayerControllerConfig _setup;
    [Inject] private readonly CharacterController _characterController;
    [Inject] private readonly Transform _cameraTransform;
    [Inject] private readonly Camera _camera;
    
    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }
    public float WalkingValue { get; private set; }
    
    private Vector3 _moveDirection = Vector3.zero;
    
    public bool MoveAvailable { get; private set; } = true;
    private bool _runningAvailable = true;
    
    private bool _isClimbing;
    private bool _isMoving;
    private bool _isRunning;
    private bool _isCrough;
    
    private float _lookVertical;
    private float _lookHorizontal;

    private float _rotationX;
    private float _installFOV;
    private float _runningValue;
    private float _installCroughHeight;
    private float _installCharacterRadius;

    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Start()
    {
      _gameStateService.GameState.Subscribe(state =>
      {
        switch (state)
        {
          case GameStateType.GAME: SetMoveAvailable(true); break;
          case GameStateType.MENU:
          case GameStateType.INTERACTIVE:
          case GameStateType.INVENTORY:
          case GameStateType.DEATH:
            SetMoveAvailable(false); break;
        }
      }).AddTo(_disposables);
      
      _playerCameraProvider.SetCamera(_camera);

      _installCroughHeight = _characterController.height;
      _installCharacterRadius = _characterController.radius;
      _installFOV = _camera.fieldOfView;

      _runningValue = _setup.RunningSpeed;
      WalkingValue = _setup.WalkingSpeed;

      Observable.EveryUpdate().Subscribe(_ =>
      {
        HandleGravity();
        HandleMovement();
        HandleJump();
        HandleCamera();
        HandleCrouch();
      }).AddTo(_disposables);
    }

    private void HandleGravity()
    {
      if (!_characterController.isGrounded && !_isClimbing)
        _moveDirection.y -= _setup.Gravity * Time.deltaTime;
    }

    private void HandleMovement()
    {
      Vector3 forward = transform.forward;
      Vector3 right = transform.right;

      _isRunning = !_isCrough && _runningAvailable && _inputService.IsRunning;

      float speed = MoveAvailable ? (_isRunning ? _runningValue : WalkingValue) : 0f;
      Vertical = speed * _inputService.Axis.y;
      Horizontal = speed * _inputService.Axis.x;

      _runningValue = Mathf.Lerp(_runningValue, _isRunning ? _setup.RunningSpeed : WalkingValue,
        _setup.TimeToRunning * Time.deltaTime);

      float y = _moveDirection.y;
      _moveDirection = forward * Vertical + right * Horizontal;
      _moveDirection.y = y;

      if(_characterController.enabled)
        _characterController.Move(_moveDirection * Time.deltaTime);
      
      _isMoving = Mathf.Abs(Horizontal) > 0.01f || Mathf.Abs(Vertical) > 0.01f;
    }

    private void HandleJump()
    {
      if (_inputService.IsJumped && MoveAvailable && _characterController.isGrounded && !_isClimbing)
        _moveDirection.y = _setup.JumpSpeed;
    }

    private void HandleCamera()
    {
      if (Cursor.lockState != CursorLockMode.Locked || !MoveAvailable) return;

      _lookVertical = -_inputService.MouseAxis.y;
      _lookHorizontal = _inputService.MouseAxis.x;
      _rotationX = Mathf.Clamp(_rotationX + _lookVertical * _setup.LookSpeed,
        -_setup.LookXLimit, _setup.LookXLimit);

      _cameraTransform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
      transform.rotation *= Quaternion.Euler(0, _lookHorizontal * _setup.LookSpeed, 0);

      float targetFov = (_isRunning && _isMoving) ? _setup.RunningFOV : _installFOV;
      _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFov, _setup.SpeedToFOV * Time.deltaTime);
    }

    private void HandleCrouch()
    {
      if (_inputService.IsCrouched &&  MoveAvailable)
      {
        _isCrough = true;
        _characterController.height = Mathf.Lerp(_characterController.height, _setup.CroughHeight, 5f * Time.deltaTime);
        _characterController.radius = Mathf.Lerp(_characterController.radius, _setup.CroughRadius, 5f * Time.deltaTime);
        
        WalkingValue = Mathf.Lerp(WalkingValue, _setup.CroughSpeed, 6f * Time.deltaTime);
      }
      else if (!Physics.Raycast(_cameraTransform.position, Vector3.up, 0.8f, 1) &&
               _characterController.height != _installCroughHeight)
      {
        _isCrough = false;
        _characterController.height = Mathf.Lerp(_characterController.height, _installCroughHeight, 6f * Time.deltaTime);
        _characterController.radius = Mathf.Lerp(_characterController.radius, _installCharacterRadius, 6f * Time.deltaTime);
        
        WalkingValue = Mathf.Lerp(WalkingValue, _setup.WalkingSpeed, 4f * Time.deltaTime);
      }
    }

    private void SetMoveAvailable(bool value) => MoveAvailable = value;
    
    public void SetRunningAvailable(bool value) => _runningAvailable = value;
    
    public void SetClimbingStatus(bool value) => _isClimbing = value;
    
    public void SetWalkingValue(float value) => WalkingValue = value;
    
    public void SetMoveDirection(Vector3 direction) => _moveDirection = direction;

    private void OnDestroy()
    {
      _disposables?.Clear();
    }
  }
}