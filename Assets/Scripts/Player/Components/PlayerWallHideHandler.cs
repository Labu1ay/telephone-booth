using System;
using Player.Configs;
using TelephoneBooth.Player.Hands;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Components
{
  public class PlayerWallHideHandler : MonoBehaviour
  {
    [Inject] private readonly PlayerControllerConfig _setup;
    [Inject] private readonly Transform _cameraTransform;
    [Inject] private readonly HandAnimator _handAnimator;
    
    [SerializeField] private LayerMask _layerMask = 1;
    
    private bool _wallDistance;
    private IDisposable _disposable;

    private void Start()
    {
      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate());
    }

    private void EveryUpdate()
    {
      if (!_setup.CanHideDistanceWall) 
        return;

      bool nearWall = Physics.Raycast(
        _cameraTransform.position,
        transform.forward,
        _setup.HideDistance,
        _layerMask);

      if (nearWall == _wallDistance)
        return;

      _wallDistance = nearWall;
      _handAnimator.SetAnimation(HandAnimationType.Hide, _wallDistance);
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}