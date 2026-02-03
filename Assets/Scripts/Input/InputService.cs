using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class InputService : IInputService, IInitializable, ILateDisposable
  {
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical"; 
    
    private const string MouseX = "Mouse X";
    private const string MouseY = "Mouse Y";
    
    private const string Jump = "Jump";
    
    private const KeyCode CrouchKey = KeyCode.LeftControl;
    private const KeyCode RunKey = KeyCode.LeftShift;
    private const KeyCode InteractKey = KeyCode.E;
    private const KeyCode InventoryKey = KeyCode.I;
    private const KeyCode PauseKey = KeyCode.Escape;

    public Vector2 Axis => IsInputLocked ? Vector2.zero : new Vector2(Input.GetAxis(Horizontal), Input.GetAxis(Vertical));
    public Vector2 MouseAxis => IsInputLocked ? Vector2.zero : new Vector2(Input.GetAxis(MouseX), Input.GetAxis(MouseY));

    public bool IsCrouched => !IsInputLocked && Input.GetKey(CrouchKey);
    public bool IsRunning => !IsInputLocked && Input.GetKey(RunKey);
    public bool IsJumped => !IsInputLocked && Input.GetButton(Jump);
    
    public bool IsInputLocked { get; private set; }

    public event Action<bool> RunningHandler;
    public event Action<bool> InteractHandler;
    public event Action PausedHandler;
    public event Action InventoryHandler;
    public event Action LeftHandler;
    public event Action RightHandler;

    private IDisposable _disposable;
    
    public void Initialize()
    {
      _disposable = Observable.EveryUpdate().Subscribe(_ =>
      {
        if(IsInputLocked) return;
        
        if(Input.GetKeyDown(RunKey))
          RunningHandler?.Invoke(true);
        else if(Input.GetKeyUp(RunKey))
          RunningHandler?.Invoke(false);
        
        if(Input.GetKeyDown(PauseKey))
          PausedHandler?.Invoke();
        
        if(Input.GetKeyDown(InteractKey))
          InteractHandler?.Invoke(true);
        else if(Input.GetKeyUp(InteractKey))
          InteractHandler?.Invoke(false);
        
        if(Input.GetKeyDown(InventoryKey))
          InventoryHandler?.Invoke();
        
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
          RightHandler?.Invoke();

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
          LeftHandler?.Invoke();
      });
    }

    public void LateDispose()
    {
      _disposable?.Dispose();
    }
    
    public void SetInputLockedStatus(bool isLocked)
    {
      IsInputLocked = isLocked;
    }
  }
}