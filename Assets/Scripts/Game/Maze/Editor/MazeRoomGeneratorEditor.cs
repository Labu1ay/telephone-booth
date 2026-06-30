namespace TelephoneBooth.Game.Maze.Editor
{
  #if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeRoomGenerator))]
public class MazeRoomGeneratorEditor : Editor
{
    private Texture2D previewTexture;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MazeRoomGenerator gen = (MazeRoomGenerator)target;

        EditorGUILayout.Space(8);

        // Кнопки
        GUI.backgroundColor = new Color(0.3f, 0.85f, 0.3f);
        if (GUILayout.Button("⚡ Generate Maze + Map", GUILayout.Height(35)))
        {
            gen.GenerateMaze();
            previewTexture = gen.MapTexture;
            EditorUtility.SetDirty(gen.gameObject);
        }

        GUI.backgroundColor = new Color(0.3f, 0.6f, 0.9f);
        if (GUILayout.Button("🗺 Regenerate Map Only", GUILayout.Height(28)))
        {
            gen.GenerateMapOnly();
            previewTexture = gen.MapTexture;
            EditorUtility.SetDirty(gen.gameObject);
        }

        GUI.backgroundColor = new Color(0.9f, 0.35f, 0.35f);
        if (GUILayout.Button("✕ Clear", GUILayout.Height(28)))
        {
            gen.ClearMaze();
            previewTexture = null;
            EditorUtility.SetDirty(gen.gameObject);
        }

        GUI.backgroundColor = Color.white;

        // Превью карты
        if (previewTexture == null)
            previewTexture = gen.MapTexture;

        if (previewTexture != null)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Map Preview", EditorStyles.boldLabel);

            float maxSize = EditorGUIUtility.currentViewWidth - 40;
            float size = Mathf.Min(maxSize, 400);

            Rect rect = GUILayoutUtility.GetRect(size, size);
            EditorGUI.DrawPreviewTexture(rect, previewTexture);

            EditorGUILayout.Space(5);
            if (GUILayout.Button("Save Map as PNG"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Save Maze Map", Application.dataPath, "MazeMap", "png");
                if (!string.IsNullOrEmpty(path))
                {
                    System.IO.File.WriteAllBytes(path, previewTexture.EncodeToPNG());
                    Debug.Log($"Map saved to: {path}");
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
#endif
}