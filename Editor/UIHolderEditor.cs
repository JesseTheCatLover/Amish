using UnityEditor;
using UnityEngine;

namespace JDialogue_System
{
    [CustomEditor(typeof(UIHolder))]
    public class UIHolderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            UIHolder holder = (UIHolder)target;

            // Draw basic config fields
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogueAssetMapping"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialoguePanelMapping"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogueBox"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterNameBox"));

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Panel References", EditorStyles.boldLabel);

            if (holder.dialoguePanelMapping)
            {
                var mapping = holder.dialoguePanelMapping;

                // Sync panelReferences list size with mapping.panels
                var panelRefsProp = serializedObject.FindProperty("panelReferences");
                while (panelRefsProp.arraySize < mapping.panels.Count)
                    panelRefsProp.InsertArrayElementAtIndex(panelRefsProp.arraySize);
                while (panelRefsProp.arraySize > mapping.panels.Count)
                    panelRefsProp.DeleteArrayElementAtIndex(panelRefsProp.arraySize - 1);

                for (int i = 0; i < mapping.panels.Count; i++)
                {
                    var panelEntry = mapping.panels[i];
                    var panelRefProp = panelRefsProp.GetArrayElementAtIndex(i);

                    var panelNameProp = panelRefProp.FindPropertyRelative("panelName");
                    var mainPanelProp = panelRefProp.FindPropertyRelative("mainPanelObject");
                    var companionProp = panelRefProp.FindPropertyRelative("companionPanelObject");

                    // Force sync the panel name
                    panelNameProp.stringValue = panelEntry.panelName;

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.LabelField($"Panel: {panelEntry.panelName}", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(mainPanelProp, new GUIContent("Main Panel Object"));
                    if (panelEntry.hasCompanion)
                    {
                        EditorGUILayout.PropertyField(companionProp, new GUIContent("Companion Panel Object"));
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Assign a Dialogue Panel Mapping to configure panel references.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
