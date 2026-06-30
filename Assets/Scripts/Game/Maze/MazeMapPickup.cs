using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.Maze
{
  public class MazeMapPickup : MonoBehaviour
  {
    [Inject] private IMazeService _mazeService;

    [SerializeField] private Renderer mapRenderer;
    [SerializeField] private MazeMapSettings mapSettings;

    private IDisposable _disposable;

    private void Start()
    {
      _disposable = _mazeService.MazeAndMapGenerated.Subscribe(_ => ApplyMazeMap());
    }

    private void ApplyMazeMap()
    {
      // Берём текущий лабиринт и рендерим карту на объект
      MazeData data = _mazeService.CurrentMaze;
      if (data != null)
      {
        Texture2D map = _mazeService.RenderMap(data, mapSettings);

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        mapRenderer.GetPropertyBlock(block);
        block.SetTexture("_MainTex", map);
        mapRenderer.SetPropertyBlock(block);
      }
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}