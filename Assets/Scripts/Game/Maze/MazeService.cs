using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace TelephoneBooth.Game.Maze
{
  public class MazeService : IMazeService
  {
    public MazeData CurrentMaze { get; private set; }
    public ReactiveCommand MazeAndMapGenerated { get; private set; } = new ReactiveCommand();

    // ==================== ГЕНЕРАЦИЯ ====================

    public MazeData Generate(MazeConfig config)
    {
      config = config.Clone();
      config.Clamp();

      int usedSeed = config.randomSeed ? Environment.TickCount : config.seed;
      System.Random rng = new System.Random(usedSeed);

      int cx = config.cellsX;
      int cz = config.cellsZ;

      bool[,] hWalls = new bool[cx, cz + 1];
      bool[,] vWalls = new bool[cx + 1, cz];

      // Все стены стоят
      Fill(hWalls, true);
      Fill(vWalls, true);

      // Recursive backtracker
      bool[,] visited = new bool[cx, cz];
      int startZ = DoorCenterCell(config.leftDoorPosition, cz);
      CarveRecursive(cx, cz, new Vector2Int(0, startZ), visited, hWalls, vWalls, rng);

      // Двери
      GetDoorRange(config.leftDoorPosition, config.doorWidthInCells, cz, out int ls, out int le);
      GetDoorRange(config.rightDoorPosition, config.doorWidthInCells, cz, out int rs, out int re);

      for (int z = ls; z <= le; z++) vWalls[0, z] = false;
      for (int z = rs; z <= re; z++) vWalls[cx, z] = false;

      // Замешательство у входа
      EntranceConfusion(cx, cz, config, hWalls, vWalls, rng);

      // Замешательство у выхода
      ExitConfusion(cx, cz, config, hWalls, vWalls, rng);

      // Петли
      AddLoops(cx, cz, config.loopPercentage, hWalls, vWalls, rng);

      // Ложные пути
      ExtendDeadEnds(cx, cz, config.falsePathPercentage, hWalls, vWalls, rng);

      var data = new MazeData(cx, cz, usedSeed, hWalls, vWalls,
        new Vector2Int(ls, le), new Vector2Int(rs, re));

      CurrentMaze = data;
      return data;
    }

    // ==================== КАРТА ====================

    public Texture2D RenderMap(MazeData data, MazeMapSettings s)
    {
      int w = s.textureWidth;
      int h = s.textureHeight;

      Color[] pixels = new Color[w * h];

      // Заливка фоном
      for (int i = 0; i < pixels.Length; i++)
        pixels[i] = s.backgroundColor;

      float cellW = (float)w / data.CellsX;
      float cellH = (float)h / data.CellsZ;

      // Рисуем проходы (пол ячеек)
      for (int cx = 0; cx < data.CellsX; cx++)
      {
        for (int cz = 0; cz < data.CellsZ; cz++)
        {
          int px = Mathf.RoundToInt(cx * cellW + cellW * 0.5f);
          int py = Mathf.RoundToInt(cz * cellH + cellH * 0.5f);
          int pw = Mathf.Max(1, Mathf.RoundToInt(cellW) - s.wallPixelThickness * 2);
          int ph = Mathf.Max(1, Mathf.RoundToInt(cellH) - s.wallPixelThickness * 2);
          FillRect(pixels, w, h, px - pw / 2, py - ph / 2, pw, ph, s.pathColor);
        }
      }

      // Рисуем проходы между ячейками
      for (int cx = 0; cx < data.CellsX; cx++)
      {
        for (int cz = 0; cz < data.CellsZ; cz++)
        {
          float centerX = cx * cellW + cellW * 0.5f;
          float centerY = cz * cellH + cellH * 0.5f;

          // Проход вправо
          if (cx + 1 < data.CellsX && !data.VWalls[cx + 1, cz])
          {
            float nextX = (cx + 1) * cellW + cellW * 0.5f;
            int fromX = Mathf.RoundToInt(centerX);
            int toX = Mathf.RoundToInt(nextX);
            int py = Mathf.RoundToInt(centerY);
            int ph = Mathf.Max(1, Mathf.RoundToInt(cellH) - s.wallPixelThickness * 2);
            FillRect(pixels, w, h, fromX, py - ph / 2, toX - fromX, ph, s.pathColor);
          }

          // Проход вверх
          if (cz + 1 < data.CellsZ && !data.HWalls[cx, cz + 1])
          {
            float nextY = (cz + 1) * cellH + cellH * 0.5f;
            int px = Mathf.RoundToInt(centerX);
            int fromY = Mathf.RoundToInt(centerY);
            int toY = Mathf.RoundToInt(nextY);
            int pw = Mathf.Max(1, Mathf.RoundToInt(cellW) - s.wallPixelThickness * 2);
            FillRect(pixels, w, h, px - pw / 2, fromY, pw, toY - fromY, s.pathColor);
          }
        }
      }

      // Рисуем стены
      // Горизонтальные
      for (int cx = 0; cx < data.CellsX; cx++)
      {
        for (int cz = 0; cz <= data.CellsZ; cz++)
        {
          if (!data.HWalls[cx, cz]) continue;
          int px = Mathf.RoundToInt(cx * cellW);
          int py = Mathf.RoundToInt(cz * cellH);
          int pw = Mathf.RoundToInt(cellW);
          FillRect(pixels, w, h, px, py - s.wallPixelThickness / 2,
            pw, s.wallPixelThickness, s.wallColor);
        }
      }

      // Вертикальные
      for (int cx = 0; cx <= data.CellsX; cx++)
      {
        for (int cz = 0; cz < data.CellsZ; cz++)
        {
          if (!data.VWalls[cx, cz]) continue;
          int px = Mathf.RoundToInt(cx * cellW);
          int py = Mathf.RoundToInt(cz * cellH);
          int ph = Mathf.RoundToInt(cellH);
          FillRect(pixels, w, h, px - s.wallPixelThickness / 2, py,
            s.wallPixelThickness, ph, s.wallColor);
        }
      }

      // Решение (путь)
      // if (s.drawSolution)
      // {
      //   Vector2Int[] path = FindPath(data);
      //   if (path != null)
      //   {
      //     for (int i = 0; i < path.Length - 1; i++)
      //     {
      //       float ax = path[i].x * cellW + cellW * 0.5f;
      //       float ay = path[i].y * cellH + cellH * 0.5f;
      //       float bx = path[i + 1].x * cellW + cellW * 0.5f;
      //       float by = path[i + 1].y * cellH + cellH * 0.5f;
      //   
      //       DrawLine(pixels, w, h, ax, ay, bx, by,
      //         Mathf.Max(2, s.wallPixelThickness), s.solutionColor);
      //     }
      //   }
      // }

      // Двери
      if (s.drawDoors)
      {
        // Левая дверь
        for (int z = data.LeftDoorRange.x; z <= data.LeftDoorRange.y; z++)
        {
          int py = Mathf.RoundToInt(z * cellH + cellH * 0.5f);
          int ph = Mathf.Max(2, Mathf.RoundToInt(cellH * 0.6f));
          FillRect(pixels, w, h, 0, py - ph / 2, s.wallPixelThickness * 2, ph, s.doorColor);
        }

        // Правая дверь
        for (int z = data.RightDoorRange.x; z <= data.RightDoorRange.y; z++)
        {
          int py = Mathf.RoundToInt(z * cellH + cellH * 0.5f);
          int ph = Mathf.Max(2, Mathf.RoundToInt(cellH * 0.6f));
          FillRect(pixels, w, h, w - s.wallPixelThickness * 2, py - ph / 2,
            s.wallPixelThickness * 2, ph, s.doorColor);
        }
      }

      // Шум
      if (s.addNoise)
      {
        System.Random noiseRng = new System.Random(data.Seed);
        for (int i = 0; i < pixels.Length; i++)
        {
          float noise = ((float)noiseRng.NextDouble() - 0.5f) * 2f * s.noiseIntensity;
          pixels[i].r = Mathf.Clamp01(pixels[i].r + noise);
          pixels[i].g = Mathf.Clamp01(pixels[i].g + noise);
          pixels[i].b = Mathf.Clamp01(pixels[i].b + noise);
        }
      }

      // Виньетка
      if (s.addVignette)
      {
        float cx2 = w * 0.5f;
        float cy2 = h * 0.5f;
        float maxDist = Mathf.Sqrt(cx2 * cx2 + cy2 * cy2);

        for (int y = 0; y < h; y++)
        {
          for (int x = 0; x < w; x++)
          {
            float dx = x - cx2;
            float dy = y - cy2;
            float dist = Mathf.Sqrt(dx * dx + dy * dy) / maxDist;
            float vignette = 1f - dist * dist * s.vignetteStrength;
            int idx = y * w + x;
            pixels[idx].r *= vignette;
            pixels[idx].g *= vignette;
            pixels[idx].b *= vignette;
          }
        }
      }

      Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
      {
        filterMode = FilterMode.Point,
        wrapMode = TextureWrapMode.Clamp,
        name = $"MazeMap_{data.Seed}"
      };

      tex.SetPixels(pixels);
      tex.Apply();

      return tex;
    }

    // ==================== ПОИСК ПУТИ ====================

    public Vector2Int[] FindPath(MazeData data)
    {
      int cx = data.CellsX;
      int cz = data.CellsZ;

      // От центра левой двери до центра правой
      int startZ = (data.LeftDoorRange.x + data.LeftDoorRange.y) / 2;
      int endZ = (data.RightDoorRange.x + data.RightDoorRange.y) / 2;
      Vector2Int start = new Vector2Int(0, startZ);
      Vector2Int end = new Vector2Int(cx - 1, endZ);

      bool[,] vis = new bool[cx, cz];
      Vector2Int[,] parent = new Vector2Int[cx, cz];

      for (int x = 0; x < cx; x++)
      for (int z = 0; z < cz; z++)
        parent[x, z] = new Vector2Int(-1, -1);

      Queue<Vector2Int> queue = new Queue<Vector2Int>();
      queue.Enqueue(start);
      vis[start.x, start.y] = true;
      bool found = false;

      while (queue.Count > 0)
      {
        Vector2Int cur = queue.Dequeue();
        if (cur == end) { found = true; break; }

        TryEnqueue(cur.x, cur.y, cur.x + 1, cur.y, data, vis, parent, queue);
        TryEnqueue(cur.x, cur.y, cur.x - 1, cur.y, data, vis, parent, queue);
        TryEnqueue(cur.x, cur.y, cur.x, cur.y + 1, data, vis, parent, queue);
        TryEnqueue(cur.x, cur.y, cur.x, cur.y - 1, data, vis, parent, queue);
      }

      if (!found) return null;

      // Восстанавливаем путь
      List<Vector2Int> path = new List<Vector2Int>();
      Vector2Int step = end;
      while (step.x != -1)
      {
        path.Add(step);
        step = parent[step.x, step.y];
      }
      path.Reverse();
      return path.ToArray();
    }

    private void TryEnqueue(int fromX, int fromZ, int toX, int toZ,
      MazeData data, bool[,] vis, Vector2Int[,] parent, Queue<Vector2Int> queue)
    {
      if (toX < 0 || toX >= data.CellsX || toZ < 0 || toZ >= data.CellsZ) return;
      if (vis[toX, toZ]) return;

      // Проверяем стену между ячейками
      bool blocked;
      if (fromX == toX)
        blocked = data.HWalls[fromX, Mathf.Max(fromZ, toZ)];
      else
        blocked = data.VWalls[Mathf.Max(fromX, toX), fromZ];

      if (blocked) return;

      vis[toX, toZ] = true;
      parent[toX, toZ] = new Vector2Int(fromX, fromZ);
      queue.Enqueue(new Vector2Int(toX, toZ));
    }

    // ==================== ПРИВАТНЫЕ МЕТОДЫ ГЕНЕРАЦИИ ====================

    private void CarveRecursive(int cx, int cz, Vector2Int start,
      bool[,] visited, bool[,] hWalls, bool[,] vWalls, System.Random rng)
    {
      Stack<Vector2Int> stack = new Stack<Vector2Int>();
      List<Vector2Int> neighbors = new List<Vector2Int>(4);

      visited[start.x, start.y] = true;
      stack.Push(start);

      while (stack.Count > 0)
      {
        Vector2Int cur = stack.Peek();
        GetUnvisited(cur, cx, cz, visited, neighbors);

        if (neighbors.Count == 0) { stack.Pop(); continue; }

        Vector2Int next = neighbors[rng.Next(neighbors.Count)];
        RemoveWall(cur, next, hWalls, vWalls);
        visited[next.x, next.y] = true;
        stack.Push(next);
      }
    }

    private void GetUnvisited(Vector2Int cell, int cx, int cz,
      bool[,] visited, List<Vector2Int> result)
    {
      result.Clear();
      if (cell.x + 1 < cx && !visited[cell.x + 1, cell.y]) result.Add(new Vector2Int(cell.x + 1, cell.y));
      if (cell.x - 1 >= 0 && !visited[cell.x - 1, cell.y]) result.Add(new Vector2Int(cell.x - 1, cell.y));
      if (cell.y + 1 < cz && !visited[cell.x, cell.y + 1]) result.Add(new Vector2Int(cell.x, cell.y + 1));
      if (cell.y - 1 >= 0 && !visited[cell.x, cell.y - 1]) result.Add(new Vector2Int(cell.x, cell.y - 1));
    }

    private void RemoveWall(Vector2Int a, Vector2Int b, bool[,] hWalls, bool[,] vWalls)
    {
      if (a.x == b.x)
        hWalls[a.x, Mathf.Max(a.y, b.y)] = false;
      else
        vWalls[Mathf.Max(a.x, b.x), a.y] = false;
    }

    private void EntranceConfusion(int cx, int cz, MazeConfig config,
      bool[,] hWalls, bool[,] vWalls, System.Random rng)
    {
      int centerZ = DoorCenterCell(config.leftDoorPosition, cz);
      int radius = config.entranceConfusionRadius;

      List<WallRef> candidates = new List<WallRef>();

      for (int x = 0; x < Mathf.Min(radius + 1, cx); x++)
      {
        for (int z = Mathf.Max(0, centerZ - radius);
             z <= Mathf.Min(cz - 1, centerZ + radius); z++)
        {
          if (z + 1 < cz && hWalls[x, z + 1])
            candidates.Add(new WallRef(true, x, z + 1));
          if (x + 1 < cx && vWalls[x + 1, z])
            candidates.Add(new WallRef(false, x + 1, z));
        }
      }

      Shuffle(candidates, rng);
      int count = Mathf.Min(config.entranceBranches * 2, candidates.Count);
      for (int i = 0; i < count; i++)
      {
        WallRef wr = candidates[i];
        if (wr.h) hWalls[wr.x, wr.z] = false;
        else vWalls[wr.x, wr.z] = false;
      }

      // Гарантированные развилки прямо у входа
      int forks = 0;
      if (cx > 1) { vWalls[1, centerZ] = false; forks++; }
      if (forks < config.entranceBranches && centerZ + 1 < cz)
      {
        hWalls[0, centerZ + 1] = false; forks++;
        if (cx > 1 && forks < config.entranceBranches)
        { vWalls[1, centerZ + 1] = false; forks++; }
      }
      if (forks < config.entranceBranches && centerZ - 1 >= 0)
      {
        hWalls[0, centerZ] = false; forks++;
        if (cx > 1 && forks < config.entranceBranches)
        { vWalls[1, centerZ - 1] = false; forks++; }
      }
      if (cx > 2)
      {
        for (int z = Mathf.Max(0, centerZ - 1);
             z <= Mathf.Min(cz - 1, centerZ + 1) && forks < config.entranceBranches * 2; z++)
        {
          if (z + 1 < cz && hWalls[1, z + 1])
          { hWalls[1, z + 1] = false; forks++; }
        }
      }
    }

    private void ExitConfusion(int cx, int cz, MazeConfig config,
      bool[,] hWalls, bool[,] vWalls, System.Random rng)
    {
      if (config.exitConfusion <= 0) return;

      int centerZ = DoorCenterCell(config.rightDoorPosition, cz);
      List<WallRef> candidates = new List<WallRef>();

      for (int x = Mathf.Max(1, cx - 3); x < cx; x++)
      {
        for (int z = Mathf.Max(0, centerZ - 2);
             z <= Mathf.Min(cz - 1, centerZ + 2); z++)
        {
          if (z + 1 <= cz - 1 && hWalls[x, z + 1])
            candidates.Add(new WallRef(true, x, z + 1));
          if (x - 1 >= 1 && vWalls[x, z])
            candidates.Add(new WallRef(false, x, z));
        }
      }

      Shuffle(candidates, rng);
      int count = Mathf.Min(config.exitConfusion, candidates.Count);
      for (int i = 0; i < count; i++)
      {
        WallRef wr = candidates[i];
        if (wr.h) hWalls[wr.x, wr.z] = false;
        else vWalls[wr.x, wr.z] = false;
      }
    }

    private void AddLoops(int cx, int cz, float percent,
      bool[,] hWalls, bool[,] vWalls, System.Random rng)
    {
      if (percent <= 0f) return;

      List<WallRef> inner = new List<WallRef>();
      for (int x = 0; x < cx; x++)
      for (int z = 1; z < cz; z++)
        if (hWalls[x, z]) inner.Add(new WallRef(true, x, z));

      for (int x = 1; x < cx; x++)
      for (int z = 0; z < cz; z++)
        if (vWalls[x, z]) inner.Add(new WallRef(false, x, z));

      Shuffle(inner, rng);
      int count = Mathf.RoundToInt(inner.Count * percent / 100f);
      for (int i = 0; i < count && i < inner.Count; i++)
      {
        WallRef wr = inner[i];
        if (wr.h) hWalls[wr.x, wr.z] = false;
        else vWalls[wr.x, wr.z] = false;
      }
    }

    private void ExtendDeadEnds(int cx, int cz, float percent,
      bool[,] hWalls, bool[,] vWalls, System.Random rng)
    {
      if (percent <= 0f) return;

      List<Vector2Int> deadEnds = new List<Vector2Int>();
      for (int x = 0; x < cx; x++)
      for (int z = 0; z < cz; z++)
        if (CountWalls(x, z, hWalls, vWalls) == 3)
          deadEnds.Add(new Vector2Int(x, z));

      Shuffle(deadEnds, rng);
      int count = Mathf.RoundToInt(deadEnds.Count * percent / 100f);

      for (int i = 0; i < count && i < deadEnds.Count; i++)
      {
        Vector2Int cell = deadEnds[i];
        List<WallRef> walls = GetStandingInner(cell, cx, cz, hWalls, vWalls);
        if (walls.Count > 0)
        {
          WallRef wr = walls[rng.Next(walls.Count)];
          if (wr.h) hWalls[wr.x, wr.z] = false;
          else vWalls[wr.x, wr.z] = false;
        }
      }
    }

    // ==================== УТИЛИТЫ ====================

    private int CountWalls(int x, int z, bool[,] hW, bool[,] vW)
    {
      int c = 0;
      if (hW[x, z]) c++;
      if (hW[x, z + 1]) c++;
      if (vW[x, z]) c++;
      if (vW[x + 1, z]) c++;
      return c;
    }

    private List<WallRef> GetStandingInner(Vector2Int cell, int cx, int cz,
      bool[,] hW, bool[,] vW)
    {
      List<WallRef> r = new List<WallRef>();
      int x = cell.x, z = cell.y;
      if (z > 0 && hW[x, z]) r.Add(new WallRef(true, x, z));
      if (z + 1 < cz && hW[x, z + 1]) r.Add(new WallRef(true, x, z + 1));
      if (x > 0 && vW[x, z]) r.Add(new WallRef(false, x, z));
      if (x + 1 < cx && vW[x + 1, z]) r.Add(new WallRef(false, x + 1, z));
      return r;
    }

    private int DoorCenterCell(float normalized, int cells)
    {
      return Mathf.Clamp(Mathf.RoundToInt(normalized * (cells - 1)), 0, cells - 1);
    }

    private void GetDoorRange(float normalized, int widthCells, int totalCells,
      out int start, out int end)
    {
      int w = Mathf.Clamp(widthCells, 1, totalCells);
      int center = DoorCenterCell(normalized, totalCells);
      start = center - w / 2;
      end = start + w - 1;
      if (start < 0) { end -= start; start = 0; }
      if (end > totalCells - 1) { start -= end - (totalCells - 1); end = totalCells - 1; }
      start = Mathf.Clamp(start, 0, totalCells - 1);
      end = Mathf.Clamp(end, 0, totalCells - 1);
    }

    private void Fill(bool[,] arr, bool value)
    {
      for (int i = 0; i < arr.GetLength(0); i++)
      for (int j = 0; j < arr.GetLength(1); j++)
        arr[i, j] = value;
    }

    private void Shuffle<T>(List<T> list, System.Random rng)
    {
      for (int i = list.Count - 1; i > 0; i--)
      {
        int j = rng.Next(i + 1);
        T tmp = list[i]; list[i] = list[j]; list[j] = tmp;
      }
    }

    private struct WallRef
    {
      public bool h;
      public int x, z;
      public WallRef(bool h, int x, int z) { this.h = h; this.x = x; this.z = z; }
    }

    // ==================== РИСОВАНИЕ ТЕКСТУРЫ ====================

    private void FillRect(Color[] pixels, int texW, int texH,
      int x, int y, int w, int h, Color color)
    {
      for (int py = y; py < y + h; py++)
      {
        if (py < 0 || py >= texH) continue;
        for (int px = x; px < x + w; px++)
        {
          if (px < 0 || px >= texW) continue;
          int idx = py * texW + px;
          if (color.a < 1f)
            pixels[idx] = Color.Lerp(pixels[idx], color, color.a);
          else
            pixels[idx] = color;
        }
      }
    }

    private void DrawLine(Color[] pixels, int texW, int texH,
      float x0, float y0, float x1, float y1, int thickness, Color color)
    {
      float dist = Mathf.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
      int steps = Mathf.Max(1, Mathf.RoundToInt(dist));
      int half = thickness / 2;

      for (int i = 0; i <= steps; i++)
      {
        float t = (float)i / steps;
        int px = Mathf.RoundToInt(Mathf.Lerp(x0, x1, t));
        int py = Mathf.RoundToInt(Mathf.Lerp(y0, y1, t));
        FillRect(pixels, texW, texH, px - half, py - half, thickness, thickness, color);
      }
    }
  }
}