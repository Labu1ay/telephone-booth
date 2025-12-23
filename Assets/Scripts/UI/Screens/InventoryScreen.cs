using System;
using TelephoneBooth.Core.Services;
using TelephoneBooth.InventorySystem.Configs;
using TelephoneBooth.InventorySystem.Data;
using TelephoneBooth.InventorySystem.Services;
using TelephoneBooth.UI.Components;
using UI.Root.Screens.Shop.ManualLayout.ManualGrid;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Screen = TelephoneBooth.UI.ScreenSystem.Screen;

namespace TelephoneBooth.UI.Screens
{
  public class InventoryScreen : Screen
  {
    [Inject] private readonly IAssetService _assetService;
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly IInventoryService _inventoryService;
    [Inject] private readonly InventoryItemViewConfig _setup;
    [Inject] private readonly DiContainer _diContainer;

    [SerializeField] private ManualGridGroup _gridGroup;
    [SerializeField] private Button _closeButton;

    private IDisposable _disposable;

    private void Start()
    {
      LoadInventoryViewItems();

      _disposable = _closeButton.OnClickAsObservable().Subscribe(_ => CloseButtonHandler());
    }

    private void LoadInventoryViewItems()
    {
      foreach (InventorySlot slot in _inventoryService.Slots)
      {
        var itemView = _assetService.Instantiate<InventoryItemView>("UI/ItemView", _diContainer, _gridGroup.transform);
        itemView.Init(slot);
      }

      _gridGroup.Recalculate();
    }
    
    private void CloseButtonHandler() => _gameStateService.SetGameState(GameStateType.GAME);

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}