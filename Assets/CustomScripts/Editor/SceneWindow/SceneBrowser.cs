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

        Scene activeScene = EditorSceneManager.GetActiveScene();
        scenes = EditorBuildSettings.scenes;
        int length = scenes.Length;
        for(int i = 0; i < length; i++)
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
        Debug.Log($"Received toggle value changed event: value = {newValue} @ path: {path}");
        int index = Array.FindIndex(scenes, x => x.path == path);

        Debug.Log($"found the scene at the index of {index}");
        if(index == -1)
            return;

        EditorBuildSettingsScene scene = scenes[index];
        scene.enabled = newValue;
        scenes[index] = scene;
        EditorBuildSettings.scenes = scenes;
    }

    private void SetButtonOpeSelectScene(EditorBuildSettingsScene item, VisualElement row)
    {
        Button btnSelect = new Button();
        btnSelect.tooltip = "Select The Scene";
        btnSelect.AddToClassList("button");
        btnSelect.AddToClassList("btnSelect");
        btnSelect.clicked += () => SelectScene(item.path);
        row.Add(btnSelect);
    }

    private void SetButtonToOpenScene(EditorBuildSettingsScene item, VisualElement row)
    {
        Button btnOpen = new Button();
        btnOpen.tooltip = "Open The Scene";
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
        EditorSceneManager.OpenScene(path);
    }
}