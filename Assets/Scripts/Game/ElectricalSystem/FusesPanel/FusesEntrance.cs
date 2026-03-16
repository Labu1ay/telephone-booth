using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.ElectricalSystem.Factory;
using TelephoneBooth.Game.ElectricalSystem.Services;
using TelephoneBooth.Game.Interactable;
using TelephoneBooth.InventorySystem;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.FusesPanel
{
  public class FusesEntrance : MonoBehaviour, IInteractable
  {
    private const string PLACED_FUSE_ID_SAVE_KEY_FORMAT = "PlacedFuseIdInEntrance_{0}";
    
    [Inject] private readonly ISavingService _savingService;
    [Inject] private readonly IFusesPanelService _fusesPanelService;
    [Inject] private readonly IFusesPanelItemFactory _fusesPanelItemFactory;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    [field: SerializeField] public FusesEntranceTypeId FusesEntranceTypeId { get; private set; }
    
    [SerializeField] private Collider _collider;
    
    private FusePanelItem _placedFusePanelItem;
    private SaveContainer<ItemTypeId> _placedFuseIdSaveContainer;

    public ItemTypeId PlacedFuseId => _placedFuseIdSaveContainer.Item;
    
    private void Start()
    {
      _collider.enabled = false;

      _fusesPanelService.FusesPanelEnabled += FusesPanelEnabled;

      _placedFuseIdSaveContainer = _savingService.GetPackage<ItemTypeId>(string.Format(PLACED_FUSE_ID_SAVE_KEY_FORMAT, FusesEntranceTypeId));
      
      if(_placedFuseIdSaveContainer.Item != default)
        PutFuse(_placedFuseIdSaveContainer.Item);
    }

    private void FusesPanelEnabled(bool isEnabled) => _collider.enabled = isEnabled;

    public void Interact()
    {
      if (_placedFusePanelItem == null)
      {
        if (_fusesPanelService.AvailableItemsCount == 0)
        {
          _fusesPanelService.ShowNoFusesTooltip();
          return;
        }
        
        ItemTypeId itemTypeId = _fusesPanelService.GetCurrentItemTypeId();
        PutFuse(itemTypeId);
        _placedFuseIdSaveContainer.Item = _placedFusePanelItem.ItemTypeId;
        
        _fusesPanelService.RemoveFuse(itemTypeId);
        
        return;
      }
      
      TakeOutFuse();
    }

    private void PutFuse(ItemTypeId itemTypeId) => 
      _placedFusePanelItem = _fusesPanelItemFactory.CreateFusePanelItem(itemTypeId, transform);

    private void TakeOutFuse()
    {
      _fusesPanelService.AddFuse(_placedFusePanelItem.ItemTypeId);
      _placedFuseIdSaveContainer.Item = default;
      _fusesPanelItemFactory.RemoveFusePanelItem(_placedFusePanelItem);
    }

    private void OnDestroy()
    {
      _fusesPanelService.FusesPanelEnabled -= FusesPanelEnabled;
    }
  }
}