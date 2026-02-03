using TelephoneBooth.Game.ElectricalSystem.FusesPanel;
using TelephoneBooth.InventorySystem;
using UnityEngine;

namespace TelephoneBooth.Game.ElectricalSystem.Factory
{
  public interface IFusesPanelItemFactory
  {
    FusePanelItem CreateFusePanelItem(ItemTypeId itemTypeId, Transform parent);
    void RemoveFusePanelItem(FusePanelItem fusePanelItem);
  }
}