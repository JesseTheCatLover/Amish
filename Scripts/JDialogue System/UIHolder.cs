using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JDialogue_System
{
    public class UIHolder : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private JDialogueAssetMapping dialogueAssetMapping;
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
        
        private Dictionary<string, GameObject> panelObjects = new();
        private Dictionary<string, GameObject> companionPanelObjects = new();
        
        private void Awake()
        {
            RefreshPanelReferences();
        }

        public void RefreshPanelReferences()
        {
            panelObjects.Clear();
            companionPanelObjects.Clear();

            foreach (var pair in panelReferences)
            {
                if (!string.IsNullOrWhiteSpace(pair.panelName))
                    panelObjects[pair.panelName] = pair.mainPanelObject;

                if (pair.companionPanelObject != null)
                    companionPanelObjects[pair.panelName] = pair.companionPanelObject;
            }
        }
        
        public GameObject GetPanelForCharacter(string character)
        {
            foreach (var entry in dialoguePanelMapping.panels)
            {
                if (entry.assignedCharacters.Contains(character))
                    return panelObjects.TryGetValue(entry.panelName, out var obj) ? obj : null;
            }

            return null;
        }

        public GameObject GetCompanionPanelForCharacter(string character)
        {
            foreach (var entry in dialoguePanelMapping.panels)
            {
                if (entry.assignedCharacters.Contains(character) && entry.hasCompanion)
                    return companionPanelObjects.TryGetValue(entry.panelName, out var obj) ? obj : null;
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
            characterNameBox.text = entry.IsAnonymous ? dialogueAssetMapping.anonymousCharacter.shownAs : 
                dialogueAssetMapping.GetCharacterDisplayName(entry.Character);
            ChangeSprites(entry);
        }
    
        public void HideUI()
        {
            gameObject.SetActive(false);
        }
    
        private void ChangeSprites(DialogueListEntry entry)
        { 
            // TODO: make a right and left specifier later.
        
            /*
        // Change Body Image (Tag: "Body")
        Image bodyImage = GameObject.FindWithTag("Body")?.GetComponent<Image>();
        if (bodyImage != null)
            bodyImage.sprite = dialogueAssetMapping.Get; */

            // Change Head Image (Tag: "Head")
            Image headImage = GameObject.FindWithTag("Head")?.GetComponent<Image>();
            if (headImage != null)
                headImage.sprite = dialogueAssetMapping.GetFaceSprite(entry.Character, entry.Face);

            // Change LeftHand Image (Tag: "LeftHand")
            Image leftHandImage = GameObject.FindWithTag("LeftHand")?.GetComponent<Image>();
            if (leftHandImage != null)
                leftHandImage.sprite = dialogueAssetMapping.GetLeftHandSprite(entry.Character, entry.LeftHand);

            // Change RightHand Image (Tag: "RightHand")
            Image rightHandImage = GameObject.FindWithTag("RightHand")?.GetComponent<Image>();
            if (rightHandImage != null)
                rightHandImage.sprite = dialogueAssetMapping.GetRightHandSprite(entry.Character, entry.RightHand);
        }
    }
}
