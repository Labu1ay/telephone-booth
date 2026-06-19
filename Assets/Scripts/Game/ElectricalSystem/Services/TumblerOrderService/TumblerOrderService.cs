using System.Collections.Generic;
using System.Linq;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.ElectricalSystem.Tumbler;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.Services.TumblerOrderService
{
  public class TumblerOrderService : ITumblerOrderService, IInitializable
  {
    private const string NEEDED_TUMBLER_ORDER_SAVE_KEY = "NeededTumblerOrder";
    private const int TUMBLER_COUNT = 16;

    private readonly ISavingService _savingService;
    private readonly ITumblerStateService _tumblerStateService;

    private List<TumblerButton> _tumblerButtons = new List<TumblerButton>();
    private SaveContainer<bool[]> _needTumblerOrder;
    private readonly Vector2Int _tumblerOnCountRange = new Vector2Int(7, 9);

    [Inject]
    public TumblerOrderService(ISavingService savingService, ITumblerStateService tumblerStateService)
    {
      _savingService = savingService;
      _tumblerStateService = tumblerStateService;
    }

    public void Initialize()
    {
      _needTumblerOrder = _savingService.GetPackage<bool[]>(NEEDED_TUMBLER_ORDER_SAVE_KEY);
      
      if (_needTumblerOrder.Item == null)
      {
        _needTumblerOrder.Item = GenerateTumblerOrder();
      }
      
#if UNITY_EDITOR
      var tumblerOrderDebug = "";

      for (var i = 0; i < _needTumblerOrder.Item.Length; i++)
      {
        tumblerOrderDebug += $"tumbler {i}: " + _needTumblerOrder.Item[i];
        tumblerOrderDebug += "||";
      }

      Debug.Log(tumblerOrderDebug);
#endif
    }
    
    public void AddTumblerButton(TumblerButton tumblerButton) => _tumblerButtons.Add(tumblerButton);
    
    public void CheckCorrectEnableTumblers()
    {
      if (_tumblerButtons.All(tumblerButton =>
            tumblerButton.TumblerButtonIsOn == _needTumblerOrder.Item[tumblerButton.TumblerIndex]))
      {
        _tumblerStateService.SetTumblerState(TumblerStateType.Active);
      }
    } 
    
    private bool[] GenerateTumblerOrder()
    {
      var needTumblerOrder = new bool[TUMBLER_COUNT];
      
      int targetCount = Random.Range(_tumblerOnCountRange.x, _tumblerOnCountRange.y + 1);
      List<int> indices = new List<int>();

      for (int i = 0; i < TUMBLER_COUNT; i++)
      {
        indices.Add(i);
      }

      for (int i = 0; i < targetCount; i++)
      {
        int random = Random.Range(0, indices.Count);
        needTumblerOrder[indices[random]] = true;
        indices.RemoveAt(random);
      }
      
      return needTumblerOrder;
    }
  }
}