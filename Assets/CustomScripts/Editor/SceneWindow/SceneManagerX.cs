using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Text;

public class SceneManagerX : EditorWindow
{
    public List<ScenesDetails> scenelist;
    public Vector2 scrollPosition;
    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/Scene Manager")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SceneManagerX window = (SceneManagerX)EditorWindow.GetWindow(typeof(SceneManagerX));
        window.Show();

    }

    private const string activeSign = " ✔";

    void OnGUI()
    {
        scenelist = ReadDetails();

        Scene activeScene = EditorSceneManager.GetActiveScene();
        GUILayout.Label(Application.productName, EditorStyles.boldLabel);

        //scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MinWidth(170), GUILayout.Height(150));
        GUILayout.BeginArea(new Rect(10, 30, 200, 150));
        foreach (ScenesDetails item in scenelist)
        {
            StringBuilder name = new StringBuilder();
            name.Append(item.Name);
            name.Append(item.Path.Equals(activeScene.path) ? activeSign : "");

            GUILayout.BeginHorizontal();
            GUILayout.Toggle(item.enabled, name.ToString(), GUILayout.MaxWidth(200));
            //Debug.Log (item.Name);
            if (GUILayout.Button("Open"))
            {
                OpenScene(item, activeScene);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    private static void OpenScene(ScenesDetails item, Scene activeScene)
    {
        if (activeScene.path.Equals(item.Path))
            return;

        Debug.Log("Open Scene" + item.Name);
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorSceneManager.OpenScene(item.Path);
        }
    }

    private static List<string> ReadNames()
    {
        List<string> temp = new List<string>();
        foreach (EditorBuildSettingsScene S in EditorBuildSettings.scenes)
        {
            if (S.enabled)
            {
                string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
                name = name.Substring(0, name.Length - 6);
                temp.Add(name);
                //Debug.Log (name);
            }
        }
        return temp;//.ToArray();
    }
    private static List<ScenesDetails> ReadDetails()
    {
        List<ScenesDetails> temp = new List<ScenesDetails>();

        foreach (EditorBuildSettingsScene S in EditorBuildSettings.scenes)
        {
            string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
            name = name.Substring(0, name.Length - 6);
            ScenesDetails s = new ScenesDetails();
            s.Name = name;
            s.Path = S.path.ToString();
            temp.Add(s);
            s.enabled = S.enabled;
        }
        return temp;//.ToArray();
    }

}
public class ScenesDetails
{
    public bool enabled;
    public string Name;
    public string Path;
}