using System.Collections.Generic;
using UnityEngine;

namespace JDialogue_System
{
    [CreateAssetMenu(fileName = "JDialogueAssetMapping", menuName = "JDialogue System/Asset Mapping")]
    public class JDialogueAssetMapping : ScriptableObject
    {
        [System.Serializable]
        public class AnonymousCharacter
        {
            public string shownAs; // The name shown for anonymous characters
        }
        
        [System.Serializable]
        public class CharacterMapping
        {
            public string characterNameKey; // Character's name in the dialogue script
            public string shownAs; // The name shown in the dialogue box
            public List<ExpressionMapping> faceExpressions;
            public List<HandGestureMapping> leftHandGestures;
            public List<HandGestureMapping> rightHandGestures;
        }
    
        [System.Serializable]
        public class ExpressionMapping
        {
            public string expressionName;  // Keyword from .jdialogue
            public Sprite sprite;          // Corresponding sprite asset
        }

        [System.Serializable]
        public class HandGestureMapping
        {
            public string gestureName;  // Keyword for lefthand/righthand gestures
            public Sprite sprite;       // Corresponding sprite asset
        }

        public List<CharacterMapping> characters; // List of all character mappings
        public AnonymousCharacter anonymousCharacter;

        private Dictionary<string, string> _displayNameDict;
        private Dictionary<string, Dictionary<string, Sprite>> _faceDict;
        private Dictionary<string, Dictionary<string, Sprite>> _leftHandDict;
        private Dictionary<string, Dictionary<string, Sprite>> _rightHandDict;

        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (characters == null)
            {
                Debug.LogError($"{name}: Character list is null! Check if you have mapped the assets in the inspector successfully");
                return;
            }
        
            _displayNameDict = new Dictionary<string, string>();
            _faceDict = new Dictionary<string, Dictionary<string, Sprite>>();
            _leftHandDict = new Dictionary<string, Dictionary<string, Sprite>>();
            _rightHandDict = new Dictionary<string, Dictionary<string, Sprite>>();

            foreach (var character in characters)
            {
                // Store display name mapping
                _displayNameDict[character.characterNameKey] = character.shownAs;

                // Create per-character dictionaries
                _faceDict[character.characterNameKey] = new Dictionary<string, Sprite>();
                _leftHandDict[character.characterNameKey] = new Dictionary<string, Sprite>();
                _rightHandDict[character.characterNameKey] = new Dictionary<string, Sprite>();

                // Populate face expressions
                foreach (var item in character.faceExpressions)
                    _faceDict[character.characterNameKey][item.expressionName] = item.sprite;

                // Populate left-hand gestures
                foreach (var item in character.leftHandGestures)
                    _leftHandDict[character.characterNameKey][item.gestureName] = item.sprite;

                // Populate right-hand gestures
                foreach (var item in character.rightHandGestures)
                    _rightHandDict[character.characterNameKey][item.gestureName] = item.sprite;
            }
        }

        public string GetCharacterDisplayName(string character)
        {
            return _displayNameDict.TryGetValue(character, out string displayName) ? displayName : character;
        }

        public Sprite GetFaceSprite(string character, string expression)
        {
            return _faceDict.TryGetValue(character, out var expressions) &&
                   expressions.TryGetValue(expression, out Sprite sprite)
                ? sprite
                : null;
        }

        public Sprite GetLeftHandSprite(string character, string gesture)
        {
            return _leftHandDict.TryGetValue(character, out var gestures) &&
                   gestures.TryGetValue(gesture, out Sprite sprite)
                ? sprite
                : null;
        }

        public Sprite GetRightHandSprite(string character, string gesture)
        {
            return _rightHandDict.TryGetValue(character, out var gestures) &&
                   gestures.TryGetValue(gesture, out Sprite sprite)
                ? sprite
                : null;
        }
    }
}