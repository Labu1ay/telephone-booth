using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Player.Factory;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Services
{
  public class PlayerMovementService : IPlayerMovementService, IInitializable, ILateDisposable
  {
    private readonly IPlayerFactory _playerFactory;
    
    private Transform _playerTransform;
    private Sequence _sequence;

    [Inject]
    public PlayerMovementService(IPlayerFactory playerFactory)
    {
      _playerFactory = playerFactory;
    }

    public async void Initialize()
    {
      _playerTransform = (await _playerFactory.GetPlayerAsync()).transform;
    }

    public async UniTask MoveToPosition(Transform needPoint, float duration = 0.5f)
    {
      
      _playerFactory.CharacterController.enabled = false;
      _sequence?.Kill();
      _sequence = DOTween.Sequence();
      _sequence.Append(_playerTransform.DOMove(needPoint.position, duration));
      _sequence.Join(_playerTransform.DORotate(needPoint.rotation.eulerAngles, duration));

      await _sequence.ToUniTask();
      _playerFactory.CharacterController.enabled = true;
    }

    public void LateDispose()
    {
      _sequence?.Kill();
    }
  }
}