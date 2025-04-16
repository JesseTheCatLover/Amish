using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JDialogue_System
{
    [CreateAssetMenu(fileName = "JDialoguePanelMapping", menuName = "JDialogue System/Panel Mapping")]
    public class JDialoguePanelMapping : ScriptableObject
    {
        [System.Serializable]
        public class PanelEntry
        {
            [Header("Panel Settings")] [Tooltip("Friendly name for this panel (e.g., 'Left Panel')")]
            public string panelName;

            [Tooltip("List of characters associated with this panel (e.g., ['amish', 'kayra'])")]
            public List<string> assignedCharacters;

            [Tooltip("Whether this panel uses a companion panel")]
            public bool hasCompanion;
        }
        
        [Tooltip("List of all the panel configurations for characters and their panels")]
        public List<PanelEntry> panels = new List<PanelEntry>();

        private Dictionary<string, PanelEntry> _characterPanelDict;

        private void OnEnable()
        {
            BuildLookup();
        }

        public void BuildLookup()
        {
            _characterPanelDict = new Dictionary<string, PanelEntry>();

            foreach (var panel in panels)
            {
                foreach (var character in panel.assignedCharacters)
                {
                    if (!_characterPanelDict.ContainsKey(character))
                    {
                        _characterPanelDict.Add(character, panel);
                    }
                    else
                    {
                        Debug.LogWarning($"Character '{character}' is already mapped to a panel. Duplicate mapping in '{name}'.");
                    }
                }
            }
        }

        public PanelEntry GetPanelForCharacter(string characterKey)
        {
            if (_characterPanelDict == null || _characterPanelDict.Count == 0)
                BuildLookup();

            return _characterPanelDict.TryGetValue(characterKey, out var panel) ? panel : null;
        }
    }
}
