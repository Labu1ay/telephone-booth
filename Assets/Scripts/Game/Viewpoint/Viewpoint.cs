using System;
using Cysharp.Threading.Tasks;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Player.Factory;
using TelephoneBooth.UI;
using TelephoneBooth.UI.Screens;
using TelephoneBooth.UI.ScreenSystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class Viewpoint : MonoBehaviour
  {
    private const string VIEWPOINT_PATH = "ViewpointIcon";

    [Inject] private readonly IAssetService _assetService;
    [Inject] private readonly IPlayerCameraProvider _playerCameraProvider;
    [Inject] private readonly IPlayerFactory _playerFactory;
    [Inject] private readonly IScreenFactory _screenFactory;
    [Inject] private readonly DiContainer _diContainer;

    [SerializeField] private string _text = "Press E";

    [Space] 
    [SerializeField, Range(0.1f, 20)] private float _maxViewRange = 5;
    [SerializeField, Range(0.1f, 20)] private float _maxTextViewRange = 2;

    private Camera _camera;
    private GameObject _player;
    private float _distance;
    private ViewpointIcon _viewpointIcon;
    
    private bool _isImageShowed;
    private bool _isTextShowed;

    private IDisposable _disposable;

    private async void Start()
    {
      _player = await _playerFactory.GetPlayerAsync();
      _camera = await _playerCameraProvider.GetCameraAsync();

      CreateViewpoint().Forget();

      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate().Forget());
    }

    private async UniTaskVoid EveryUpdate()
    {
      if (_viewpointIcon)
      {
        _viewpointIcon.transform.position =
          _camera.WorldToScreenPoint(CalculateWorldPosition(transform.position, _camera));
      }

      _distance = Vector3.Distance(_player.transform.position, transform.position);

      TextShowHandler();
      ImageShowHandler().Forget();
    }

    private void TextShowHandler()
    {
      if (_distance < _maxTextViewRange && !_isTextShowed)
      {
        _isTextShowed = true;
        _viewpointIcon.ChangeFadeText(1f);
      }
      else if (_distance > _maxTextViewRange && _isTextShowed)
      {
        _isTextShowed = false;
        _viewpointIcon.ChangeFadeText(0f);
      }
    }

    private async UniTaskVoid ImageShowHandler()
    {
      if (_distance < _maxViewRange && !_isImageShowed)
      {
        var result = await CreateViewpoint();
        if(!result) return;
        
        _isImageShowed = true;
        
        await UniTask.WaitWhile(() => _viewpointIcon == null);
        _viewpointIcon.ChangeFadeImage(1f);
      }
      else if(_distance > _maxViewRange && _isImageShowed)
      {
        _isImageShowed = false;
        _viewpointIcon.ChangeFadeImage(0f, callback: RemoveViewpoint);
      }
    }

    private async UniTask<bool> CreateViewpoint()
    {
      var screen = _screenFactory.Get<GameScreen>();
      
      if (screen == null)
        return false;

      _viewpointIcon = await _assetService.InstantiateAsync<ViewpointIcon>(VIEWPOINT_PATH, _diContainer, screen.transform);

      _viewpointIcon.Init(_text);

      return true;
    }

    private void RemoveViewpoint()
    {
      Destroy(_viewpointIcon.gameObject);
      _viewpointIcon = null;
    }

    private Vector3 CalculateWorldPosition(Vector3 position, Camera camera)
    {
      Vector3 cameraNormal = camera.transform.forward;
      Vector3 vectorFromCamera = position - camera.transform.position;
      float camNormDot = Vector3.Dot(cameraNormal, vectorFromCamera.normalized);
      if (camNormDot <= 0f)
      {
        float camDot = Vector3.Dot(cameraNormal, vectorFromCamera);
        Vector3 proj = (cameraNormal * camDot * 1.01f);
        position = camera.transform.position + (vectorFromCamera - proj);
      }

      return position;
    }


    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}