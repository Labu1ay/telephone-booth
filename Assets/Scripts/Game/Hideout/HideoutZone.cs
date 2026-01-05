using System;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.Interactable;
using TelephoneBooth.Game.TooltipSystem.Services;
using TelephoneBooth.Player.Services;
using UniRx;
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
    [Inject] protected readonly IPlayerVisibleService _playerVisibleService;
    [Inject] protected readonly ITooltipService _tooltipService;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }

    [SerializeField] protected Transform _cameraPoint;

    private IDisposable _disposable;

    private void Start()
    {
      _disposable = _playerVisibleService.IsVisible.Subscribe(isVisible =>
      {
        Outline.ChangeOutlineColor(isVisible switch
        {
          true => Color.red,
          false => Color.yellow,
        });
      });
    }

    public virtual void Interact()
    {
      _gameStateService.SetGameState(GameStateType.INTERACTIVE);
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}