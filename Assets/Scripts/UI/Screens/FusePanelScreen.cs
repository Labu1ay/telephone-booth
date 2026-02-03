using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TelephoneBooth.Core.Services;
using TelephoneBooth.InventorySystem;
using TelephoneBooth.InventorySystem.Configs;
using TelephoneBooth.InventorySystem.Data;
using TelephoneBooth.InventorySystem.Services;
using TelephoneBooth.UI.Components;
using UnityEngine;
using Zenject;
using Screen = TelephoneBooth.UI.ScreenSystem.Screen;

namespace TelephoneBooth.UI.Screens
{
  public class FusePanelScreen : Screen
  {
    private const string FUSE_ITEM_VIEW_PATH = "UI/FuseItemView";
    private const float SPACE_OFFSET = 50f;
    
    [Inject] private readonly IAssetService _assetService;
    [Inject] private readonly DiContainer _diContainer;
    [Inject] private readonly InventoryItemViewConfig _viewConfig;
    [Inject] private readonly IInventoryService _inventoryService;

    [SerializeField] private RectTransform _fusesContent;
    [SerializeField] private CanvasGroup _tooltipGroup;
    
    private Dictionary<ItemTypeId, FuseItemView> _fuseItemsView = new Dictionary<ItemTypeId, FuseItemView>();

    private float? _itemViewWidth;
    
    private Sequence _sequence;

    public void SetSelectedItemActive(ItemTypeId itemTypeId, bool value)
    {
      var itemView = _fuseItemsView.FirstOrDefault(i => i.Key == itemTypeId).Value;
      itemView.SetSelectedImageActive(value);
    }

    public void AddItem(InventorySlot slotData)
    {
      var itemView = _assetService.Instantiate<FuseItemView>(FUSE_ITEM_VIEW_PATH, _diContainer, _fusesContent);
      itemView.Init(slotData);

      _itemViewWidth ??= itemView.GetComponent<RectTransform>().sizeDelta.x;
      
      _fuseItemsView.Add(slotData.ItemTypeId, itemView);

      ChangeContentXSize(true);
      
      _sequence?.Kill();
      _tooltipGroup.alpha = 0;
    }

    public void RemoveItem(ItemTypeId itemTypeId)
    {
      var itemView = _fuseItemsView[itemTypeId];
      _fuseItemsView.Remove(itemTypeId);
      
      Destroy(itemView.gameObject);
      
     ChangeContentXSize(false);
    }

    public void ShowNoFusesTooltip()
    {
      if (_sequence != null && _sequence.IsActive() && _sequence.IsPlaying()) 
        return;
    
      _sequence = DOTween.Sequence();
    
      _sequence.Append(_tooltipGroup.DOFade(1f, 0.5f));
      _sequence.AppendInterval(1f);
      _sequence.Append(_tooltipGroup.DOFade(0f, 0.5f));
    
      _sequence.SetAutoKill(true);
    }

    private void ChangeContentXSize(bool isAdding)
    {
      if(_itemViewWidth == null)
        return;
      
      var sizeDelta = _fusesContent.sizeDelta;
      sizeDelta.x += (_itemViewWidth.Value + SPACE_OFFSET) * (isAdding ? 1 : -1);
      _fusesContent.sizeDelta = sizeDelta;
    }

    private void OnDestroy()
    {
      _sequence?.Kill();
    }
  }
}