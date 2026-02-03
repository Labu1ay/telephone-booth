using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Screen = TelephoneBooth.UI.ScreenSystem.Screen;

namespace TelephoneBooth.UI.Screens
{
  public class GameScreen : Screen
  {
    private const float TOOLTIP_FADE_DURATION = 0.5f;
    
    [SerializeField] private TextMeshProUGUI _tooltipText;

    private Tween _tooltipTween;

    private void Start()
    {
      ForceHideTooltip().Forget();
    }

    public void ShowTooltip(string tooltipText)
    {
      ForceHideTooltip().Forget();
      
      _tooltipText.text = tooltipText;
      SetTooltipTextFade(1f, TOOLTIP_FADE_DURATION).Forget();
    }
    
    public void HideTooltip() => SetTooltipTextFade(0f, TOOLTIP_FADE_DURATION);
    public async UniTask ForceHideTooltip() => await SetTooltipTextFade(0f, 0f);

    private async UniTask SetTooltipTextFade(float value, float duration)
    {
      _tooltipTween?.Kill();
      _tooltipTween = _tooltipText.DOFade(value, duration);
      await _tooltipTween.ToUniTask();
    }
  }
}