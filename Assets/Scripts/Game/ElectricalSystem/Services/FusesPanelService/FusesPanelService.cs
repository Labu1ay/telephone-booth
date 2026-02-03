using System;
using System.Collections.Generic;
using System.Linq;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.ElectricalSystem.Configs;
using TelephoneBooth.InventorySystem;
using TelephoneBooth.InventorySystem.Services;
using TelephoneBooth.UI.Screens;
using TelephoneBooth.UI.ScreenSystem;
using TelephoneBooth.Utils.Extensions;
using UniRx;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public class FusesPanelService : IFusesPanelService, IInitializable, ILateDisposable
  {
    private readonly IInputService _inputService;
    private readonly IInventoryService _inventoryService;
    private readonly IScreenManager _screenManager;
    private readonly IGameStateService _gameStateService;
    private readonly FusePanelItemConfig _setup;

    private ItemTypeId[] _neededItemTypes;
    
    private FusePanelScreen _screen;
    private List<ItemTypeId> _availableItemsTypeId = new ();
    private int _selectedIndex;
    
    public event Action<bool> FusesPanelEnabled;
    
    public int AvailableItemsCount => _availableItemsTypeId.Count;

    private IDisposable _disposable;

    [Inject]
    public FusesPanelService(
      IInventoryService inventoryService, 
      IScreenManager screenManager,
      IGameStateService gameStateService,
      IInputService inputService,
      FusePanelItemConfig setup)
    {
      _inventoryService = inventoryService;
      _screenManager = screenManager;
      _gameStateService = gameStateService;
      _inputService = inputService;
      _setup = setup;
    }
    
    public void Initialize()
    {
      _neededItemTypes = _setup.GetNeededFuseItems();
      
      _disposable = _gameStateService.GameState.Subscribe(state =>
      {
        if (state != GameStateType.DEATH) return;
        
        if(_screenManager.HasActiveScreen<FusePanelScreen>())
          _screenManager.DestroyScreen<FusePanelScreen>();
        
        Cleanup();
      });
    }

    public void EnableFusesPanel()
    {
      FusesPanelEnabled?.Invoke(true);
      
      var slotsData = _inventoryService.Slots.Where(i => _neededItemTypes.Contains(i.ItemTypeId)).ToList();

      foreach (var slot in slotsData) 
        _availableItemsTypeId.Add(slot.ItemTypeId);
    
      _screen = _screenManager.ShowScreen<FusePanelScreen>();
      
      foreach (var slotData in slotsData) 
        _screen.AddItem(slotData);
      
      _inputService.RightHandler += SelectNext;
      _inputService.LeftHandler += SelectPrevious;

      if (_availableItemsTypeId.Count == 0)
        return;
      
      _screen.SetSelectedItemActive(_availableItemsTypeId[_selectedIndex], true);
    }
    
    public void DisableFusesPanel()
    {
      FusesPanelEnabled?.Invoke(false);
      
      _screenManager.DestroyScreen<FusePanelScreen>();
      Cleanup();
    }
    
    public ItemTypeId GetCurrentItemTypeId() => _availableItemsTypeId[_selectedIndex];

    public void AddFuse(ItemTypeId itemTypeId)
    {
      _inventoryService.AddItem(itemTypeId);
      _availableItemsTypeId.Add(itemTypeId);
      
      var slotsData = _inventoryService.Slots.FirstOrDefault(i => i.ItemTypeId == itemTypeId);
      _screen.AddItem(slotsData);
      
      _screen.SetSelectedItemActive(_availableItemsTypeId[_selectedIndex], true);
    }

    public void RemoveFuse(ItemTypeId itemTypeId)
    {
      _screen.RemoveItem(itemTypeId);
      _inventoryService.RemoveItem(itemTypeId);

      _availableItemsTypeId.Remove(itemTypeId);

      if (_selectedIndex > _availableItemsTypeId.Count - 1) 
        _selectedIndex = (_availableItemsTypeId.Count - 1).ZeroIfNegative();
      
      if (_availableItemsTypeId.Count > 0)
        _screen.SetSelectedItemActive(_availableItemsTypeId[_selectedIndex], true);
    }
    
    public void ShowNoFusesTooltip() => _screen.ShowNoFusesTooltip();

    private void SelectNext()
    {
      if(_availableItemsTypeId.Count > 1)
        SelectItem((_selectedIndex + 1) % _availableItemsTypeId.Count);
    }

    private void SelectPrevious()
    {
      if(_availableItemsTypeId.Count > 1)
        SelectItem((_selectedIndex - 1 + _availableItemsTypeId.Count) % _availableItemsTypeId.Count);
    }

    private void SelectItem(int newIndex)
    {
      _screen.SetSelectedItemActive(_availableItemsTypeId[_selectedIndex], false);
      _selectedIndex = newIndex;
      _screen.SetSelectedItemActive(_availableItemsTypeId[_selectedIndex], true);
    }

    private void Cleanup()
    {
      _selectedIndex = 0;
      _availableItemsTypeId.Clear();
      
      _inputService.RightHandler -= SelectNext;
      _inputService.LeftHandler -= SelectPrevious;
    }

    public void LateDispose()
    {
      Cleanup();
      _disposable?.Dispose();
    }
  }
}