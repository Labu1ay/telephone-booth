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
    
    private bool _isImageShowing = true;
    private bool _isTextShowing = true;

    public void Init(string text)
    {
      _text.text = text;
    }

    public void ShowImage()
    {
      if(_isImageShowing) return;
      _isImageShowing = true;
      
      ChangeFadeImage(1f);
    }

    public void HideImage()
    {
      if(!_isImageShowing) return;
      _isImageShowing = false;
      
      ChangeFadeImage(0f);
    }

    public void ShowText()
    {
      if(_isTextShowing) return;
      _isTextShowing = true;
      
      ChangeFadeText(1f);
    }

    public void HideText()
    {
      if(!_isTextShowing) return;
      _isTextShowing = false;
      
      ChangeFadeText(0f);
    }

    private void ChangeFadeImage(float fadeValue)
    {
      _tweenImage?.Kill();
      _tweenImage = Image.DOFade(fadeValue, FADE_DURATION);
    }

    private void ChangeFadeText(float fadeValue)
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