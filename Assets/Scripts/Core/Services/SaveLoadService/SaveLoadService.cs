using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TelephoneBooth.Core.Services
{
  public class SaveLoadService : ISaveLoadService
  {
    public void Save(string key, string value) =>
      PlayerPrefs.SetString(key, value);

    public void Save<T>(string key, T obj) =>
      PlayerPrefs.SetString(key, JsonConvert.SerializeObject(obj, new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.All
      }));

    public string Load(string key) =>
      PlayerPrefs.GetString(key);

    public T Load<T>(string key)
    {
      if (!PlayerPrefs.HasKey(key)) return default;

      string savedValue = PlayerPrefs.GetString(key);
    
      try
      {
        return JsonConvert.DeserializeObject<T>(savedValue, new JsonSerializerSettings
        {
          TypeNameHandling = TypeNameHandling.All
        });
      }
      catch (JsonException)
      {
        try
        {
          return (T)Convert.ChangeType(savedValue, typeof(T));
        }
        catch
        {
          return default;
        }
      }
    }

    public bool HasKey(string key) =>
      PlayerPrefs.HasKey(key);
  }
}