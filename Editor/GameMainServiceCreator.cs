using Core;
using Save_System;
using UnityEditor;
using UnityEngine;

public static class GameMainServiceCreator
{
    [MenuItem("GameObject/JObject/Game Main Service", false, 10)]
    public static void CreateGameMainService(MenuCommand menuCommand)
    {
        // Root object
        GameObject serviceRoot = new GameObject("GameMainService");

        serviceRoot.AddComponent<GameMainService>();
        serviceRoot.AddComponent<GameSaveService>();

        // Optional: Align and register with Unity's undo system
        GameObjectUtility.SetParentAndAlign(serviceRoot, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(serviceRoot, "Create Game Main Service Object");
        Selection.activeObject = serviceRoot;
    }
}