using System;

namespace TelephoneBooth.Core.Services
{
  [Serializable]
  public sealed class SaveContainer<T>
  {
    public T Item { get; set; }
  }
}