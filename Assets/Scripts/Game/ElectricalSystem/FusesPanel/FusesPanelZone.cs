using System;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.ElectricalSystem.Services;
using TelephoneBooth.Game.Interactable;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.FusesPanel
{
  public class FusesPanelZone : MonoBehaviour, IInteractable, ILockable
  {
    [Inject] private readonly IInputService _inputService;
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly ICameraMovementService _cameraMovementService;
    [Inject] private readonly IInteractiveCameraService _interactiveCameraService;
    [Inject] private readonly IGeneratorStateService _generatorStateService;
    [Inject] private readonly IFusesPanelService _fusesPanelService;
    [Inject] private readonly IInteractableContinuousService _interactableContinuousService;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    public bool IsLocked { get; private set; }

    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private Collider _collider;

    private IDisposable _disposable;

    private void Start()
    {
      _disposable = _generatorStateService.CurrentGeneratorState.Subscribe(state =>
      {
        //IsLocked = state == GeneratorStateType.NoFuel;
      });
    }

    public void Interact()
    {
      _gameStateService.SetGameState(GameStateType.GAME_INTERACTIVE);
      _collider.enabled = false;
      
      _cameraMovementService.SetCameraPoint(_cameraPoint,callback: () =>
      {
        _inputService.InteractHandler += InteractHandler;
        _interactiveCameraService.AddHandleCamera(15f, 15f);
        _fusesPanelService.EnableFusesPanel();
      });
    }
    
    private void InteractHandler(bool isInteracted)
    {
      if(!isInteracted) return;

      _interactableContinuousService.InteractContinuous(1f, () =>
      {
        _inputService.InteractHandler -= InteractHandler;
        _interactiveCameraService.RemoveHandleCamera();
        _fusesPanelService.DisableFusesPanel();
      
        _cameraMovementService.RollbackCamera(callback: () =>
        {
          _gameStateService.SetGameState(GameStateType.GAME);
          _collider.enabled = true;
        });
      });
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}