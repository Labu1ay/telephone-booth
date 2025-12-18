using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.Interactable;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.SecurityCamera
{
  public class SecurityCameraZone : MonoBehaviour, IInteractable
  {
    [Inject] private readonly IInputService _inputService;
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly IPlayerCameraProvider _playerCameraProvider;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }

    [SerializeField] private Transform _cameraPoint;

    public void Interact()
    {
        _gameStateService.SetGameState(GameStateType.INTERACTIVE);
        _playerCameraProvider.SetCameraPoint(_cameraPoint,callback: () =>
        {
          _inputService.InteractHandler += InteractHandler;
        });
      
    }

    private void InteractHandler()
    {
      _inputService.InteractHandler -= InteractHandler;
      _playerCameraProvider.RollbackCamera(callback: () =>
      {
        _gameStateService.SetGameState(GameStateType.GAME);
      });
    }
  }
}