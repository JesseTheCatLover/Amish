using Core;
using JDialogue_System;
using UnityEngine;

namespace Save_System
{
    public class GameSaveService : MonoBehaviour, IGameService
    {
        [HideInInspector] private CharacterSkinTrackerAsset SkinTracker;

        public bool InitializeService()
        {
            var dialogueUI = GameObject.FindWithTag("DialogueUI")?.GetComponent<UIHolder>();
    
            if (dialogueUI == null)
            {
                Debug.LogError($"{name}: Dialogue UI not found! Cannot initialize GameSaveService. Make sure to assign (DialogueUI) tag to the UI");
                return false;  // Early exit if initialization can't proceed.
            }

            GameSaveManager.CharacterSkinTrackerReference = dialogueUI.dialogueAssetMapping?.skinTrackerAsset;
            if (GameSaveManager.CharacterSkinTrackerReference == null)
            {
                Debug.LogError($"{name}: Skin Tracker reference is null. Cannot initialize save system. Make sure to provide Mappings for the UIHolder");
                return false;
            }

            return true;
        }
    }
}