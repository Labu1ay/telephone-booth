using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Game.ElectricalSystem.FusesPanel;
using TelephoneBooth.Game.ElectricalSystem.Services;
using TelephoneBooth.Game.Interactable;
using TelephoneBooth.Game.TooltipSystem.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.ElectricalPanel
{
  public class PowerSwitch : MonoBehaviour, ITooltipInteractable
  {
    private const float SWITCH_DURATION = 0.5f;
    
    [Inject] private readonly IFusesOrderService _fusesOrderService;
    [Inject] private readonly IGeneratorStateService _generatorStateService;
    [Inject] private readonly IElectricalStateService _electricalStateService;
    [Inject] private readonly ITooltipService _tooltipService;
    
    [SerializeField] private FusesEntrance[] _fusesEntrances;
    [SerializeField] private Transform _switcherTransform;
    [SerializeField] private Vector3 _powerOffEulerAngle;
    [SerializeField] private Vector3 _powerOnEulerAngle;

    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    public string TooltipText => "Press E to switch power";

    private bool _isSwitching;
    private Tween _tween;
    
    public async void Interact()
    {
      if(_isSwitching) return;
      
      if (_generatorStateService.CurrentGeneratorState.Value != GeneratorStateType.GeneratorIsOn)
      {
        _tooltipService.TryShowTemporaryTooltip("First you need to start the generator");
        return;
      }
      
      switch (_electricalStateService.CurrentElectricalState.Value)
      {
        case ElectricalStateType.PowerIsOff: 
          TryStartEnergy().Forget();
          break;
        case ElectricalStateType.PowerIsOn:
          await StartSwitchAnimation(_powerOffEulerAngle);
          _electricalStateService.SetElectricalState(ElectricalStateType.PowerIsOff);
          break;
      }
    }
    
    private async UniTaskVoid TryStartEnergy()
    {
      await StartSwitchAnimation(_powerOnEulerAngle);
      
      var placedFuses = 
        _fusesEntrances.ToDictionary(f => f.FusesEntranceTypeId, f => f.PlacedFuseId);
      
      if (_fusesOrderService.CheckCorrectPlacedFuses(placedFuses))
      {
        _electricalStateService.SetElectricalState(ElectricalStateType.PowerIsOn);
        return;
      }

      StartSwitchAnimation(_powerOffEulerAngle).Forget();
      _generatorStateService.SetGeneratorState(GeneratorStateType.GeneratorIsOff);
      _tooltipService.TryShowTemporaryTooltip("Fuses are missing or incorrectly placed");
      // elector VFX
      // command to enemy
    }

    private async UniTask StartSwitchAnimation(Vector3 needRotateEulerAngle)
    {
      _isSwitching = true;
      
      _tween?.Kill();
      _tween = _switcherTransform.DORotate(needRotateEulerAngle, SWITCH_DURATION);

      await _tween.ToUniTask();
      _isSwitching = false;
    }

    private void OnDestroy()
    {
      _tween?.Kill();
    }
  }
}