using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace TelephoneBooth.Game.Maze
{
  public class MazeRoomGenerator : MonoBehaviour
  {
    private const string GeneratedRoot = "GeneratedMaze";

    [Inject] private IMazeService _mazeService;

    [Header("Конфигурация")]
    public MazeConfig config = new MazeConfig();

    [Header("Карта")]
    public MazeMapSettings mapSettings = new MazeMapSettings();

    [Header("Визуал")]
    public bool createFloor = true;
    public bool createCeiling = false;
    public Material wallMaterial;
    public Material floorMaterial;

    /// <summary>Последняя сгенерированная текстура карты</summary>
    public Texture2D MapTexture { get; private set; }

    /// <summary>Последние данные лабиринта</summary>
    public MazeData CurrentData { get; private set; }

    // ==================== ПУБЛИЧНЫЕ МЕТОДЫ ====================

    private async void Start()
    {
      await UniTask.Delay(100);
      GenerateMaze();
    }


    [ContextMenu("Generate Maze")]
    public void GenerateMaze()
    {
      EnsureService();
      ClearMaze();

      CurrentData = _mazeService.Generate(config);
      config.seed = CurrentData.Seed;

      BuildGeometry(CurrentData);
      MapTexture = _mazeService.RenderMap(CurrentData, mapSettings);

      _mazeService.MazeAndMapGenerated.Execute();

      Debug.Log($"Maze generated. Seed: {CurrentData.Seed} | " +
                $"Grid: {CurrentData.CellsX}x{CurrentData.CellsZ}", this);
    }

    [ContextMenu("Clear Maze")]
    public void ClearMaze()
    {
      for (int i = transform.childCount - 1; i >= 0; i--)
      {
        if (transform.GetChild(i).name == GeneratedRoot)
          DestroySafe(transform.GetChild(i).gameObject);
      }

      if (MapTexture != null)
      {
        DestroySafe(MapTexture);
        MapTexture = null;
      }

      CurrentData = null;
    }

    [ContextMenu("Generate Map Only")]
    public void GenerateMapOnly()
    {
      EnsureService();

      if (CurrentData == null)
      {
        Debug.LogWarning("Сначала сгенерируйте лабиринт!", this);
        return;
      }

      if (MapTexture != null) DestroySafe(MapTexture);
      MapTexture = _mazeService.RenderMap(CurrentData, mapSettings);
      Debug.Log("Map texture regenerated.", this);
    }

    /// <summary>
    /// Применяет текстуру карты на указанный Renderer (например на объект-записку).
    /// </summary>
    public void ApplyMapTo(Renderer targetRenderer)
    {
      if (MapTexture == null)
      {
        Debug.LogWarning("Карта не сгенерирована!", this);
        return;
      }

      MaterialPropertyBlock block = new MaterialPropertyBlock();
      targetRenderer.GetPropertyBlock(block);
      block.SetTexture("_MainTex", MapTexture);
      targetRenderer.SetPropertyBlock(block);
    }

    // ==================== ПОСТРОЕНИЕ ГЕОМЕТРИИ ====================

    private void BuildGeometry(MazeData data)
    {
      GameObject root = new GameObject(GeneratedRoot);
      root.transform.SetParent(transform, false);
      Transform parent = root.transform;

      float cellW = config.roomWidth / data.CellsX;
      float cellD = config.roomDepth / data.CellsZ;
      float halfW = config.roomWidth * 0.5f;
      float halfD = config.roomDepth * 0.5f;
      float halfH = config.wallHeight * 0.5f;
      float thick = config.wallThickness;

      // Пол
      if (createFloor)
      {
        var floor = MakeBox("Floor", parent,
          new Vector3(0, -0.05f, 0),
          new Vector3(config.roomWidth, 0.1f, config.roomDepth));
        if (floorMaterial) floor.GetComponent<Renderer>().sharedMaterial = floorMaterial;
      }

      // Потолок
      if (createCeiling)
      {
        var ceil = MakeBox("Ceiling", parent,
          new Vector3(0, config.wallHeight + 0.05f, 0),
          new Vector3(config.roomWidth, 0.1f, config.roomDepth));
        if (floorMaterial) ceil.GetComponent<Renderer>().sharedMaterial = floorMaterial;
      }

      // Внешние стены
      Wall("Front", parent, new Vector3(0, halfH, halfD),
        new Vector3(config.roomWidth, config.wallHeight, thick));
      Wall("Back", parent, new Vector3(0, halfH, -halfD),
        new Vector3(config.roomWidth, config.wallHeight, thick));

      BuildSideWithDoor("Left", parent, -halfW,
        data.LeftDoorRange.x, data.LeftDoorRange.y, cellD, halfD, halfH, thick);
      BuildSideWithDoor("Right", parent, halfW,
        data.RightDoorRange.x, data.RightDoorRange.y, cellD, halfD, halfH, thick);

      // Внутренние стены — горизонтальные
      for (int x = 0; x < data.CellsX; x++)
      {
        for (int z = 1; z < data.CellsZ; z++)
        {
          if (!data.HWalls[x, z]) continue;
          float cx = -halfW + x * cellW + cellW * 0.5f;
          float cz = -halfD + z * cellD;
          Wall($"H_{x}_{z}", parent,
            new Vector3(cx, halfH, cz), new Vector3(cellW, config.wallHeight, thick));
        }
      }

      // Внутренние стены — вертикальные
      for (int x = 1; x < data.CellsX; x++)
      {
        for (int z = 0; z < data.CellsZ; z++)
        {
          if (!data.VWalls[x, z]) continue;
          float cx = -halfW + x * cellW;
          float cz = -halfD + z * cellD + cellD * 0.5f;
          Wall($"V_{x}_{z}", parent,
            new Vector3(cx, halfH, cz), new Vector3(thick, config.wallHeight, cellD));
        }
      }
    }

    private void BuildSideWithDoor(string name, Transform parent, float xPos,
      int doorStart, int doorEnd, float cellD, float halfD, float halfH, float thick)
    {
      float startZ = -halfD;
      float endZ = halfD;
      float openStart = startZ + doorStart * cellD;
      float openEnd = startZ + (doorEnd + 1) * cellD;

      float lenA = openStart - startZ;
      if (lenA > 0.001f)
        Wall(name + "_A", parent,
          new Vector3(xPos, halfH, startZ + lenA * 0.5f),
          new Vector3(thick, config.wallHeight, lenA));

      float lenB = endZ - openEnd;
      if (lenB > 0.001f)
        Wall(name + "_B", parent,
          new Vector3(xPos, halfH, openEnd + lenB * 0.5f),
          new Vector3(thick, config.wallHeight, lenB));
    }

    private GameObject Wall(string name, Transform parent, Vector3 pos, Vector3 scale)
    {
      var go = MakeBox(name, parent, pos, scale);
      go.isStatic = true;
      if (wallMaterial) go.GetComponent<Renderer>().sharedMaterial = wallMaterial;
      return go;
    }

    private GameObject MakeBox(string name, Transform parent, Vector3 pos, Vector3 scale)
    {
      var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
      go.name = name;
      go.transform.SetParent(parent, false);
      go.transform.localPosition = pos;
      go.transform.localScale = scale;
      return go;
    }

    // ==================== УТИЛИТЫ ====================

    /// <summary>
    /// Для работы из эдитора без Zenject — создаём сервис вручную.
    /// </summary>
    private void EnsureService()
    {
      if (_mazeService == null)
        _mazeService = new MazeService();
    }

    private void DestroySafe(Object obj)
    {
#if UNITY_EDITOR
      if (!Application.isPlaying)
        DestroyImmediate(obj);
      else
        Destroy(obj);
#else
        Destroy(obj);
#endif
    }

    private void OnValidate()
    {
      config?.Clamp();
    }
  }
}