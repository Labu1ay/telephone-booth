using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.ElectricalSystem.FusesPanel;
using TelephoneBooth.InventorySystem;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.Factory
{
  public class FusesPanelItemFactory : IFusesPanelItemFactory
  {
    private const string FUSE_PATH = "FusePanelItem";
    
    private readonly IAssetService _assetService;
    private readonly DiContainer _diContainer;

    [Inject]
    public FusesPanelItemFactory(IAssetService assetService, DiContainer diContainer)
    {
      _assetService = assetService;
      _diContainer = diContainer;
    }

    public FusePanelItem CreateFusePanelItem(ItemTypeId itemTypeId, Transform parent)
    {
      var fusePanelItem = _assetService.Instantiate<FusePanelItem>(FUSE_PATH, _diContainer, parent.position, Quaternion.identity, parent);
      fusePanelItem.Init(itemTypeId);

      return fusePanelItem;
    }

    public void RemoveFusePanelItem(FusePanelItem fusePanelItem)
    {
      Object.Destroy(fusePanelItem.gameObject);
    }
  }
}