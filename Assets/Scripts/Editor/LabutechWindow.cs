using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TelephoneBooth.Editor
{
  public class LabutechWindow : EditorWindow
  {
    private void OnGUI()
    {
      EditorSceneManager.playModeStartScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"),
        EditorSceneManager.playModeStartScene, typeof(SceneAsset), false);

      var scenePath = "Assets/Scenes/Loading.unity";
      if (GUILayout.Button("Set start Scene: " + scenePath)) SetPlayModeStartScene(scenePath);

      if (GUILayout.Button("Set NONE Scene")) SetPlayModeNoneScene();

      if (GUILayout.Button("ClearPlayerPrefs")) ClearPlayerPrefs();
    }

    private void SetPlayModeStartScene(string scenePath)
    {
      SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
      if (myWantedStartScene != null) EditorSceneManager.playModeStartScene = myWantedStartScene;
      else Debug.Log("Could notfind Scene " + scenePath);
    }

    private void SetPlayModeNoneScene() => EditorSceneManager.playModeStartScene = null;

    private void ClearPlayerPrefs()
    {
      PlayerPrefs.DeleteAll();
      Debug.Log("PlayerPrefs cleared!");
    }

    [MenuItem("Labutech/Start From Scene")]
    static void Open() => GetWindow<LabutechWindow>();
  }
}