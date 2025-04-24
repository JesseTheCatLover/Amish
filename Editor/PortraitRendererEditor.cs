using JDialogue_System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PortraitRenderer))]
public class PortraitRendererEditor : UnityEditor.Editor
{
    private SerializedProperty previewCharacterKey;

    private void OnEnable()
    {
        previewCharacterKey = serializedObject.FindProperty("previewCharacterKey");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        // Add a gap for visual separation
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Overlay Preview (Editor Only)", EditorStyles.boldLabel);

        var myTarget = (PortraitRenderer)target;

        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Editor preview disabled in Play Mode.", MessageType.Info);
            return;
        }

        var uiHolder = FindFirstObjectByType<UIHolder>();
        if (!uiHolder || !uiHolder.dialogueAssetMapping)
        {
            EditorGUILayout.HelpBox("UIHolder or DialogueAssetMapping not found.", MessageType.Error);
            return;
        }

        var mapping = uiHolder.dialogueAssetMapping;
        var keys = mapping.GetAllCharacterKeys();

        if (keys == null || keys.Length == 0)
        {
            EditorGUILayout.HelpBox("No characters found in DialogueAssetMapping.", MessageType.Warning);
            return;
        }

        int currentIndex = System.Array.IndexOf(keys, previewCharacterKey.stringValue);
        if (currentIndex < 0) currentIndex = 0;

        int newIndex = EditorGUILayout.Popup("Preview Character", currentIndex, keys);
        if (newIndex != currentIndex)
        {
            previewCharacterKey.stringValue = keys[newIndex];
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Load Preview Character In Editor"))
        {
            myTarget.PreviewCharacterInEditor();
        }
    }
}