using System.Collections.Generic;
using UnityEngine;

namespace TelephoneBooth.Utils
{
  public class ObjectPool<T> where T : Component {
    private List<T> _pooledObjects = new List<T>();

    public ObjectPool() {}

    public ObjectPool(T obj, Transform parent, int startSize)
    {
      for (int i = 0; i < startSize; i++) 
        Instantiate(obj, parent);

      _pooledObjects.ForEach(Destroy);
    }

    public T Instantiate(T obj, Transform parent = null, Vector3 position = default, Quaternion rotation = default) {
      T prefab = GetPooledObject(obj, position, rotation);
        
      if(position != default) prefab.transform.position = position;
      if(rotation != default) prefab.transform.rotation = rotation;
      if(parent) prefab.transform.SetParent(parent);
        
      prefab.gameObject.SetActive(true);
        
      return prefab;
    }
    
    public void Destroy(T obj) => obj.gameObject.SetActive(false);

    public void Release()
    {
      _pooledObjects.RemoveAll(pooledObject =>
      {
        if (pooledObject.gameObject.activeInHierarchy) return false;
            
        Object.Destroy(pooledObject.gameObject);
        return true;
      });
    }

    private T GetPooledObject(T obj, Vector3 position, Quaternion rotation) {
      for (int i = 0; i < _pooledObjects.Count; i++) {
        if (!_pooledObjects[i].gameObject.activeInHierarchy) {
          return _pooledObjects[i];
        }
      }
      return CreateObject(obj, position, rotation);
    }

    private T CreateObject(T obj, Vector3 position, Quaternion rotation) {
      T prefab = Object.Instantiate(obj, position, rotation);
    
      prefab.gameObject.SetActive(false);
      _pooledObjects.Add(prefab);
        
      return prefab;
    }
  }
}