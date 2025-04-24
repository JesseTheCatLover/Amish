using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace JDialogue_System
{
    public class UIHolder : MonoBehaviour
    {
        [Header("Mapping Configuration")]
        [SerializeField] public JDialogueAssetMapping dialogueAssetMapping;
        [SerializeField] public JDialoguePanelMapping dialoguePanelMapping;
        
        [Header("Dialogue Elements")]
        [SerializeField] private TextMeshProUGUI dialogueBox;
        [SerializeField] private TextMeshProUGUI characterNameBox;
        
        [Header("Dynamic Panels")]
        [HideInInspector] public List<UIPanelReference> panelReferences = new List<UIPanelReference>();
    
        [System.Serializable]
        public class UIPanelReference
        {
            public string panelName;
            public GameObject mainPanelObject;
            public GameObject companionPanelObject;
        }
        
        private Dictionary<string, GameObject> dictMainPanelObjects = new();
        private Dictionary<string, GameObject> dictCompanionPanelObjects = new();

        private PortraitListEntry lastCompanionRendered;
            
        private void Awake()
        {
            RefreshPanelReferences();
        }

        private void RefreshPanelReferences()
        {
            dictMainPanelObjects.Clear();
            dictCompanionPanelObjects.Clear();

            foreach (var pair in panelReferences)
            {
                if (!string.IsNullOrWhiteSpace(pair.panelName))
                    dictMainPanelObjects[pair.panelName] = pair.mainPanelObject;

                if (pair.companionPanelObject != null)
                    dictCompanionPanelObjects[pair.panelName] = pair.companionPanelObject;
            }
        }

        private GameObject GetMainPanelForCharacter(string character)
        {
            foreach (var entry in dialoguePanelMapping.panels)
            {
                if (entry.assignedCharacters.Contains(character))
                    return dictMainPanelObjects.TryGetValue(entry.panelName, out var obj) ? obj : null;
            }

            return null;
        }

        private GameObject GetCompanionPanelForCharacter(string character)
        {
            foreach (var entry in dialoguePanelMapping.panels)
            {
                if (entry.assignedCharacters.Contains(character) && entry.hasCompanion)
                    return dictCompanionPanelObjects.TryGetValue(entry.panelName, out var obj) ? obj : null;
            }

            return null;
        }
        
        public void UpdateDialogue(DialogueListEntry entry)
        {
            if (dialogueAssetMapping == null)
            {
                Debug.LogError($"In object: ({name}): DialogueAssetMapping not set for DialogueUI");
                return;
            }
            dialogueBox.text = entry.Dialogue;
            if (entry.CompanionCharacter == null) // Single-character dialogue
            {
                characterNameBox.text = entry.MainCharacter.IsAnonymous ? dialogueAssetMapping.anonymousCharacter.shownAs : 
                    dialogueAssetMapping.GetCharacterDisplayName(entry.MainCharacter.CharacterKey);
                ChangeSprites(entry.MainCharacter);
            }
            else // Dual-character dialogue
            {
                string mainDisplayName = entry.MainCharacter.IsAnonymous ? dialogueAssetMapping.anonymousCharacter.shownAs :
                    dialogueAssetMapping.GetCharacterDisplayName(entry.MainCharacter.CharacterKey);
                string companionDisplayName = entry.CompanionCharacter.IsAnonymous ? dialogueAssetMapping.anonymousCharacter.shownAs :
                    dialogueAssetMapping.GetCharacterDisplayName(entry.CompanionCharacter.CharacterKey);
                
                characterNameBox.text = mainDisplayName + " & " + companionDisplayName;
                ChangeSprites(entry.MainCharacter, entry.CompanionCharacter);
            }
        }
    
        public void HideUI()
        {
            gameObject.SetActive(false);
        }

        private void ChangeSprites([CanBeNull] PortraitListEntry mainEntry, [CanBeNull] PortraitListEntry compEntry = null)
        {
            if(dialogueAssetMapping.IsPortraitAvailableForCharacter(mainEntry?.CharacterKey))
                SetCharacterSprites(mainEntry, GetMainPanelForCharacter(mainEntry?.CharacterKey));

            if (dialogueAssetMapping.IsPortraitAvailableForCharacter(compEntry?.CharacterKey))
            {
                if (compEntry != null) // has companion
                {
                    SetCharacterSprites(compEntry, GetCompanionPanelForCharacter(compEntry.CharacterKey));
                    lastCompanionRendered = compEntry; // A reference to the last companion
                }
                else
                {
                    if (lastCompanionRendered != null) // a last companion exists
                    {
                        SetCharacterSprites(null, GetCompanionPanelForCharacter(lastCompanionRendered.CharacterKey));
                    }
                }
            }
        }
        
        private void SetCharacterSprites([CanBeNull] PortraitListEntry entry, GameObject panel)
        {
            if (panel == null)
            {
                Debug.LogError($"{this.GetType().Name}: Panel is not set in for character key ({entry?.CharacterKey})");
                return;
            }

            if (dialogueAssetMapping.IsPortraitAvailableForCharacter(entry?.CharacterKey))
            {
                PortraitRenderer renderer = panel.GetComponentInChildren<PortraitRenderer>();
                if (renderer)
                    renderer.ApplyPortrait(entry, dialogueAssetMapping);
                else
                    Debug.LogWarning($"[{this.GetType().Name}] No PortraitRenderer found in panel {panel.name}");
            }
        }
    }
}
