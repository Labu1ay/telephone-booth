using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TelephoneBooth.InventorySystem.Data
{
  [Serializable]
  public struct InventoryItemViewData
  {
    public string ItemName;
    [PreviewField] public Sprite Icon;
  }
}