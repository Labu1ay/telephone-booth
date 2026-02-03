using System;
using TelephoneBooth.Game.ElectricalSystem.Services;
using TelephoneBooth.Game.Interactable;
using TelephoneBooth.Game.TooltipSystem.Services;
using TelephoneBooth.InventorySystem;
using TelephoneBooth.InventorySystem.Services;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.Generator
{
  public class GeneratorTank : MonoBehaviour, ITooltipInteractable, ILockable
  {
    [Inject] private readonly ITooltipService _tooltipService;
    [Inject] private readonly IInteractableContinuousService _interactableContinuousService;
    [Inject] private readonly IGeneratorStateService _generatorStateService;
    [Inject] private readonly IInventoryService _inventoryService;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    public string TooltipText => "Press E to fill with fuel";

    private ItemTypeId _fuelTypeId = ItemTypeId.Fuel;

    public bool IsLocked { get; private set; }

    private IDisposable _disposable;

    private void Start()
    {
      _disposable = _generatorStateService.CurrentGeneratorState
        .Subscribe(state => IsLocked = state != GeneratorStateType.NoFuel);
    }

    public void Interact()
    {
      if (_inventoryService.HasItem(_fuelTypeId))
      {
        _interactableContinuousService.InteractContinuous(this, 3f, () =>
        {
          _inventoryService.RemoveItem(_fuelTypeId);
          _generatorStateService.SetGeneratorState(GeneratorStateType.GeneratorIsOff);
        });
      }
      else
      {
        _tooltipService.TryShowTemporaryTooltip("You need to find fuel");
      }
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}