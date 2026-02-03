using System;
using Cysharp.Threading.Tasks;

namespace TelephoneBooth.Game.Interactable
{
  public interface IInteractableContinuousService
  {
    UniTaskVoid InteractContinuous(float duration, Action onFinished);
    UniTaskVoid InteractContinuous(IInteractable interactable, float duration, Action onFinished);
  }
}