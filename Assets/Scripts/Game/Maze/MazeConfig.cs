using System;
using UnityEngine;

namespace TelephoneBooth.Game.Maze
{
  [Serializable]
  public class MazeConfig
  {
    [Header("Размер комнаты")]
    [Min(2f)] public float roomWidth = 20f;
    [Min(2f)] public float roomDepth = 20f;
    [Min(2f)] public float wallHeight = 3f;
    [Min(0.05f)] public float wallThickness = 0.2f;

    [Header("Сетка лабиринта")]
    [Min(2)] public int cellsX = 10;
    [Min(2)] public int cellsZ = 10;

    [Header("Двери")]
    [Range(0f, 1f)] public float leftDoorPosition = 0.5f;
    [Range(0f, 1f)] public float rightDoorPosition = 0.5f;
    [Min(1)] public int doorWidthInCells = 1;

    [Header("Сложность")]
    [Range(1, 5)] public int entranceBranches = 3;
    [Range(1, 6)] public int entranceConfusionRadius = 3;
    [Range(0f, 30f)] public float loopPercentage = 12f;
    [Range(0f, 50f)] public float falsePathPercentage = 30f;
    [Range(0, 4)] public int exitConfusion = 2;

    [Header("Сид")]
    public bool randomSeed = true;
    public int seed = 12345;

    public void Clamp()
    {
      roomWidth = Mathf.Max(2f, roomWidth);
      roomDepth = Mathf.Max(2f, roomDepth);
      wallHeight = Mathf.Max(2f, wallHeight);
      wallThickness = Mathf.Max(0.05f, wallThickness);
      cellsX = Mathf.Max(2, cellsX);
      cellsZ = Mathf.Max(2, cellsZ);
      doorWidthInCells = Mathf.Clamp(doorWidthInCells, 1, cellsZ);
    }

    public MazeConfig Clone()
    {
      return (MazeConfig)MemberwiseClone();
    }
  }
}