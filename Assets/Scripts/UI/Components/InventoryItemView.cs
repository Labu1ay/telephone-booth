using TelephoneBooth.Core.Services;
using TelephoneBooth.InventorySystem.Configs;
using TelephoneBooth.InventorySystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TelephoneBooth.UI.Components
{
  public class InventoryItemView : MonoBehaviour
  {
    [Inject] private readonly InventoryItemViewConfig _setup;
    
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private TextMeshProUGUI _nameText;
    public void Init(InventorySlot slotData)
    {
      _countText.text = slotData.Count.ToString();
      
      if(slotData.Count <= 1)
        _countText.gameObject.SetActive(false);
      
      var data = _setup.GetItemViewData(slotData.ItemTypeId);
      _iconImage.sprite = data.Icon;
      _nameText.text = data.ItemName;
    }
  }
}