using System;
using System.Collections.Generic;
using System.Linq;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.ElectricalSystem.Configs;
using TelephoneBooth.Game.ElectricalSystem.FusesPanel;
using TelephoneBooth.InventorySystem;
using Zenject;
using Random = UnityEngine.Random;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public class FusesOrderService : IFusesOrderService, IInitializable
  {
    private const string NEEDED_FUSES_ORDER_SAVE_KEY = "NeededFusesOrder";
    
    private readonly ISavingService _savingService;
    private readonly FusePanelItemConfig _setup;
    
    private SaveContainer<Dictionary<FusesEntranceTypeId, ItemTypeId>> _neededFusesOrder;

    public Dictionary<FusesEntranceTypeId, ItemTypeId> NeededFusesOrder => _neededFusesOrder.Item;

    [Inject]
    public FusesOrderService(ISavingService savingService, FusePanelItemConfig setup)
    {
      _savingService = savingService;
      _setup = setup;
    }

    public void Initialize()
    {
      _neededFusesOrder = _savingService.GetPackage<Dictionary<FusesEntranceTypeId, ItemTypeId>>(NEEDED_FUSES_ORDER_SAVE_KEY);

      if (_neededFusesOrder.Item == null)
      {
        _neededFusesOrder.Item = GetNewFusesOrder();
        _savingService.SaveKey(NEEDED_FUSES_ORDER_SAVE_KEY);
      }
    }

    public bool CheckCorrectPlacedFuses(Dictionary<FusesEntranceTypeId, ItemTypeId> placedFuses) => 
      placedFuses.All(placedFuse => placedFuse.Value == _neededFusesOrder.Item[placedFuse.Key]);

    private Dictionary<FusesEntranceTypeId, ItemTypeId> GetNewFusesOrder()
    {
      var fusesEntranceTypeId = Enum.GetValues(typeof(FusesEntranceTypeId))
        .Cast<FusesEntranceTypeId>()
        .Except(new[] { FusesEntranceTypeId.Unknown })
        .ToArray();

      var neededFuseItems = _setup.GetNeededFuseItems().ToList();
      var neededFusesOrder = new Dictionary<FusesEntranceTypeId, ItemTypeId>();

      foreach (var fuseEntranceTypeId in fusesEntranceTypeId)
      {
        var neededFuseItem = neededFuseItems[Random.Range(0, neededFuseItems.Count)];
        neededFuseItems.Remove(neededFuseItem);
          
        neededFusesOrder.Add(fuseEntranceTypeId, neededFuseItem);
      }

      return neededFusesOrder;
    }
  }
}