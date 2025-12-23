using System.Collections.Generic;
using TelephoneBooth.InventorySystem.Data;

namespace TelephoneBooth.InventorySystem.Services
{
  public interface IInventoryService
  {
    List<InventorySlot> Slots { get; }
    bool HasItem(ItemTypeId itemTypeId);
    bool AddItem(ItemTypeId itemTypeId, int amount = 1);
    void RemoveItem(ItemTypeId itemTypeId, int amount = 1);
  }
}