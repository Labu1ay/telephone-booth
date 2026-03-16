using System;
using DG.Tweening;
using TelephoneBooth.Game.ElectricalSystem.Services;
using TelephoneBooth.Game.Interactable;
using TelephoneBooth.Game.TooltipSystem.Services;
using UniRx;
using UnityEngine;
using Zenject;
using Sequence = DG.Tweening.Sequence;

namespace TelephoneBooth.Game.ElectricalSystem.Generator
{
  public class GeneratorButton : MonoBehaviour, ITooltipInteractable, ILockable
  {
    private const float PRESSED_BUTTON_DURATION = 0.35f;
    
    [Inject] private readonly IGeneratorStateService _generatorStateService;
    [Inject] private readonly ITooltipService _tooltipService;

    [SerializeField] private Renderer _buttonRenderer;
    [SerializeField] private Transform _pressedButtonPoint;
    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    public string TooltipText => "Press E to start generator";
    private Transform _buttonTransform => _buttonRenderer.transform;
    
    public bool IsLocked { get; private set; }
    
    private Vector3 _startButtonPosition;
    private Sequence _sequence;
    private IDisposable _disposable;

    private void Start()
    {
      _startButtonPosition = _buttonTransform.position;

      _disposable = _generatorStateService.CurrentGeneratorState.Subscribe(state =>
      {
        _buttonRenderer.material.color = state switch
        {
          GeneratorStateType.NoFuel => Color.gray,
          GeneratorStateType.GeneratorIsOff => Color.red,
          GeneratorStateType.GeneratorIsOn => Color.green,
          _ => Color.gray
        };

        if (state != GeneratorStateType.GeneratorIsOn)
          IsLocked = false;
      });
    }

    public void Interact()
    {
      switch (_generatorStateService.CurrentGeneratorState.Value)
      {
        case GeneratorStateType.NoFuel:
          _tooltipService.TryShowTemporaryTooltip("You need to fill with fuel");
          break;
        case GeneratorStateType.GeneratorIsOff:
          PlayButtonAnimation(() => _generatorStateService.SetGeneratorState(GeneratorStateType.GeneratorIsOn));
          IsLocked = true;
          break;
      }
    }

    private void PlayButtonAnimation(Action pressedButtonAction = null)
    {
      _sequence?.Kill();
      
      _sequence = DOTween.Sequence();
      _sequence.Append(_buttonTransform.DOMove(_pressedButtonPoint.position, PRESSED_BUTTON_DURATION));
      _sequence.AppendCallback(() => pressedButtonAction?.Invoke());
      _sequence.Append(_buttonTransform.DOMove(_startButtonPosition, PRESSED_BUTTON_DURATION));
    }

    private void OnDestroy()
    {
      _sequence?.Kill();
      _disposable?.Dispose();
    }
  }
}