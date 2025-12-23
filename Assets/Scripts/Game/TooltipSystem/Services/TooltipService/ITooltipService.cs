using Cysharp.Threading.Tasks;

namespace TelephoneBooth.Game.TooltipSystem.Services
{
  public interface ITooltipService
  {
    UniTask TryShowTooltip(string tooltipText, float delaySeconds = 5f);
    UniTask TryShowTemporaryTooltip(string tooltipText, float delaySeconds = 0f, float durationSeconds = 1f);
    void HideTooltip();
  }
}