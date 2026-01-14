using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TelephoneBooth.UI.Gameplay
{
  public class LoadingInteractable : MonoBehaviour
  {
    private const float FADE_DURATION = 0.1f;
    
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private Image _image;

    private Tween _tween;

    public void Show(float startProgress = 0f)
    {
      _image.fillAmount = startProgress;
      
      _tween?.Kill();
      _tween = _group.DOFade(1, FADE_DURATION);
    }

    public async UniTask Hide()
    {
      _tween?.Kill();
      _tween = _group.DOFade(0, FADE_DURATION);

      await _tween.ToUniTask();
    }

    public void SetProgress(float value, float maxValue)
    {
      var progress = value / maxValue;
      _image.fillAmount = progress;
    }
    
    private void OnDestroy()
    {
      _tween?.Kill();
    }
  }
}