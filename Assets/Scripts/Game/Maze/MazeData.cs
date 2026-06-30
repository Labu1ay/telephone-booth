using UnityEngine;

namespace TelephoneBooth.Game.Maze
{
  public class MazeData
  {
    public int CellsX { get; }
    public int CellsZ { get; }
    public int Seed { get; }

    /// <summary>Горизонтальные стены [cellsX, cellsZ+1]</summary>
    public bool[,] HWalls { get; }

    /// <summary>Вертикальные стены [cellsX+1, cellsZ]</summary>
    public bool[,] VWalls { get; }

    /// <summary>Диапазон ячеек левой двери</summary>
    public Vector2Int LeftDoorRange { get; }

    /// <summary>Диапазон ячеек правой двери</summary>
    public Vector2Int RightDoorRange { get; }

    public MazeData(int cellsX, int cellsZ, int seed,
      bool[,] hWalls, bool[,] vWalls,
      Vector2Int leftDoor, Vector2Int rightDoor)
    {
      CellsX = cellsX;
      CellsZ = cellsZ;
      Seed = seed;
      HWalls = hWalls;
      VWalls = vWalls;
      LeftDoorRange = leftDoor;
      RightDoorRange = rightDoor;
    }

    /// <summary>
    /// Проверяет можно ли пройти из ячейки в указанном направлении.
    /// </summary>
    public bool CanPass(int x, int z, Direction dir)
    {
      switch (dir)
      {
        case Direction.Up:    return z + 1 <= CellsZ && !HWalls[x, z + 1];
        case Direction.Down:  return z - 1 >= -1     && !HWalls[x, z];
        case Direction.Right: return x + 1 <= CellsX && !VWalls[x + 1, z];
        case Direction.Left:  return x - 1 >= -1     && !VWalls[x, z];
        default: return false;
      }
    }

    /// <summary>
    /// Количество стен вокруг ячейки (0-4).
    /// </summary>
    public int CountWalls(int x, int z)
    {
      int count = 0;
      if (HWalls[x, z]) count++;
      if (HWalls[x, z + 1]) count++;
      if (VWalls[x, z]) count++;
      if (VWalls[x + 1, z]) count++;
      return count;
    }

    public enum Direction { Up, Down, Left, Right }
  }
}