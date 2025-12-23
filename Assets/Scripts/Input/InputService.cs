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

    public Vector2 Axis => new(Input.GetAxis(Horizontal), Input.GetAxis(Vertical));
    public Vector2 MouseAxis => new Vector2(Input.GetAxis(MouseX), Input.GetAxis(MouseY));

    public bool IsCrouched => Input.GetKey(CrouchKey);
    public bool IsRunning => Input.GetKey(RunKey);
    public bool IsJumped => Input.GetButton(Jump);

    public event Action<bool> RunningHandler;
    public event Action PausedHandler;
    public event Action InteractHandler;
    public event Action InventoryHandler;
    public event Action LeftHandler;
    public event Action RightHandler;

    private IDisposable _disposable;
    
    public void Initialize()
    {
      _disposable = Observable.EveryUpdate().Subscribe(_ =>
      {
        if(Input.GetKeyDown(RunKey))
          RunningHandler?.Invoke(true);
        else if(Input.GetKeyUp(RunKey))
          RunningHandler?.Invoke(false);
        
        if(Input.GetKeyDown(PauseKey))
          PausedHandler?.Invoke();
        
        if(Input.GetKeyDown(InteractKey))
          InteractHandler?.Invoke();
        
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
  }
}