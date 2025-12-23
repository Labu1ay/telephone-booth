using TelephoneBooth.InventorySystem.Configs;
using TelephoneBooth.InventorySystem.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.InventorySystem
{
  public class InventorySystemInstaller : MonoInstaller
  {
    [SerializeField] private InventoryItemViewConfig _inventoryItemViewConfig;
    
    public override void InstallBindings()
    {
      Container.Bind<InventoryItemViewConfig>().FromInstance(_inventoryItemViewConfig).AsSingle();
      Container.BindInterfacesAndSelfTo<InventoryService>().AsSingle();
    }
  }
}