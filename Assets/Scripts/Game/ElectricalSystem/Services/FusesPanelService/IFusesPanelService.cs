using System;
using TelephoneBooth.InventorySystem;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public interface IFusesPanelService
  {
    event Action<bool> FusesPanelEnabled;
    int AvailableItemsCount { get; }
    void EnableFusesPanel();
    void DisableFusesPanel();
    ItemTypeId GetCurrentItemTypeId();
    void AddFuse(ItemTypeId itemTypeId);
    void RemoveFuse(ItemTypeId itemTypeId);
    void ShowNoFusesTooltip();
  }
}