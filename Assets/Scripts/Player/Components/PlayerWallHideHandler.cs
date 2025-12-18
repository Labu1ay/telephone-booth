using System;
using Cysharp.Threading.Tasks;
using Player.Configs;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Player.Hands;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Components
{
  public class PlayerWallHideHandler : MonoBehaviour
  {
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly PlayerControllerConfig _setup;
    [Inject] private readonly Transform _cameraTransform;
    [Inject] private readonly HandAnimator _handAnimator;
    
    [SerializeField] private LayerMask _layerMask = 1;
    [SerializeField] private GameObject _hands;
    
    private bool _wallDistance;
    private IDisposable _disposable;

    private void Start()
    {
      _gameStateService.GameState.Subscribe(state =>
      {
        switch(state)
        {
          case GameStateType.INTERACTIVE: HideHands().Forget(); break;
          case GameStateType.GAME: ShowHands(); break;
        }
      });
      
      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate());
    }

    private void EveryUpdate()
    {
      if (!_setup.CanHideDistanceWall && _gameStateService.GameState.Value != GameStateType.GAME) 
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

    private async UniTaskVoid HideHands()
    {
      _handAnimator.SetAnimation(HandAnimationType.Hide, true);
      await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
      _hands.SetActive(false);
    }

    private void ShowHands()
    {
      _hands.SetActive(true);
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}