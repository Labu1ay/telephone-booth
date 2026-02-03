using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TelephoneBooth.Game.ElectricalSystem.Data;
using TelephoneBooth.InventorySystem;
using UnityEngine;

namespace TelephoneBooth.Game.ElectricalSystem.Configs
{
  [CreateAssetMenu(fileName = "FusePanelItemConfig", menuName = "configs/FusePanelItemConfig", order = 0)]
  public class FusePanelItemConfig : SerializedScriptableObject
  {
    [OdinSerialize] private Dictionary<ItemTypeId, FusePanelItemData> _fusePanelItemsSettings = new();
    
    public FusePanelItemData GetFusePanelItemSettings(ItemTypeId itemTypeId) => 
    _fusePanelItemsSettings.ContainsKey(itemTypeId) ? _fusePanelItemsSettings[itemTypeId] : default;

    public ItemTypeId[] GetNeededFuseItems() => _fusePanelItemsSettings.Keys.ToArray();
  }
}