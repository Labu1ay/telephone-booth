using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.Interactable;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.Hideout
{
  public abstract class HideoutZone : MonoBehaviour, IInteractable
  {
    [Inject] protected readonly IInputService _inputService;
    [Inject] protected readonly ICameraMovementService _cameraMovementService;
    [Inject] protected readonly IInteractiveCameraService _interactiveCameraService;
    [Inject] protected readonly IGameStateService _gameStateService;
    [field: SerializeField] public InteractableOutline Outline { get; private set; }

    [SerializeField] protected Transform _cameraPoint;
    
    public virtual void Interact()
    {
      _gameStateService.SetGameState(GameStateType.INTERACTIVE);
    }
  }
}