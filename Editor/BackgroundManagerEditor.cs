#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Background_System;

[CustomEditor(typeof(BackgroundManager))]
public class BackgroundManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BackgroundManager manager = (BackgroundManager)target;

        if (GUILayout.Button("Load Default Background"))
        {
            manager.LoadDefaultInEditor();
        }
    }
}
#endif