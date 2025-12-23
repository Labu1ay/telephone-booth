using System.Collections.Generic;
using TelephoneBooth.Core.Services;
using TelephoneBooth.InventorySystem.Data;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.InventorySystem.Services
{
  public class InventoryService : IInventoryService, IInitializable
  {
    private const string INVENTORY_SAVE_KEY = "Inventory";
    private const int INVENTORY_CAPACITY = 99;
    
    private readonly ISavingService _savingService;
    
    private SaveContainer<List<InventorySlot>> _slots;
    
    public List<InventorySlot> Slots => _slots.Item;

    [Inject]
    public InventoryService(ISavingService savingService)
    {
      _savingService = savingService;
    }

    public void Initialize()
    {
      _slots = _savingService.GetPackage(INVENTORY_SAVE_KEY, new List<InventorySlot>());
    }
    
    public bool HasItem(ItemTypeId itemTypeId) => 
      _slots.Item.Exists(x => x.ItemTypeId == itemTypeId);

    public bool AddItem(ItemTypeId itemTypeId, int amount = 1)
    {
      if (itemTypeId == default) return false;

      var slot = _slots.Item.Find(x => x.ItemTypeId == itemTypeId);
    
      if (slot != null)
      {
        slot.Count += amount;
        return true;
      }

      if (_slots.Item.Count < INVENTORY_CAPACITY)
      {
        _slots.Item.Add(new InventorySlot(itemTypeId, amount));
        return true;
      }

      Debug.Log("Inventory full!");
      return false;
    }
    
    public void RemoveItem(ItemTypeId itemTypeId, int amount = 1)
    {
      var slot = _slots.Item.Find(x => x.ItemTypeId == itemTypeId);
      if (slot == null) return;

      slot.Count -= amount;
      if (slot.Count <= 0) _slots.Item.Remove(slot);
    }
  }
}