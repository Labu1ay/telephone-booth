using System;
using Player.Configs;
using TelephoneBooth.Game;
using TelephoneBooth.Player.Configs;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Components
{
  public class HandsHolder : MonoBehaviour
  {
    [Inject] private readonly IInputService _inputService;
    [Inject] private readonly PlayerControllerConfig _playerSetup;
    [Inject] private readonly HandsHolderConfig _setup;
    [Inject] private readonly CharacterController _characterController;

    private float _toggleSpeedThreshold;
    private float _currentBobAmplitude;

    private Vector3 _initialLocalPosition;
    private Vector3 _initialLocalEulerRotation;
    private Vector3 _smoothedTargetPosition;
    private Vector3 _smoothedTargetRotation;

    private IDisposable _disposable;

    private void Start()
    {
      _toggleSpeedThreshold = _playerSetup.CroughSpeed * 1.5f;
      _currentBobAmplitude = _setup.Amount;

      _initialLocalPosition = transform.localPosition;
      _initialLocalEulerRotation = transform.localRotation.eulerAngles;

      _smoothedTargetPosition = _initialLocalPosition;
      _smoothedTargetRotation = _initialLocalEulerRotation;

      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate());
      _inputService.RunningHandler += UpdateBobAmplitudeOnSprintToggle;
    }

    private void EveryUpdate()
    {
      if (!_setup.Enabled)
        return;

      float playerHorizontalSpeed = new Vector3(
        _characterController.velocity.x,
        0f,
        _characterController.velocity.z).magnitude;

      ApplyHeadBobEffect(playerHorizontalSpeed);
      SmoothApplyPositionAndRotation();
    }

    private void UpdateBobAmplitudeOnSprintToggle(bool isRunning)
    {
      _currentBobAmplitude = isRunning 
        ? _setup.Amount * _setup.SprintAmount 
        : _setup.Amount / _setup.SprintAmount;
    }

    private void ApplyHeadBobEffect(float playerHorizontalSpeed)
    {
      float frameDelta = Time.deltaTime;

      _smoothedTargetPosition = Vector3.Lerp(_smoothedTargetPosition, _initialLocalPosition, frameDelta);
      _smoothedTargetRotation = Vector3.Lerp(_smoothedTargetRotation, _initialLocalEulerRotation, frameDelta);

      if (playerHorizontalSpeed <= _toggleSpeedThreshold)
        return;

      Vector3 bobDisplacement = ComputeBobDisplacement();

      if (_characterController.isGrounded)
      {
        _smoothedTargetPosition += bobDisplacement;
        _smoothedTargetRotation += new Vector3(-bobDisplacement.z, 0f, bobDisplacement.x) * (_setup.RotationMultiplier * 10f);
      }
      else
      {
        _smoothedTargetPosition += bobDisplacement * 0.5f;
      }
    }

    private Vector3 ComputeBobDisplacement()
    {
      float time = Time.time * _setup.Frequency;
      float interpolationSpeed = _setup.Smooth * Time.deltaTime;

      float verticalOffset = Mathf.Lerp(0f, Mathf.Sin(time) * _currentBobAmplitude * 2f, interpolationSpeed);
      float horizontalOffset = Mathf.Lerp(0f, Mathf.Cos(time * 0.5f) * _currentBobAmplitude * 1.3f, interpolationSpeed);

      return new Vector3(horizontalOffset, verticalOffset, 0f);
    }

    private void SmoothApplyPositionAndRotation()
    {
      float frameDelta = Time.deltaTime;

      transform.localPosition = Vector3.Lerp(
        transform.localPosition,
        _smoothedTargetPosition,
        _setup.Smooth * frameDelta);

      if (!_setup.EnabledRotationMovement)
        return;

      transform.localRotation = Quaternion.Lerp(
        transform.localRotation,
        Quaternion.Euler(_smoothedTargetRotation),
        _setup.Smooth / 1.5f * frameDelta);
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
      _inputService.RunningHandler -= UpdateBobAmplitudeOnSprintToggle;
    }
  }
}