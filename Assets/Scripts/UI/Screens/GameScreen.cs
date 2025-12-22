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
      SetTooltipTextFade(0f, 0f);
    }

    public void ShowTooltip(string tooltipText)
    {
      SetTooltipTextFade(0f, 0f);
      
      _tooltipText.text = tooltipText;
      SetTooltipTextFade(1f, TOOLTIP_FADE_DURATION);
    }
    
    public void HideTooltip() => SetTooltipTextFade(0f, TOOLTIP_FADE_DURATION);

    private void SetTooltipTextFade(float value, float duration)
    {
      _tooltipTween?.Kill();
      _tooltipTween = _tooltipText.DOFade(value, duration);
    }
  }
}