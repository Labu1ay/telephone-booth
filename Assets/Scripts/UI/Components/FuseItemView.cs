using TelephoneBooth.InventorySystem.Configs;
using TelephoneBooth.InventorySystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TelephoneBooth.UI.Components
{
  public class FuseItemView : MonoBehaviour
  {
    [Inject] private readonly InventoryItemViewConfig _setup;
    
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameText;

    [SerializeField] private GameObject _selectedImage;
    
    public void Init(InventorySlot slotData)
    {
      var data = _setup.GetItemViewData(slotData.ItemTypeId);
      
      _iconImage.sprite = data.Icon;
      _nameText.text = data.ItemName;
    }
    
    public void SetSelectedImageActive(bool value) => _selectedImage.SetActive(value);
  }
}