using TelephoneBooth.Game.ElectricalSystem.Configs;
using TelephoneBooth.InventorySystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.FusesPanel
{
  public class FusePanelItem : MonoBehaviour
  {
    [Inject] private readonly FusePanelItemConfig _setup;
    
    [SerializeField] private Text _markText;
    
    public ItemTypeId ItemTypeId { get; private set; }

    public void Init(ItemTypeId itemTypeId)
    {
      ItemTypeId = itemTypeId;
      
      var settings = _setup.GetFusePanelItemSettings(itemTypeId);
      _markText.text = settings.MarkText;
    }
  }
}