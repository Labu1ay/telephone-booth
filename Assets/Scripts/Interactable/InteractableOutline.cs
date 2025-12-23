using UnityEngine;

namespace TelephoneBooth.Game.Interactable
{
  [RequireComponent(typeof(QuickOutline))]
  public class InteractableOutline : MonoBehaviour
  {
    private const float START_WIDTH = 4f;

    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private QuickOutline _quickOutline;

    private Color _baseColor = Color.yellow;

    private void OnValidate()
    {
      _quickOutline ??= GetComponent<QuickOutline>();
    }

    private void Start()
    {
      if (_renderers == null) return;

      _quickOutline.Init(_renderers);
      _quickOutline.OutlineColor = _baseColor;
      _quickOutline.OutlineWidth = START_WIDTH;
      _quickOutline.OutlineMode = QuickOutline.Mode.OutlineAll;

      _quickOutline.enabled = false;
    }


    public void ShowOutline() => _quickOutline.enabled = true;

    public void HideOutline() => _quickOutline.enabled = false;
  }
}