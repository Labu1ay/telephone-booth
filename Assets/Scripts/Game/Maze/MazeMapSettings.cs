using System;
using UnityEngine;

namespace TelephoneBooth.Game.Maze
{
    [Serializable]
  public class MazeMapSettings
  {
    [Header("Размер текстуры")]
    public int textureWidth = 512;
    public int textureHeight = 512;

    [Header("Цвета")]
    public Color backgroundColor = new Color(0.05f, 0.05f, 0.08f, 1f);
    public Color wallColor = new Color(0.6f, 0.55f, 0.45f, 1f);
    public Color pathColor = new Color(0.15f, 0.15f, 0.2f, 1f);
    public Color solutionColor = new Color(0.8f, 0.2f, 0.2f, 0.6f);
    public Color doorColor = new Color(0.2f, 0.8f, 0.3f, 1f);

    [Header("Опции")]
    public bool drawSolution = true;
    public bool drawDoors = true;
    [Range(1, 8)] public int wallPixelThickness = 2;

    [Header("Стиль для хоррора")]
    public bool addNoise = true;
    [Range(0f, 0.3f)] public float noiseIntensity = 0.08f;
    public bool addVignette = true;
    [Range(0f, 1f)] public float vignetteStrength = 0.4f;
  }
}