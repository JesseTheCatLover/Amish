using System;
using System.Collections.Generic;
using UnityEngine;

namespace JDialogue_System
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private List<TextAsset> jDialogueFiles; // List of .jdialogue files
        private UIHolder _uiHolder; // Reference to UI Manager
        private List<DialogueListEntry> _dialogueEntries;
        private int _currentIndex = 0;
        public static event Action OnNextDialogueTriggered; // Define an event for dialogue progression
    
        // Set the language you want
        private Languages _selectedLanguage = Languages.English; // TODO: Make it select from the settings
    
        private void Awake()
        {
            _uiHolder = GameObject.FindWithTag("DialogueUI")?.GetComponent<UIHolder>();
            if (_uiHolder == null)
            {
                Debug.LogError("DialogueUI not found! Make sure you have a UI GameObject with the 'DialogueUI' tag, and DialogueUI script assigned to it.");
            }
        }
    
        private void OnEnable()
        {
            OnNextDialogueTriggered += ProceedToNextDialogue;
        }

        private void OnDisable()
        {
            OnNextDialogueTriggered -= ProceedToNextDialogue;
        }
        private void Start() // TODO: Define a trigger for start of the dialogue
        {
            ParseAllDialogues();
            StartDialogue();
        }

        private void ParseAllDialogues()
        {
            _dialogueEntries = new List<DialogueListEntry>();

            foreach (TextAsset dialogueFile in jDialogueFiles)
            {
                if (dialogueFile == null) continue;
                List<DialogueListEntry> parsedEntries = JDialogueParser.ParseDialogue(dialogueFile, _selectedLanguage);
                _dialogueEntries.AddRange(parsedEntries);
            }
        }

        public void StartDialogue()
        {
            if (_dialogueEntries.Count > 0)
            {
                ShowDialogue(_dialogueEntries[_currentIndex]);
            }
        }

        private void ShowDialogue(DialogueListEntry entry)
        {
            _uiHolder?.UpdateDialogue(entry);
        }

        public void ProceedToNextDialogue()
        {
            _currentIndex++;
            if (_currentIndex < _dialogueEntries.Count)
            {
                ShowDialogue(_dialogueEntries[_currentIndex]);
            }
            else
            {
                EndDialogue();
            }
        }

        private void EndDialogue()
        {
            //DialogueUI?.HideUI();
            Debug.Log("Dialogue finished!");
        }
    
        public static void TriggerNextDialogue()
        {
            OnNextDialogueTriggered?.Invoke();
        }
    }
}