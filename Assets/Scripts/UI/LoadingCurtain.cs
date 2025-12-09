using System;
using DG.Tweening;
using UnityEngine;

namespace TelephoneBooth.UI
{
  public class LoadingCurtain : MonoBehaviour {
    private const float FADE_DURATION = 1f;

    [SerializeField] private CanvasGroup _curtain;

    private Tween _tween;

    private void Start() => DontDestroyOnLoad(this);

    public void Show(Action action = null) {
      gameObject.SetActive(true);
      Fade(1f, action);
    }

    public void Hide(Action action = null) {
      Fade(0f, () => {
        gameObject.SetActive(false);
        action?.Invoke();
      });
    }

    private void Fade(float endValue, Action action = null) {
      _tween?.Kill();
      _tween = _curtain.DOFade(endValue, FADE_DURATION).OnComplete(() => action?.Invoke());
    }

    private void OnDestroy() {
      _tween?.Kill();
    }
  }
}