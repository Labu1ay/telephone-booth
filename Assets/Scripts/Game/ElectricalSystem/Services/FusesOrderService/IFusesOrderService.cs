using System.Collections.Generic;
using TelephoneBooth.Game.ElectricalSystem.FusesPanel;
using TelephoneBooth.InventorySystem;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public interface IFusesOrderService
  {
    Dictionary<FusesEntranceTypeId, ItemTypeId> NeededFusesOrder { get; }
    bool CheckCorrectPlacedFuses(Dictionary<FusesEntranceTypeId, ItemTypeId> placedFuses);
  }
}