using TelephoneBooth.Game.Interactable;
using TelephoneBooth.Game.TooltipSystem.Services;
using TelephoneBooth.InventorySystem.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.InventorySystem.ItemBase
{
  public class InteractableItem : Item, IInteractable
  {
    [Inject] private readonly IInventoryService _inventoryService;
    [Inject] private readonly ITooltipService _tooltipService;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    [SerializeField] private string _interactTooltip;
    
    public virtual void Interact()
    {
      _tooltipService.TryShowTemporaryTooltip(_interactTooltip);
      _inventoryService.AddItem(_itemTypeId);
      Destroy(gameObject);
    }
  }
}