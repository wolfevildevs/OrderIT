using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelDataSO targetLevelData;
    private Vector2 scrollPosition;

    // This creates a custom tab in Unity's upper top menu bar!
    [MenuItem("Runner Tools/Level Designer Window")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Designer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Custom Level Designer Matrix", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 1. Select which level asset you want to design/modify
        targetLevelData = (LevelDataSO)EditorGUILayout.ObjectField("Select Level Data:", targetLevelData, typeof(LevelDataSO), false);
        
        EditorGUILayout.Space();
        if (targetLevelData == null)
        {
            EditorGUILayout.HelpBox("Please assign a LevelDataSO asset to start designing the layout manually.", MessageType.Info);
            return;
        }

        GUILayout.Label($"Editing Layout for: {targetLevelData.name}", EditorStyles.miniBoldLabel);
        EditorGUILayout.Space();

        // 2. Display the list of chunks with complete control (Add, Remove, Re-order)
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
        
        SerializedObject serializedLevel = new SerializedObject(targetLevelData);
        SerializedProperty chunksProperty = serializedLevel.FindProperty("chunksOrder");

        EditorGUILayout.PropertyField(chunksProperty, true);
        
        serializedLevel.ApplyModifiedProperties();
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();

        // 3. Action Buttons
        if (GUILayout.Button("Clear Entire Layout", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to wipe the level layout clean?", "Yes", "No"))
            {
                targetLevelData.chunksOrder.Clear();
                EditorUtility.SetDirty(targetLevelData);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Changes are saved automatically to your ScriptableObject file. You can drag and drop prefabs directly into the slots above.", MessageType.None);
    }
}