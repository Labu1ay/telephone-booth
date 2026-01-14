using TelephoneBooth.Core.Services;
using TelephoneBooth.Game.Interactable;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.SecurityCamera
{
  public class CameraSecurity : MonoBehaviour, ITooltipInteractable, ILockable
  {
    private const string SAVING_FORMAT_KEY = "CameraSecurity_{0}";
    private const float REPAIR_DURATION = 5f;

    [Inject] private readonly IInteractableContinuousService _interactableContinuousService;
    [Inject] private readonly ISavingService _savingService;
    
    [SerializeField] private string _cameraId;
    private SaveContainer<bool> _isAvailable;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    [field: SerializeField] public Camera Camera { get; private set; }
    
    public bool IsLocked => _isAvailable.Item;
    public bool IsAvailable => _isAvailable.Item;
    
    public string TooltipText => "Hold E to repair security camera";

    private void Start()
    {
      _isAvailable = _savingService.GetPackage<bool>(string.Format(SAVING_FORMAT_KEY, _cameraId));
    }

    public void Interact()
    {
      _interactableContinuousService.InteractContinuous(this, REPAIR_DURATION, () => _isAvailable.Item = true);
    }
  }
}