using System;
using UnityEngine;

namespace TelephoneBooth.Game
{
  public interface IInputService
  {
    event Action<bool> RunningHandler;
    event Action PausedHandler;
    event Action InteractHandler;
    event Action InventoryHandler;
    event Action LeftHandler;
    event Action RightHandler;
    Vector2 MouseAxis { get; }
    Vector2 Axis { get; }
    bool IsCrouched { get; }
    bool IsRunning { get; }
    bool IsJumped { get; }
  }
}