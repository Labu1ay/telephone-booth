using System;
using TelephoneBooth.Game;
using TelephoneBooth.Player.Configs;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Components
{
  public class HandsSmooth : MonoBehaviour
  {
    [Inject] private readonly IInputService _inputService;
    [Inject] private readonly HandsSmoothConfig _setup;
    [Inject] private readonly CharacterController _characterController;
    [Inject] private readonly PlayerController _playerController;

    private float _crouchRotation;
    private Vector3 _startLocalPosition;
    private Quaternion _startLocalRotation;

    private IDisposable _disposable;

    private void Start()
    {
      _startLocalPosition = transform.localPosition;
      _startLocalRotation = transform.localRotation;

      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate());
    }

    private void EveryUpdate()
    {
      if(!_playerController.MoveAvailable) return;
      
      var axis = _inputService.Axis;
      var mouseAxis = _inputService.MouseAxis;

      float moveX = Mathf.Clamp(mouseAxis.x * _setup.Amount, -_setup.MaxAmount, _setup.MaxAmount);
      float moveY = Mathf.Clamp(mouseAxis.y * _setup.Amount, -_setup.MaxAmount, _setup.MaxAmount);

      Vector3 finalPosition = new Vector3(moveX, moveY + -_characterController.velocity.y / 60, 0);

      transform.localPosition = Vector3.Lerp(
        transform.localPosition,
        finalPosition + _startLocalPosition,
        Time.deltaTime * _setup.Smooth);

      float tiltX = Mathf.Clamp(mouseAxis.x * _setup.RotationAmount, -_setup.MaxRotationAmount,
        _setup.MaxRotationAmount);
      float tiltY = Mathf.Clamp(mouseAxis.y * _setup.RotationSmooth, -_setup.MaxRotationAmount,
        _setup.MaxRotationAmount);

      _crouchRotation = _setup.EnabledCroughRotation && _inputService.IsCrouched
        ? Mathf.Lerp(_crouchRotation, _setup.RotationCroughMultiplier, _setup.RotationCroughSmooth * Time.deltaTime)
        : Mathf.Lerp(_crouchRotation, 0f, _setup.RotationCroughSmooth * Time.deltaTime);

      Vector3 vector = new Vector3(Mathf.Max(axis.y * 0.4f, 0) * _setup.RotationMovementMultiplier, 0,
        axis.x * _setup.RotationMovementMultiplier);
      Vector3 finalRotation = new Vector3(-tiltY, 0, tiltX + _crouchRotation) + vector;

      transform.localRotation = Quaternion.Slerp(
        transform.localRotation,
        Quaternion.Euler(finalRotation) * _startLocalRotation,
        Time.deltaTime * _setup.RotationSmooth);
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}