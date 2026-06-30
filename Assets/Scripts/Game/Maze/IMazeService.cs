using System;
using UniRx;
using UnityEngine;

namespace TelephoneBooth.Game.Maze
{
  public interface IMazeService
  {
    /// <summary>
    /// Последний сгенерированный лабиринт. Null если не генерировали.
    /// </summary>
    MazeData CurrentMaze { get; }

    ReactiveCommand MazeAndMapGenerated { get; }

    /// <summary>
    /// Генерирует данные лабиринта по конфигу. Не создаёт геометрию.
    /// </summary>
    MazeData Generate(MazeConfig config);

    /// <summary>
    /// Генерирует текстуру-карту лабиринта вид сверху.
    /// </summary>
    Texture2D RenderMap(MazeData data, MazeMapSettings settings);

    /// <summary>
    /// Находит путь от входа до выхода (BFS). 
    /// Возвращает список ячеек или null если пути нет.
    /// </summary>
    Vector2Int[] FindPath(MazeData data);
  }
}