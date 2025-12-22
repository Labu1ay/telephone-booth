namespace TelephoneBooth.Game.Interactable
{
  public interface IInteractable
  {
    InteractableOutline Outline { get; }
    void Interact();
  }
}