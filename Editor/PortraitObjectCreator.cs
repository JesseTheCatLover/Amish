using JDialogue_System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PortraitObjectCreator
{
    [MenuItem("GameObject/JObject/Portrait Object", false, 11)]
    public static void CreatePortraitObject(MenuCommand menuCommand)
    {
        // Root GameObject
        GameObject portraitGO = new GameObject("Portrait");
        portraitGO.AddComponent<RectTransform>();
        portraitGO.AddComponent<PortraitRenderer>();

        // Create children with Image and tag
        CreatePart("Body", "Body", portraitGO.transform);
        CreatePart("Head", "Head", portraitGO.transform);
        CreatePart("LeftHand", "LeftHand", portraitGO.transform);
        CreatePart("RightHand", "RightHand", portraitGO.transform);

        // Parenting and undo support
        GameObjectUtility.SetParentAndAlign(portraitGO, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(portraitGO, "Create Portrait Object");
        Selection.activeObject = portraitGO;
    }

    private static void CreatePart(string name, string tag, Transform parent)
    {
        GameObject part = new GameObject(name);
        part.transform.SetParent(parent);
        part.AddComponent<RectTransform>();
        part.AddComponent<Image>();
        part.tag = tag;
    }
}