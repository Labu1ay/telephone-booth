using DG.Tweening;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.TooltipSystem.Services;
using TelephoneBooth.InventorySystem;
using TelephoneBooth.InventorySystem.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.Environments
{
  public class ClosedDoor : Door
  {
    private const string FORMAT_SAVE_KEY = "ClosedDoor_{0}";
    
    [Inject] private readonly ISavingService _savingService;
    [Inject] private readonly IInventoryService _inventoryService;
    [Inject] private readonly ITooltipService _tooltipService;
    
    [SerializeField] private ItemTypeId _openedItemTypeId;
    
    private SaveContainer<bool> _isOpenedSaveContainer;

    protected override void Start()
    {
      base.Start();
      
      _isOpenedSaveContainer = _savingService.GetPackage(string.Format(FORMAT_SAVE_KEY, _openedItemTypeId), false);
    }

    public override void Interact()
    {
      if (_isOpenedSaveContainer.Item)
      {
        base.Interact();
        return;
      }
    
      if (_inventoryService.HasItem(_openedItemTypeId))
      {
        _inventoryService.RemoveItem(_openedItemTypeId);
        _isOpenedSaveContainer.Item = true;
        _tooltipService.TryShowTemporaryTooltip("The door is opened.", durationSeconds: 1f);
        return;
      }
    
      PlayLockedAnimation();
    }
    
    private void PlayLockedAnimation()
    {
      _tween?.Kill();
      
      _tween = transform
        .DOShakeRotation(0.35f, strength: new Vector3(0f, 2f, 0f), vibrato: 14, randomness: 0f, fadeOut: true)
        .SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
          transform.DOLocalRotate(Vector3.zero, 0.08f).SetEase(Ease.OutQuad);
        });
    }
  }
}