using DG.Tweening;
using TelephoneBooth.Game.TooltipSystem.Services;
using TelephoneBooth.InventorySystem;
using TelephoneBooth.InventorySystem.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.Environments
{
  public class ClosedDoor : Door
  {
    [Inject] private readonly IInventoryService _inventoryService;
    [Inject] private readonly ITooltipService _tooltipService;
    
    [SerializeField] private ItemTypeId _openedItemTypeId;
    [SerializeField] private bool _isOpened;

    public override void Interact()
    {
      if (_isOpened)
      {
        base.Interact();
        return;
      }
    
      if (_inventoryService.HasItem(_openedItemTypeId))
      {
        _inventoryService.RemoveItem(_openedItemTypeId);
        _isOpened = true;
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