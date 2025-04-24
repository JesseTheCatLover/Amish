using System.Collections.Generic;
using UnityEngine;

namespace JDialogue_System
{
    public class JDialogueRunner : MonoBehaviour
    {
        [SerializeField] private List<TextAsset> jDialogueFiles; // List of .jdialogue files
        private UIHolder _uiHolder; // Reference to UIHolder
        private List<DialogueListEntry> _dialogueEntries;
        private int _currentIndex = 0;
        public static JDialogueRunner ActiveRunner { get; private set; }
    
        // Set the language you want
        private Languages _selectedLanguage = Languages.English; // TODO: Make it select from the settings (should recieve it from the UIHolder or smth)
    
        private void Awake()
        {
            _uiHolder = GameObject.FindWithTag("DialogueUI")?.GetComponent<UIHolder>();
            if (_uiHolder == null)
            {
                Debug.LogError("DialogueUI not found! Make sure you have a UI GameObject with the 'DialogueUI' tag, and DialogueUI script assigned to it.");
            }
        }
    
        private void Start()
        {
            ParseAllDialogues();
            SetAsActiveRunnerAndStart(); // TODO : called in the Start() for now
        }
        
        public void SetAsActiveRunnerAndStart()
        {
            if (ActiveRunner != null && ActiveRunner != this)
            {
                Debug.LogWarning($"Overriding previously active dialogue runner: {gameObject.name} - {ActiveRunner.name}");
            }

            ActiveRunner = this;
            _currentIndex = 0;
            StartDialogue();
        }
        
        public static void TriggerNextDialogue()
        {
            if (ActiveRunner == null)
            {
                Debug.LogError("No active dialogue runner found to proceed to the next dialogue! make sure ActiveRunner is set.");
                return;
            }
            ActiveRunner.ProceedToNextDialogue();
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

        private void StartDialogue()
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

        private void ProceedToNextDialogue()
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
            Debug.Log($"Dialogue finished for {gameObject.name}");
            if (ActiveRunner == this)
                ActiveRunner = null;
        }
    }
}