using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.UI.ScreenSystem
{
  public class ScreenSystemInstaller : MonoInstaller
  {
    [SerializeField] private List<Screen> _screens;
    
    public override void InstallBindings()
    {
      Container
        .BindInterfacesAndSelfTo<ScreenFactory>()
        .AsSingle()
        .WithArguments(_screens, transform);

      Container.BindInterfacesAndSelfTo<ScreenManager>().AsSingle();
    }
  }
}