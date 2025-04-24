using System.Collections.Generic;
using Save_System;
using UnityEngine;

namespace JDialogue_System
{
    [CreateAssetMenu(fileName = "CharacterSkinTrackerAsset", menuName = "JDialogue System/Skin Tracker Asset")]
    public class CharacterSkinTrackerAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Define a class to store the character and their corresponding skin.
        /// </summary>
        [System.Serializable]
        public class CharacterSkinData
        {
            /// <summary>
            /// The unique key identifying the character.
            /// </summary>
            public string characterKey;

            /// <summary>
            /// The unique key identifying the character's skin.
            /// </summary>
            public string skinKey;
        }

        [Header("List of current skins applied for each character")]
        public List<CharacterSkinData> skinList = new();

        // Runtime dictionary for fast lookups
        private Dictionary<string, string> _skinLookupMap = new();

        /// <summary>
        /// Gets the current skin for the specified character.
        /// </summary>
        /// <param name="characterKey">The key identifying the character.</param>
        /// <returns>The skin key assigned to the character, or null if no skin is assigned.</returns>
        public string GetSkin(string characterKey)
        {
            return _skinLookupMap.TryGetValue(characterKey, out var skin) ? skin : null;
        }

        /// <summary>
        /// Sets the skin for the specified character.
        /// </summary>
        /// <param name="characterKey">The key identifying the character.</param>
        /// <param name="skinKey">The key identifying the skin to apply.</param>
        public void SetSkin(string characterKey, string skinKey)
        {
            // If the skin for the character is already set to the same key, no need to update
            if (_skinLookupMap.ContainsKey(characterKey) && _skinLookupMap[characterKey] == skinKey)
            {
                return;
            }

            // Set the skin in the dictionary
            _skinLookupMap[characterKey] = skinKey;

            // Update the serialized list (e.g., for saving/loading)
            SyncSerializedListWithDictionary();

            // Call the save manager to persist the change
            GameSaveManager.SaveSkinState(this);
        }

        /// <summary>
        /// Syncs the serialized list to the runtime dictionary for saving purposes.
        /// </summary>
        private void SyncSerializedListWithDictionary()
        {
            // Don't clear the list, only add or update items
            foreach (var pair in _skinLookupMap)
            {
                // Check if this key exists, if not, add it
                if (!skinList.Exists(data => data.characterKey == pair.Key))
                {
                    skinList.Add(new CharacterSkinData
                    {
                        characterKey = pair.Key,
                        skinKey = pair.Value
                    });
                }
                else
                {
                    // If it exists, update the existing entry
                    var existingEntry = skinList.Find(data => data.characterKey == pair.Key);
                    existingEntry.skinKey = pair.Value;
                }
            }
        }

        /// <summary>
        /// Syncs the runtime dictionary to the serialized list for lookup purposes.
        /// </summary>
        private void SyncDictionaryWithSerializedList()
        {
            _skinLookupMap.Clear();
            foreach (var entry in skinList)
            {
                _skinLookupMap[entry.characterKey] = entry.skinKey;
            }
        }

        /// <summary>
        /// Called after deserialization to sync the serialized data to the runtime dictionary.
        /// </summary>
        public void OnAfterDeserialize()
        {
            SyncDictionaryWithSerializedList();
        }

        /// <summary>
        /// Called before serialization to sync the runtime dictionary to the serialized list.
        /// </summary>
        public void OnBeforeSerialize()
        {
            SyncSerializedListWithDictionary();
        }
    }
}
