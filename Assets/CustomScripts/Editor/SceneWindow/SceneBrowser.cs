using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class SceneBrowser : EditorWindow
{
    [MenuItem("Tools/SceneBrowser")]
    public static void ShowExample()
    {
        SceneBrowser wnd = GetWindow<SceneBrowser>();
        wnd.titleContent = new GUIContent("SceneBrowser");
    }

    private Scene activeScene;
    private VisualElement root;
    private ScrollView scrollView;
    private EditorBuildSettingsScene[] scenes;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/CustomScripts/Editor/SceneWindow/SceneBrowser.uss");
        root.styleSheets.Add(styleSheet);

        scrollView = new ScrollView();
        scrollView.AddToClassList("scrollContainer");
        root.Add(scrollView);
        BuildSceneRowElements();

        EditorBuildSettings.sceneListChanged += BuildSceneRowElements;
    }

    private void BuildSceneRowElements()
    {
        scrollView.Clear();

        activeScene = EditorSceneManager.GetActiveScene();
        scenes = EditorBuildSettings.scenes;
        int length = scenes.Length;
        for (int i = 0; i < length; i++)
        {
            EditorBuildSettingsScene scene = scenes[i];
            bool isActive = activeScene.path == scene.path;
            VisualElement row = new VisualElement();
            row.AddToClassList("rowContainer");
            string sceneName = Path.GetFileNameWithoutExtension(scene.path);
            Toggle toggle = new Toggle(sceneName);
            toggle.RegisterValueChangedCallback((state) => ChangeSceneSetting(scene.path, state.newValue));
            toggle.AddToClassList("sceneToggle");

            if (isActive) toggle.AddToClassList("toggleSelected");
            else toggle.RemoveFromClassList("toggleSelected");

            row.Add(toggle);
            toggle.value = scene.enabled;
            SetButtonToOpenScene(scene, row);
            SetButtonOpeSelectScene(scene, row);

            scrollView.Add(row);
        }
    }

    private void ChangeSceneSetting(string path, bool newValue)
    {
        int index = Array.FindIndex(scenes, x => x.path == path);

        if (index == -1)
            return;

        EditorBuildSettingsScene scene = scenes[index];
        scene.enabled = newValue;
        scenes[index] = scene;
        EditorBuildSettings.scenes = scenes;
    }

    private void SetButtonOpeSelectScene(EditorBuildSettingsScene item, VisualElement row)
    {
        Button btnSelect = new() { tooltip = "Select The Scene" };
        btnSelect.AddToClassList("button");
        btnSelect.AddToClassList("btnSelect");
        btnSelect.clicked += () => SelectScene(item.path);
        row.Add(btnSelect);
    }

    private void SetButtonToOpenScene(EditorBuildSettingsScene item, VisualElement row)
    {
        Button btnOpen = new() { tooltip = "Open The Scene" };
        btnOpen.AddToClassList("button");
        btnOpen.AddToClassList("btnOpen");
        btnOpen.clicked += () => OpenScene(item.path);
        row.Add(btnOpen);
    }

    private void SelectScene(string path)
    {
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(path));
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
    }

    private void OpenScene(string path)
    {

        if (EditorUtility.DisplayDialog("Wannt to Save?", "Do you want to save current scene?", "Yes", "No"))
        {
            Debug.Log($"Saving active scene before opening new scene.");
            EditorSceneManager.SaveScene(activeScene);
        }

        EditorSceneManager.OpenScene(path);
        BuildSceneRowElements();
    }
}