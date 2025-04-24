using System.Collections.Generic;
using System.IO;
using JDialogue_System;
using UnityEngine;

namespace Save_System
{
    public static class GameSaveManager
    {
        private static GameSaveData _dataToSave = new GameSaveData();
        private static string SavePath => Application.persistentDataPath + "/game_save_data.json";

        public static CharacterSkinTrackerAsset CharacterSkinTrackerReference;
        
        public static void SaveSkinState(CharacterSkinTrackerAsset skinTracker)
        {
            _dataToSave.savedSkins = new List<CharacterSkinTrackerAsset.CharacterSkinData>(skinTracker.skinList);
            Debug.Log("Skin state saved.");
        }

        public static void LoadSkinState(CharacterSkinTrackerAsset skinTracker)
        {
            if (_dataToSave.savedSkins != null && _dataToSave.savedSkins.Count > 0)
            {
                skinTracker.skinList = new List<CharacterSkinTrackerAsset.CharacterSkinData>(_dataToSave.savedSkins);
                skinTracker.OnAfterDeserialize(); // Manually sync the dictionary
                Debug.Log("Skin state loaded.");
            }
            else
            {
                Debug.Log("No skin state found.");
            }
        }

        public static void SaveGame()
        {
            
            var json = JsonUtility.ToJson(_dataToSave, true);
            
            // Save to the persistent data path
            File.WriteAllText(SavePath, json);
        }

        public static void LoadGame()
        {
            if (File.Exists(SavePath))
            {
                var json = File.ReadAllText(SavePath);
                JsonUtility.FromJsonOverwrite(json, _dataToSave);
                Debug.Log("Save data loaded.");
                
                LoadSkinState(CharacterSkinTrackerReference);
            }
            else
            {
                Debug.LogWarning("No saved file found.");
            }
        }
    }
}