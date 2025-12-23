using System;

namespace TelephoneBooth.InventorySystem.Data
{
  [Serializable]
  public class InventorySlot
  {
    public ItemTypeId ItemTypeId;
    public int Count;

    public InventorySlot(ItemTypeId itemTypeId, int count = 1)
    {
      ItemTypeId = itemTypeId;
      Count = count;
    }
  }
}