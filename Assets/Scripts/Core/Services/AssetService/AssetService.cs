using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Core.Services
{
  public class AssetService : IAssetService
  {
    public GameObject Instantiate(string path, DiContainer diContainer)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return diContainer.InstantiatePrefab((GameObject)prefab);
    }

    public GameObject Instantiate(string path, DiContainer diContainer, Transform parent)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return diContainer.InstantiatePrefab((GameObject)prefab, parent);
    }

    public T Instantiate<T>(string path, DiContainer diContainer)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return diContainer.InstantiatePrefab((GameObject)prefab).GetComponent<T>();
    }

    public T Instantiate<T>(string path, DiContainer diContainer, Transform parent)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return diContainer.InstantiatePrefab((GameObject)prefab, parent).GetComponent<T>();
    }

    public T Load<T>(string path)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return prefab.GetComponent<T>();
    }

    public async UniTaskVoid UnloadUnusedAssets() =>
      await Resources.UnloadUnusedAssets();
  }
}