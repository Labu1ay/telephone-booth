using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TelephoneBooth.Game
{
  public class ViewpointIcon : MonoBehaviour
  {
    private const float FADE_DURATION = 0.2f;
    
    [field: SerializeField] public Image Image { get; private set; }
    [SerializeField] private TextMeshProUGUI _text;
    
    private Tween _tweenImage;
    private Tween _tweenText;

    public void Init(string text)
    {
      _text.text = text;
    }

    public void ChangeFadeImage(float fadeValue, Action callback = null)
    {
      _tweenImage?.Kill();
      _tweenImage = Image
        .DOFade(fadeValue, FADE_DURATION)
        .OnComplete(() => callback?.Invoke());
    }

    public void ChangeFadeText(float fadeValue)
    {
      _tweenText?.Kill();
      _text.DOFade(fadeValue, FADE_DURATION);
    }

    private void OnDestroy()
    {
      _tweenImage?.Kill();
      _tweenText?.Kill();
    }
  }
}