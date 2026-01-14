using DG.Tweening;
using UnityEngine;

namespace TelephoneBooth.UI.AnimatedComponents
{
  public class CanvasGroupFader : MonoBehaviour
  {
    [SerializeField] private CanvasGroup _faderGroup;
    
    [Space]
    [Range(0f, 1f)] [SerializeField] private float _minAlpha = 0f;
    [Range(0f, 1f)] [SerializeField] private float _maxAlpha = 1f;
    [Range(0f, 10f)] [SerializeField] private float _fadeDuration = 1f;
    [Range(0f, 10f)] [SerializeField] private float _fadeDelay = 0f;
    
    [Space]
    [SerializeField] private bool _playOnAwake = true;

    private Sequence _sequence;

    private void Start()
    {
      _faderGroup.alpha = _minAlpha;
      
      if(_playOnAwake)
        PlayFadeAnimation();
    }

    public void PlayFadeAnimation()
    {
      
      _sequence = DOTween.Sequence();
      _sequence.Append(_faderGroup.DOFade(_maxAlpha, _fadeDuration));
      _sequence.AppendInterval(_fadeDelay);
      _sequence.Append(_faderGroup.DOFade(_minAlpha, _fadeDuration));
      _sequence.AppendInterval(_fadeDelay);
      _sequence.SetLoops(-1);
    }
    
    public void StopFadeAnimation() => _sequence?.Kill();

    private void OnDestroy()
    {
      StopFadeAnimation();
    }
  }
}