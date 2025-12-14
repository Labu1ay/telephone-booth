using System;
using System.Collections.Generic;
using System.Linq;
using TelephoneBooth.Core.Services;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace TelephoneBooth.UI.ScreenSystem
{
  public class ScreenFactory : IScreenFactory
  {
    private readonly DiContainer _diContainer;
    private readonly IAssetService _assetService;
    private readonly List<Screen> _screens;
    private readonly Transform _parentTransform;
    
    private readonly List<Screen> _instances = new();

    [Inject]
    public ScreenFactory(DiContainer diContainer, IAssetService assetService, List<Screen> screens, Transform parentTransform)
    {
      _diContainer = diContainer;
      _assetService = assetService;
      _screens = screens;
      _parentTransform = parentTransform;
    }

    public Screen GetOrCreate<T>() where T : Screen
    {
      foreach (var screen in _instances.Where(screen => screen.GetType() == typeof(T)))
        return screen;

      var prefab = GetScreenPrefab<T>();

      var newScreen = _assetService.Instantiate(prefab, _diContainer, _parentTransform);
      _instances.Add(newScreen);
      return newScreen;
    }

    public Screen Get<T>() where T : Screen
    {
      foreach (var screen in _instances.Where(screen => screen.GetType() == typeof(T)))
        return screen;
      
      return null;
    }

    public void Destroy<T>() where T : Screen
    {
      foreach (var screen in _instances.ToList().Where(screen => screen.GetType() == typeof(T)))
      {
        _instances.Remove(screen);
        Object.Destroy(screen.gameObject);
        return;
      }

      Debug.LogWarning($"Screen {typeof(T)} was not found.");
    }

    private Screen GetScreenPrefab<T>() where T : Screen
    {
      foreach (var screen in _screens)
      {
        if(screen.GetType() == typeof(T))
          return screen;
      }

      throw new Exception($"Prefab {typeof(T)} was not found.");
    }
  }
}