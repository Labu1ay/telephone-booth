using Sirenix.OdinInspector;
using TelephoneBooth.Core.Services;
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
    [Inject] private readonly ISavingService _savingService;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    [SerializeField] private string _interactTooltip;
    
    [SerializeField] private bool _singleItem = true;
    [HideIf("_singleItem")] [SerializeField] private string _addingSavingKey;
    
    private SaveContainer<bool> _isCollectedSaveContainer;

    private void Start()
    {
      var saveKey = _singleItem ? _itemTypeId.ToString() : string.Concat(_itemTypeId.ToString(), _addingSavingKey);
      _isCollectedSaveContainer = _savingService.GetPackage(saveKey, false);
      
      if(_isCollectedSaveContainer.Item)
        Destroy(gameObject);
    }

    public virtual void Interact()
    {
      _tooltipService.TryShowTemporaryTooltip(_interactTooltip);
      _inventoryService.AddItem(_itemTypeId);
      
      _isCollectedSaveContainer.Item = true;
      
      Destroy(gameObject);
    }
  }
}