namespace TelephoneBooth.Game.Interactable
{
  public interface ITooltipInteractable : IInteractable
  {
    string TooltipText { get; }
  }
}