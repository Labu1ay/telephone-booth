using Cysharp.Threading.Tasks;

namespace TelephoneBooth.Game.Interactable.Services
{
  public interface IInteractableTooltipService
  {
    UniTask TryShowTooltip(string tooltipText);
    void HideTooltip();
  }
}