using DG.Tweening;
using TelephoneBooth.Game.Interactable;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TelephoneBooth.Game.Environments
{
  public class Door : MonoBehaviour, IInteractable
  {
    private const float OPEN_DURATION = 0.75f;

    [field: SerializeField] public InteractableOutline Outline { get; private set; }
    [SerializeField] private Collider _collider;
    [SerializeField] private Vector3 _openEulerAngles;

    private bool _isOpen;
    private Tween _tween;

    private CompositeDisposable _disposables = new CompositeDisposable();

    public void Interact()
    {
      if (_isOpen)
        CloseDoor();
      else
        OpenDoor();

      _isOpen = !_isOpen;
    }

    private void OpenDoor()
    {
      TriggerSubscribe();
      RotateDoor(_openEulerAngles, Ease.Linear);
    }

    private void CloseDoor()
    {
      _disposables?.Clear();
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
        .OnTriggerEnterAsObservable()
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