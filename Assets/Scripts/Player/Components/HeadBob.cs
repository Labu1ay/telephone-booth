using System;
using TelephoneBooth.Player.Configs;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Components
{
  public class HeadBob : MonoBehaviour
  {
    private const float TOGGLE_SPEED = 3.0f;
    
    [Inject] private readonly PlayerCameraConfig _setup;
    [Inject] private readonly CharacterController _characterController;

    private Vector3 _startLocalPosition;
    private Vector3 _startLocalRotation;
    private Vector3 _currentRotation;

    private IDisposable _disposable;

    private void Start()
    {
      _startLocalPosition = transform.localPosition;
      _startLocalRotation = transform.localRotation.eulerAngles;

      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate());
    }

    private void EveryUpdate()
    {
      if (!_setup.Enabled) return;
      
      CheckMotion();
      ResetPosition();
      
      if (_setup.EnabledRotationMovement)
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(_currentRotation),
          _setup.RotationMovementSmooth * Time.deltaTime);
    }

    private void CheckMotion()
    {
      float speed = new Vector3(_characterController.velocity.x, 0, _characterController.velocity.z).magnitude;
      if (speed < TOGGLE_SPEED) return;
      if (!_characterController.isGrounded) return;
      PlayMotion(HeadBobMotion());
    }

    private void PlayMotion(Vector3 movement)
    {
      transform.localPosition += movement;
      _currentRotation += new Vector3(-movement.x, -movement.y, movement.x) * _setup.RotationMovementAmount;
    }

    private Vector3 HeadBobMotion()
    {
      Vector3 pos = Vector3.zero;
      pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * _setup.Frequency) * _setup.Amount * 1.4f,
        _setup.Smooth * Time.deltaTime);
      pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * _setup.Frequency / 2f) * _setup.Amount * 1.6f,
        _setup.Smooth * Time.deltaTime);
      return pos;
    }

    private void ResetPosition()
    {
      if (transform.localPosition == _startLocalPosition) return;
      transform.localPosition = Vector3.Lerp(transform.localPosition, _startLocalPosition, 1 * Time.deltaTime);
      _currentRotation = Vector3.Lerp(_currentRotation, _startLocalRotation, 1 * Time.deltaTime);
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}