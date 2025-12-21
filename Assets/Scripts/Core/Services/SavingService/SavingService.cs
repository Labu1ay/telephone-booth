using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace TelephoneBooth.Core.Services
{
  public sealed class SavingService : ISavingService
  {
    private readonly Dictionary<string, object> _items = new();

    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
    {
      TypeNameHandling = TypeNameHandling.None,
      ObjectCreationHandling = ObjectCreationHandling.Replace,
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
      Formatting = Formatting.None
    };

    public SaveContainer<T> GetPackage<T>(string key, T defaultValue = default)
    {
      if (string.IsNullOrWhiteSpace(key))
        throw new ArgumentException("Key is null/empty.", nameof(key));

      if (_items.TryGetValue(key, out var cached))
      {
        if (cached is SaveContainer<T> package) return package;

        throw new InvalidOperationException(
          $"Key '{key}' already cached as '{cached.GetType().FullName}', " +
          $"requested '{typeof(SaveContainer<T>).FullName}'.");
      }

      if (PlayerPrefs.HasKey(key))
      {
        var loaded = LoadItem<SaveContainer<T>>(key);
        if (loaded != null)
        {
          _items[key] = loaded;
          return loaded;
        }
      }

      var created = new SaveContainer<T> { Item = defaultValue };
      _items[key] = created;
      return created;
    }

    public bool RemovePackage(string key)
    {
      if (string.IsNullOrWhiteSpace(key)) return false;

      var removed = _items.Remove(key);
      if (PlayerPrefs.HasKey(key)) PlayerPrefs.DeleteKey(key);
      return removed;
    }

    public void Save()
    {
      foreach (var item in _items)
        SaveItem(item.Key, item.Value);

      PlayerPrefs.Save();
    }

    public void SaveKey(string key)
    {
      if (string.IsNullOrWhiteSpace(key))
        throw new ArgumentException("Key is null/empty.", nameof(key));

      if (_items.TryGetValue(key, out var obj))
      {
        SaveItem(key, obj);
        PlayerPrefs.Save();
      }
    }

    private void SaveItem(string key, object obj)
    {
      var json = JsonConvert.SerializeObject(obj, _settings);
      PlayerPrefs.SetString(key, json);
    }

    private T LoadItem<T>(string key)
    {
      try
      {
        var json = PlayerPrefs.GetString(key, string.Empty);
        if (string.IsNullOrEmpty(json)) return default;
        return JsonConvert.DeserializeObject<T>(json, _settings);
      }
      catch
      {
        return default;
      }
    }
  }
}