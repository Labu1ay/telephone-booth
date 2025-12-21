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
    [Inject] private readonly IPlayerCameraProvider _playerCameraProvider;
    [Inject] private readonly ISecurityCameraService _securityCameraService;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }

    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private SecurityCameraMonitor _securityCameraMonitor;

    public void Interact()
    {
        _gameStateService.SetGameState(GameStateType.INTERACTIVE);
        _playerCameraProvider.SetCameraPoint(_cameraPoint,callback: () =>
        {
          _inputService.InteractHandler += InteractHandler;
          _securityCameraService.EnableMonitor();
        });
      
    }

    private async void InteractHandler()
    {
      _inputService.InteractHandler -= InteractHandler;
      
      await _securityCameraService.DisableMonitor();
      
      _playerCameraProvider.RollbackCamera(callback: () =>
      {
        _gameStateService.SetGameState(GameStateType.GAME);
      });
    }
  }
}