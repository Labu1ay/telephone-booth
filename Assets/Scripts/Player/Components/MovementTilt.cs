using System;
using TelephoneBooth.Player.Configs;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Components
{
  public class MovementTilt : MonoBehaviour
  {
    [Inject] private readonly CharacterController _characterController;
    [Inject] private readonly PlayerController _playerController;
    [Inject] private readonly PlayerCameraConfig _setup;

    private Quaternion _installRotation;
    private Vector3 _movementVector;

    private IDisposable _disposable;

    private void Start()
    {
      _installRotation = transform.localRotation;

      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate());
    }

    private void EveryUpdate()
    {
      float movementX = _playerController.Vertical * _setup.RotationAmount;
      float movementZ = -_playerController.Horizontal * _setup.RotationAmount;
      
      _movementVector =
        new Vector3(
          _setup.CanMovementFX ? movementX + _characterController.velocity.y * _setup.MovementAmount : movementX, 0,
          movementZ);
      
      transform.localRotation = Quaternion.Lerp(transform.localRotation,
        Quaternion.Euler(_movementVector + _installRotation.eulerAngles), Time.deltaTime * _setup.RotationSmooth);
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}