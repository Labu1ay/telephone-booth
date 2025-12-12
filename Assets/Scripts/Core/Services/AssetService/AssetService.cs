using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Core.Services
{

  public class AssetService : IAssetService
  {
    public GameObject Instantiate(string path, DiContainer diContainer, Transform parent = null)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return diContainer.InstantiatePrefab((GameObject)prefab, parent);
    }
    
    public GameObject Instantiate(string path, DiContainer diContainer, Vector3 position, Quaternion rotation, Transform parent = null)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return diContainer.InstantiatePrefab((GameObject)prefab, position, rotation, parent);
    }

    public T Instantiate<T>(string path, DiContainer diContainer, Transform parent = null)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return diContainer.InstantiatePrefab((GameObject)prefab, parent).GetComponent<T>();
    }

    public T Instantiate<T>(string path, DiContainer diContainer, Vector3 position, Quaternion rotation, Transform parent = null)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return diContainer.InstantiatePrefab((GameObject)prefab, position, rotation, parent).GetComponent<T>();
    }
    
    public T Instantiate<T>(T prefab, DiContainer diContainer, Transform parent = null) where T : Component => 
      diContainer.InstantiatePrefabForComponent<T>(prefab, parent);

    public T Load<T>(string path)
    {
      var prefab = Resources.LoadAsync(path).asset;
      return prefab.GetComponent<T>();
    }

    public async UniTaskVoid UnloadUnusedAssets() =>
      await Resources.UnloadUnusedAssets();
  }
}