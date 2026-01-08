using System;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.Interactable;
using TelephoneBooth.Game.SecurityCamera.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.SecurityCamera
{
  public class SecurityCameraZone : MonoBehaviour, IInteractable
  {
    [Inject] private readonly IInputService _inputService;
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly ICameraMovementService _cameraMovementService;
    [Inject] private readonly IInteractiveCameraService _interactiveCameraService;
    [Inject] private readonly ISecurityCameraService _securityCameraService;
    [Inject] private readonly IEnemyVisibleService _enemyVisibleService;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }

    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private SecurityCameraMonitor _securityCameraMonitor;

    private void Start()
    {
      _enemyVisibleService.DangerousTimeOvered += DangerousTimeOvered;
    }

    public void Interact()
    {
        _gameStateService.SetGameState(GameStateType.INTERACTIVE);
        _cameraMovementService.SetCameraPoint(_cameraPoint,callback: () =>
        {
          _inputService.InteractHandler += InteractHandler;
          _interactiveCameraService.AddHandleCamera(5f, 10f);
          _securityCameraService.EnableMonitor();
        });
      
    }

    private async void InteractHandler()
    {
      _inputService.InteractHandler -= InteractHandler;
      _interactiveCameraService.RemoveHandleCamera();
      await _securityCameraService.DisableMonitor();
      
      _cameraMovementService.RollbackCamera(callback: () =>
      {
        _gameStateService.SetGameState(GameStateType.GAME);
      });
    }
    
    private void DangerousTimeOvered()
    {
      _inputService.InteractHandler -= InteractHandler;
      _interactiveCameraService.RemoveHandleCamera();
      _cameraMovementService.SetCameraPoint(_cameraPoint);
    }

    private void OnDestroy()
    {
      _enemyVisibleService.DangerousTimeOvered -= DangerousTimeOvered;
    }
  }
}