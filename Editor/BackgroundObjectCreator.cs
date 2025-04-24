using Background_System;
using UnityEditor;
using UnityEngine;

public class BackgroundObjectCreator
{
    [MenuItem("GameObject/JObject/Background Object", false, 10)]
    public static void CreateBackgroundObject(MenuCommand menuCommand)
    {
        // Create the root object
        GameObject backgroundGameObject = new GameObject("Background");
        backgroundGameObject.AddComponent<SpriteRenderer>();
        backgroundGameObject.AddComponent<BackgroundManager>();
        backgroundGameObject.AddComponent<AutoFitSpriteToCamera>();

        // Create ItemGroups child
        GameObject itemGroups = new GameObject("ItemGroups");
        itemGroups.transform.SetParent(backgroundGameObject.transform);
        
        // Create LightGroups child
        GameObject lightGroups = new GameObject("LightGroups");
        lightGroups.transform.SetParent(backgroundGameObject.transform);

        // Optional: select it after creation
        GameObjectUtility.SetParentAndAlign(backgroundGameObject, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(backgroundGameObject, "Create Background Object");
        Selection.activeObject = backgroundGameObject;
    }
}