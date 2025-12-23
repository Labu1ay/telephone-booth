using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TelephoneBooth.InventorySystem.Configs
{
  [CreateAssetMenu(fileName = "InventoryItemViewConfig", menuName = "configs/InventoryItemViewConfig", order = 0)]
  public class InventoryItemViewConfig : SerializedScriptableObject
  {
    [OdinSerialize] private Dictionary<ItemTypeId, InventoryItemViewData> _itemViews 
      = new Dictionary<ItemTypeId, InventoryItemViewData>();
    
    public InventoryItemViewData GetItemViewData(ItemTypeId itemTypeId) =>
      _itemViews.ContainsKey(itemTypeId) ? _itemViews[itemTypeId] : null;
  }

  [Serializable]
  public class InventoryItemViewData
  {
    public string ItemName;
    public Sprite Icon;
  }
}