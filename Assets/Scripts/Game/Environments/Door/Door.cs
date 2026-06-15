using DG.Tweening;
using TelephoneBooth.Game.Interactable;
using TelephoneBooth.Player.Factory;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.Environments
{
  public class Door : MonoBehaviour, ITooltipInteractable
  {
    private const float OPEN_DURATION = 0.75f;

    [Inject] private readonly IPlayerFactory _playerFactory;
    
    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    [SerializeField] private Collider _collider;
    [SerializeField] private float _openYAngle = 105f;

    private GameObject _player;
    private bool _isOpen;
    protected Tween _tween;

    private CompositeDisposable _disposables = new CompositeDisposable();
    
    public string TooltipText => "Press E to open or close the door";

    protected async virtual void Start()
    {
      _player = await _playerFactory.GetPlayerAsync();
    }

    public virtual void Interact()
    {
      if (_isOpen)
        CloseDoor();
      else
        OpenDoor(_player.transform);

      _isOpen = !_isOpen;
    }

    private void OpenDoor(Transform characterTransform)
    {
      _disposables?.Clear();
      
      Vector3 directionToPlayer = characterTransform.position - transform.position;
      directionToPlayer.y = 0; 

      float dot = Vector3.Dot(transform.forward, directionToPlayer.normalized);
      float targetAngle = (dot > 0) ? -_openYAngle : _openYAngle;

      RotateDoor(new Vector3(0, targetAngle, 0), Ease.Linear);
    }

    private void CloseDoor()
    {
      TriggerSubscribe();
      RotateDoor(Vector3.zero, Ease.Linear);
    }

    private void RotateDoor(Vector3 eulerAngles, Ease easeType)
    {
      _tween?.Kill();
      _tween = transform
        .DOLocalRotate(eulerAngles, OPEN_DURATION)
        .SetEase(easeType)
        .OnComplete(() =>
        {
          _disposables?.Clear();
        });
    }

    private void TriggerSubscribe()
    {
     _disposables?.Clear();

      _collider
        .OnTriggerStayAsObservable()
        .Subscribe(other =>
        {
          if (other.gameObject.layer != LayerMask.NameToLayer(Constants.PLAYER_LAYER)) return;

          if (_tween != null && _tween.IsActive() && _tween.IsPlaying())
            _tween.Pause();
        })
        .AddTo(_disposables);

      _collider
        .OnTriggerExitAsObservable()
        .Subscribe(other =>
        {
          if (other.gameObject.layer != LayerMask.NameToLayer(Constants.PLAYER_LAYER)) return;

          if (_tween != null && _tween.IsActive() && !_tween.IsPlaying())
            _tween.Play();
        })
        .AddTo(_disposables);
    }

    private void OnDestroy()
    {
      _tween?.Kill();
      _disposables?.Clear();
    }
  }
}