using System;
using Cysharp.Threading.Tasks;

namespace TelephoneBooth.Game.Interactable
{
  public interface IInteractableContinuousService
  {
    UniTaskVoid InteractContinuous(IInteractable interactable, float duration, Action onFinished);
  }
}