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
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Start()
    {
      _gameStateService.GameState.Subscribe(state =>
      {
        switch(state)
        {
          case GameStateType.INTERACTIVE: 
          case GameStateType.DEATH: 
            HideHands().Forget(); break;
          case GameStateType.GAME: ShowHands(); break;
        }
      })
      .AddTo(_disposables);
      
      Observable.EveryUpdate().Subscribe(_ => EveryUpdate()).AddTo(_disposables);
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
      _disposables?.Clear();
    }
  }
}