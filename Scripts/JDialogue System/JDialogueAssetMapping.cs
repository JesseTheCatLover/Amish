using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JDialogue_System
{
    [CreateAssetMenu(fileName = "JDialogueAssetMapping", menuName = "JDialogue System/Asset Mapping")]
    public class JDialogueAssetMapping : ScriptableObject
    {
        [System.Serializable]
        public class AnonymousCharacter
        {
            [Tooltip("The name shown for anonymous characters")]
            public string shownAs;
        }

        [System.Serializable]
        public class CharacterMapping
        {
            [Tooltip("Character keyword used in the dialogue script")]
            public string characterNameKey;

            [Tooltip("The name shown in the dialogue box")]
            public string shownAs;

            [Tooltip("Should the portrait be rendered?")]
            public bool hasNoPortrait = false;

            [Tooltip("List of skins for this character")]
            public List<SkinMapping> skins;
        }

        [System.Serializable]
        public class SkinMapping
        {
            [Tooltip("Unique key for this skin (e.g., 'normal', 'bloody', ...)")]
            public string skinKey;

            [Tooltip("Body sprite for this skin")] public Sprite bodySprite;

            [Tooltip("Body item overlays for this skin (The list starts from the bottom-most layer to the top-most layer)")]
            public List<BodyItemOverlay> bodyItemOverlays;

            [Tooltip("Face expression mappings for this skin")]
            public List<ExpressionMapping> faceExpressions;

            [Tooltip("Left-hand gesture mappings for this skin")]
            public List<HandGestureMapping> leftHandGestures;

            [Tooltip("Right-hand gesture mappings for this skin")]
            public List<HandGestureMapping> rightHandGestures;
        }

        [System.Serializable]
        public class BodyItemOverlay
        {
            [Tooltip("Associated tag for this item")]
            public string tag;

            [Tooltip("Corresponding sprite asset for this item")]
            public Sprite sprite;

            [Tooltip("Should be layered on top of the head sprite?")]
            public bool onTopOfHeadLayer = false;
        }

        [System.Serializable]
        public class ExpressionMapping
        {
            [Tooltip("Keyword for face expressions (main, sad, angry, ...)")]
            public string expressionName;

            [Tooltip("Corresponding sprite asset for this expression")]
            public Sprite sprite;
        }

        [System.Serializable]
        public class HandGestureMapping
        {
            [Tooltip("Keyword for lefthand/righthand gestures")]
            public string gestureName;

            [Tooltip("Corresponding sprite asset for this gesture")]
            public Sprite sprite;

            [Tooltip("Should be layered under the head sprite?")]
            public bool underHeadLayer = false;

            [Tooltip("Corresponding item tags for this gesture (rifle, hat, ...)")]
            public List<string> tags;
        }

        [Tooltip("List of all character mappings in this asset")]
        public List<CharacterMapping> characters;

        [Tooltip("Display settings for anonymous characters")]
        public AnonymousCharacter anonymousCharacter;

        [Header("Current skins"), Tooltip("List of current skins for each character in game.")] [SerializeField]
        public CharacterSkinTrackerAsset skinTrackerAsset;

        // Display name lookup
        private Dictionary<string, string> _displayNameDict;

        // Body sprite: character -> (skinKey -> body Sprite)
        private Dictionary<string, Dictionary<string, Sprite>> _bodySpriteDict;

        // Body item overlays: character -> (skinKey -> list of overlays)
        private Dictionary<string, Dictionary<string, List<BodyItemOverlay>>> _bodyItemOverlayDict;

        // Face expressions: character -> (skinKey -> (expressionName -> face Sprite))
        private Dictionary<string, Dictionary<string, Dictionary<string, Sprite>>> _faceDict;

        // Left-hand gestures: character -> (skinKey -> (gestureName -> left hand mapping))
        private Dictionary<string, Dictionary<string, Dictionary<string, HandGestureMapping>>> _leftHandDict;

        // Right-hand gestures: character -> (skinKey -> (gestureName -> right hand mapping))
        private Dictionary<string, Dictionary<string, Dictionary<string, HandGestureMapping>>> _rightHandDict;

        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (characters == null)
            {
                Debug.LogError(
                    $"{name}: Character list is null! Check if you have mapped the assets in the inspector successfully");
                return;
            }

            if (skinTrackerAsset == null)
            {
                Debug.LogError(
                    $"{name}: SkinTrackerAsset is null! Check if you have mapped the assets in the inspector successfully");
            }

            _displayNameDict = new Dictionary<string, string>();
            _bodySpriteDict = new Dictionary<string, Dictionary<string, Sprite>>();
            _bodyItemOverlayDict = new Dictionary<string, Dictionary<string, List<BodyItemOverlay>>>();
            _faceDict = new Dictionary<string, Dictionary<string, Dictionary<string, Sprite>>>();
            _leftHandDict = new Dictionary<string, Dictionary<string, Dictionary<string, HandGestureMapping>>>();
            _rightHandDict = new Dictionary<string, Dictionary<string, Dictionary<string, HandGestureMapping>>>();

            foreach (var character in characters)
            {
                var charKey = character.characterNameKey;

                // Display name mapping
                _displayNameDict[charKey] = character.shownAs;

                foreach (var skin in character.skins)
                {
                    if (string.IsNullOrEmpty(skin.skinKey))
                    {
                        Debug.LogWarning($"{name}: Skin on character '{charKey}' is missing a skinKey; skipping.");
                        continue;
                    }

                    var skinKey = skin.skinKey;
                    GetOrCreateNestedDict(_bodySpriteDict, charKey)[skinKey] = skin.bodySprite;
                    GetOrCreateNestedDict(_bodyItemOverlayDict, charKey)[skinKey] = skin.bodyItemOverlays;

                    var faceMap = GetOrCreateNestedDict(_faceDict, charKey, skinKey);
                    foreach (var expr in skin.faceExpressions)
                        faceMap[expr.expressionName] = expr.sprite;

                    var leftMap = GetOrCreateNestedDict(_leftHandDict, charKey, skinKey);
                    foreach (var left in skin.leftHandGestures)
                        leftMap[left.gestureName] = left;

                    var rightMap = GetOrCreateNestedDict(_rightHandDict, charKey, skinKey);
                    foreach (var right in skin.rightHandGestures)
                        rightMap[right.gestureName] = right;
                }
            }
        }

        private Dictionary<TKey2, TValue> GetOrCreateNestedDict<TKey1, TKey2, TValue>(
            Dictionary<TKey1, Dictionary<TKey2, TValue>> outerDict, TKey1 key1)
        {
            if (!outerDict.TryGetValue(key1, out var innerDict))
            {
                innerDict = new Dictionary<TKey2, TValue>();
                outerDict[key1] = innerDict;
            }

            return innerDict;
        }

        private Dictionary<TKey3, TValue> GetOrCreateNestedDict<TKey1, TKey2, TKey3, TValue>(
            Dictionary<TKey1, Dictionary<TKey2, Dictionary<TKey3, TValue>>> outerDict,
            TKey1 key1, TKey2 key2)
        {
            if (!outerDict.TryGetValue(key1, out var midDict))
            {
                midDict = new Dictionary<TKey2, Dictionary<TKey3, TValue>>();
                outerDict[key1] = midDict;
            }

            if (!midDict.TryGetValue(key2, out var innerDict))
            {
                innerDict = new Dictionary<TKey3, TValue>();
                midDict[key2] = innerDict;
            }

            return innerDict;
        }

        public string[] GetAllCharacterKeys()
        {
            return characters.Select(c => c.characterNameKey).ToArray();
        }
        
        public bool IsPortraitAvailableForCharacter(string characterKey)
        {
            var character = characters.FirstOrDefault(c => c.characterNameKey == characterKey);
            return character != null && !character.hasNoPortrait;
        }

        // Display name accessor
        public string GetCharacterDisplayName(string characterKey)
        {
            return _displayNameDict.TryGetValue(characterKey, out var displayName)
                ? displayName
                : characterKey;
        }

        // Body sprite accessor
        public Sprite GetBodySprite(string characterKey, string skinKey)
        {
            if (_bodySpriteDict.TryGetValue(characterKey, out var skinMap) &&
                skinMap.TryGetValue(skinKey, out var body))
            {
                if (!body)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(
                        $"[JDialogue]:[{name}]: Body sprite for '{characterKey}' (skin '{skinKey}') is missing, make sure to set the sprite in the asset mapping");
#endif
                }

                return body;
            }

            // Fallback to first skin if available
            if (skinMap != null && skinMap.Count > 0)
            {
                foreach (var kvp in skinMap)
                {
                    if (!kvp.Value)
                    {
#if UNITY_EDITOR
                        Debug.LogWarning(
                            $"[JDialogue]:[{name}]: Body fallback sprite for '{characterKey}' is missing, make sure to set the sprite in the asset mapping");
#endif
                    }

                    return kvp.Value;
                }
            }

#if UNITY_EDITOR
            Debug.LogWarning($"[JDialogue]:[{name}]: No body sprite found for '{characterKey}' (skin '{skinKey}')");
#endif

            return null;
        }

// Body Item Overlay accessor
        public List<BodyItemOverlay> GetBodyItemOverlays(string characterKey, string skinKey)
        {
            if (_bodyItemOverlayDict.TryGetValue(characterKey, out var skinMap) &&
                skinMap.TryGetValue(skinKey, out var overlays))
            {
                return overlays;
            }

#if UNITY_EDITOR
            Debug.LogWarning(
                $"[JDialogue]:[{name}]: Missing body item overlays for '{characterKey}' with skin '{skinKey}'");
#endif

            return null;
        }

        // Face sprite accessor
        public Sprite GetFaceSprite(string characterKey, string skinKey, string expression)
        {
            if (_faceDict.TryGetValue(characterKey, out var skinMap) &&
                skinMap.TryGetValue(skinKey, out var exprMap) &&
                exprMap.TryGetValue(expression, out var sprite))
            {
                if (!sprite)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(
                        $"[JDialogue]:[{name}]: Face sprite for '{expression}' in '{characterKey}' (skin '{skinKey}') is missing, make sure to set the sprite in the asset mapping");
#endif
                }

                return sprite;
            }

#if UNITY_EDITOR
            Debug.LogWarning(
                $"[JDialogue]:[{name}]: Missing face expression '{expression}' for character '{characterKey}' with skin '{skinKey}'");
#endif

            return null;
        }

        // Helper method for logging warnings
        private void LogMissingGestureWarning(string handType, string gesture, string characterKey, string skinKey)
        {
#if UNITY_EDITOR
            Debug.LogWarning(
                $"[JDialogue]:[{name}]: {handType} hand gesture mapping for gesture '{gesture}' in '{characterKey}' (skin '{skinKey}') exists, but the mapping is missing or null. Make sure to assign a valid gesture for this key.");
#endif
        }

        // Helper method for logging missing gesture mappings
        private void LogMissingGestureMappingWarning(string handType, string gesture, string characterKey,
            string skinKey)
        {
#if UNITY_EDITOR
            Debug.LogWarning(
                $"[JDialogue]:[{name}]: Missing {handType} hand gesture mapping for gesture '{gesture}' in '{characterKey}' with skin '{skinKey}'. Ensure the gesture is defined in the asset mapping.");
#endif
        }

        // Generic method to get gesture mapping for left or right hand
        private HandGestureMapping GetHandGestureMapping(string handType, string characterKey, string skinKey,
            string gesture)
        {
            var handDict = handType == "left" ? _leftHandDict : _rightHandDict;
            if (handDict.TryGetValue(characterKey, out var skinMap) &&
                skinMap.TryGetValue(skinKey, out var gestureMap) &&
                gestureMap.TryGetValue(gesture, out var handGesture))
            {
                if (handGesture == null)
                {
                    LogMissingGestureWarning(handType, gesture, characterKey, skinKey);
                }

                return handGesture;
            }

            LogMissingGestureMappingWarning(handType, gesture, characterKey, skinKey);
            return null;
        }

        // Generic method to get sprite for left or right hand gesture
        private Sprite GetHandSprite(string handType, string characterKey, string skinKey, string gesture)
        {
            var handGesture = GetHandGestureMapping(handType, characterKey, skinKey, gesture);
            if (handGesture != null)
            {
                var sprite = handGesture.sprite;
                if (!sprite)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(
                        $"[JDialogue]:[{name}]: {handType} hand sprite for gesture '{gesture}' in '{characterKey}' (skin '{skinKey}') is missing, make sure to set the sprite in the asset mapping.");
#endif
                }

                return sprite;
            }

            return null;
        }

        // Left-hand gesture mapping accessor
        public HandGestureMapping GetLeftHandGestureMapping(string characterKey, string skinKey, string gesture)
        {
            return GetHandGestureMapping("left", characterKey, skinKey, gesture);
        }

        // Right-hand gesture mapping accessor
        public HandGestureMapping GetRightHandGestureMapping(string characterKey, string skinKey, string gesture)
        {
            return GetHandGestureMapping("right", characterKey, skinKey, gesture);
        }

        // Left-hand sprite accessor
        public Sprite GetLeftHandSprite(string characterKey, string skinKey, string gesture)
        {
            return GetHandSprite("left", characterKey, skinKey, gesture);
        }

        // Right-hand sprite accessor
        public Sprite GetRightHandSprite(string characterKey, string skinKey, string gesture)
        {
            return GetHandSprite("right", characterKey, skinKey, gesture);
        }
        
        public Sprite GetDefaultFaceSprite(string characterKey, string skinKey)
        {
            if (_faceDict.TryGetValue(characterKey, out var skinMap) &&
                skinMap.TryGetValue(skinKey, out var exprMap))
            {
                var fallback = exprMap.Values.FirstOrDefault(s => s);
                if (fallback)
                    return fallback;
            }

            return null;
        }
        
        public string GetDefaultLeftHandGestureKey(string characterKey, string skinKey)
        {
            if (_leftHandDict.TryGetValue(characterKey, out var skinMap) &&
                skinMap.TryGetValue(skinKey, out var handDict))
            {
                return handDict.Values.FirstOrDefault()?.gestureName;
            }

            return null;
        }
        
        public string GetDefaultRightHandGestureKey(string characterKey, string skinKey)
        {
            if (_rightHandDict.TryGetValue(characterKey, out var skinMap) &&
                skinMap.TryGetValue(skinKey, out var handDict))
            {
                return handDict.Values.FirstOrDefault()?.gestureName;
            }

            return null;
        }
        
        public Sprite GetDefaultLeftHandSprite(string characterKey, string skinKey)
        {
            if (_leftHandDict.TryGetValue(characterKey, out var skinMap) &&
                skinMap.TryGetValue(skinKey, out var handDict))
            {
                return handDict.Values.FirstOrDefault()?.sprite;
            }

            return null;
        }

        public Sprite GetDefaultRightHandSprite(string characterKey, string skinKey)
        {
            if (_rightHandDict.TryGetValue(characterKey, out var skinMap) &&
                skinMap.TryGetValue(skinKey, out var handDict))
            {
                return handDict.Values.FirstOrDefault()?.sprite;
            }

            return null;
        }
    }
}