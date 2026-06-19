using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.ElectricalSystem.Services;
using TelephoneBooth.Game.ElectricalSystem.Services.TumblerOrderService;
using TelephoneBooth.Game.Interactable;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.Tumbler
{
  public class TumblerButton : MonoBehaviour, ITooltipInteractable, ILockable
  {
    private const string TUMBLER_ENABLED_SAVING_KEY_FORMAT = "Tumbler_{0}_Enabled";
    private const float SWITCH_DURATION = 0.3f;
    
    [Inject] private readonly ISavingService _savingService;
    [Inject] private readonly ITumblerStateService _tumblerStateService;
    [Inject] private readonly ITumblerOrderService _tumblerOrderService;
    
    [SerializeField] private Transform _tumblerTransform;
    [SerializeField] private Vector3 _tumblerIsOnEulerAngle;
    [SerializeField] private Renderer _indicatorRenderer;

    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    [field: SerializeField] public int TumblerIndex { get; private set; }
    
    public bool TumblerButtonIsOn => _tumblerButtonIsOnSaveContainer.Item;
    public string TooltipText => "Press E to switch tumbler";
    public bool IsLocked { get; private set; }
    
    private SaveContainer<bool> _tumblerButtonIsOnSaveContainer;
    private Vector3 _tumblerIsOffEulerAngle;
    private bool _isSwitching;
    private Tween _tween;
    private IDisposable _disposable;

    private async void Start()
    {
      _tumblerOrderService.AddTumblerButton(this);
      _tumblerIsOffEulerAngle = _tumblerTransform.localEulerAngles;
      
      _tumblerButtonIsOnSaveContainer = _savingService.GetPackage<bool>(string.Format(TUMBLER_ENABLED_SAVING_KEY_FORMAT, TumblerIndex));
      
      await StartSwitchAnimation(_tumblerButtonIsOnSaveContainer.Item, true);

      _disposable = _tumblerStateService.CurrentTumblerState.Subscribe(state =>
      {
        if (state == TumblerStateType.Active)
        {
          IsLocked = true;
          _indicatorRenderer.material.color = Color.green;
        }
      });
    }
    
    public void Interact()
    {
      if(_isSwitching) return;
      
      _tumblerButtonIsOnSaveContainer.Item = !_tumblerButtonIsOnSaveContainer.Item;
      
      StartSwitchAnimation(_tumblerButtonIsOnSaveContainer.Item).Forget();
    }
    
    private async UniTask StartSwitchAnimation(bool isOn, bool force = false)
    {
      var needRotateEulerAngle = isOn ? _tumblerIsOnEulerAngle : _tumblerIsOffEulerAngle;
      var duration = force ? 0 : SWITCH_DURATION;
      
      _isSwitching = true;
      
      _tween?.Kill();
      _tween = _tumblerTransform.DOLocalRotate(needRotateEulerAngle, duration);

      await _tween.ToUniTask();
      
      _indicatorRenderer.material.color = isOn ? Color.green : Color.red;
      _tumblerOrderService.CheckCorrectEnableTumblers();
      
      _isSwitching = false;
    }

    private void OnDestroy()
    {
      _tween?.Kill();
      _disposable?.Dispose();
    }
    
  }
}