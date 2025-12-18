namespace TelephoneBooth.Game.Interactable
{
  public interface IInteractable
  {
    public InteractableOutline Outline { get; }
    public void Interact();
  }
}