using System.Collections.Generic;
using JUtils;
using UnityEngine;
using UnityEngine.UI;

namespace JDialogue_System
{
    public class PortraitRenderer : MonoBehaviour
    {
        [Header("Portrait Tag Setup")] 
        [SerializeField] private string bodyTag = "Body";
        [SerializeField] private string headTag = "Head";
        [SerializeField] private string leftHandTag = "LeftHand";
        [SerializeField] private string rightHandTag = "RightHand";

        private Transform bodyLayer;
        private Transform headLayer;
        private Transform leftHandLayer;
        private Transform rightHandLayer;
        
        private int bodyLayerIndex;
        private int headLayerIndex;
        private int leftHandLayerIndex;
        private int rightHandLayerIndex;

        private HashSet<string> activeOverlayItemsBody = new HashSet<string>();
        private HashSet<string> activeOverlayItemsHead = new HashSet<string>();
        
#if UNITY_EDITOR
        [SerializeField, HideInInspector] private string previewCharacterKey;
#endif

        private void Awake()
        {
            CacheLayers();
            HidePortrait();
        }

        /// <summary>
        ///  Cache all target transforms based on tag
        /// </summary>
        private void CacheLayers()
        {
            bodyLayer = TryFindLayer(bodyTag);
            headLayer = TryFindLayer(headTag);
            leftHandLayer = TryFindLayer(leftHandTag);
            rightHandLayer = TryFindLayer(rightHandTag);
            
            bodyLayerIndex = bodyLayer.transform.GetSiblingIndex();
            headLayerIndex = headLayer.transform.GetSiblingIndex();
            leftHandLayerIndex = leftHandLayer.transform.GetSiblingIndex();
            rightHandLayerIndex = rightHandLayer.transform.GetSiblingIndex();
        }

        private Transform TryFindLayer(string tag)
        {
            var result = GameObjectUtils.FindChildWithTag(transform, tag);
            if (!result)
            {
                Debug.LogWarning(
                    $"[{this.GetType().Name}] Missing child with tag ({tag}) under ({gameObject.name}). make sure you have set the tag properly.");
            }

            return result;
        }
        
#if UNITY_EDITOR
        public void PreviewCharacterInEditor()
        {
            if (!Application.isPlaying)
                CacheLayers();

            ResetOverlayItems();

            var UIHolder = FindFirstObjectByType<UIHolder>();
            if (!UIHolder) return;

            var mapping = UIHolder.dialogueAssetMapping;
            if (!mapping) return;

            ApplyPortrait(mapping);
        }
#endif

        /// <summary>
        /// Uses entry sprites to display portrait on runtime
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="mapping"></param>
        public void ApplyPortrait(PortraitListEntry entry, JDialogueAssetMapping mapping)
        {
            if (!mapping) return;
            if (entry == null || !mapping.IsPortraitAvailableForCharacter(entry.CharacterKey))
            {
                HidePortrait();
                return;
            }

            string key = entry.CharacterKey;
            string skin = mapping.skinTrackerAsset.GetSkin(key);

            OrderHandLayerHierarchy(key, skin, entry.LeftHand, entry.RightHand, mapping);

            SetLayerSprite(bodyLayer, mapping.GetBodySprite(key, skin));
            SetLayerSprite(headLayer, mapping.GetFaceSprite(key, skin, entry.Face));
            SetLayerSprite(leftHandLayer, mapping.GetLeftHandSprite(key, skin, entry.LeftHand));
            SetLayerSprite(rightHandLayer, mapping.GetRightHandSprite(key, skin, entry.RightHand));

            ApplyItemOverlays(entry, mapping);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Uses default sprites to display on editor preview
        /// </summary>
        /// <param name="mapping"></param>
        private void ApplyPortrait(JDialogueAssetMapping mapping)
        {
            string previewKey = previewCharacterKey;
            string skin = mapping.skinTrackerAsset.GetSkin(previewKey);
            
            OrderHandLayerHierarchy(previewKey, skin, mapping.GetDefaultLeftHandGestureKey(previewKey, skin), 
                mapping.GetDefaultRightHandGestureKey(previewKey, skin), mapping);
            
            SetLayerSprite(bodyLayer, mapping.GetBodySprite(previewKey, skin));
            SetLayerSprite(headLayer, mapping.GetDefaultFaceSprite(previewKey, skin));
            SetLayerSprite(leftHandLayer, mapping.GetDefaultLeftHandSprite(previewKey, skin));
            SetLayerSprite(rightHandLayer, mapping.GetDefaultRightHandSprite(previewKey, skin));

            ResetOverlayItems();
            var itemOverlayList = mapping.GetBodyItemOverlays(previewKey, skin);

            foreach (var overlay in itemOverlayList)
            {
                if (overlay.onTopOfHeadLayer)
                {
                    AddOverlayItemIfNotCreated(overlay, overlay.tag, headLayer, ref activeOverlayItemsHead);
                }
                else
                {
                    AddOverlayItemIfNotCreated(overlay, overlay.tag, bodyLayer, ref activeOverlayItemsBody);
                }
            }
        }
#endif

        private void OrderHandLayerHierarchy(string characterKey, string skin, string leftHand, string rightHand,
            JDialogueAssetMapping mapping)
        {
            if (mapping.GetLeftHandGestureMapping(characterKey, skin, leftHand).underHeadLayer)
            {
                leftHandLayer.transform.SetSiblingIndex(headLayerIndex);
            }
            else
            {
                headLayer.transform.SetSiblingIndex(headLayerIndex);
                leftHandLayer.transform.SetSiblingIndex(leftHandLayerIndex);
            }
            if (mapping.GetRightHandGestureMapping(characterKey, skin, rightHand).underHeadLayer)
            {
                rightHandLayer.transform.SetSiblingIndex(headLayerIndex);
            }
            else
            {
                headLayer.transform.SetSiblingIndex(headLayerIndex);
                rightHandLayer.transform.SetSiblingIndex(rightHandLayerIndex);
            }
        }

        private void SetLayerSprite(Transform targetLayer, Sprite sprite, bool transparent = false)
        {
            if (!targetLayer) return;
            var img = targetLayer.GetComponent<Image>();
            if (img)
            {
                img.sprite = sprite;
                img.color = transparent
                    ? Color.clear : // Fully transparent
                    Color.white; // Rest transparency
            }
        }

        public void HidePortrait()
        {
            SetLayerSprite(bodyLayer, null, true);
            SetLayerSprite(headLayer, null, true);
            SetLayerSprite(leftHandLayer, null, true);
            SetLayerSprite(rightHandLayer, null, true);
            ResetOverlayItems();
        }

        private void ResetOverlayItems()
        {
            GameObjectUtils.ClearChildren(bodyLayer);
            GameObjectUtils.ClearChildren(headLayer);
            activeOverlayItemsBody.Clear();
            activeOverlayItemsHead.Clear();
        }

        private void ApplyItemOverlays(PortraitListEntry entry, JDialogueAssetMapping mapping)
        {
            var key = entry.CharacterKey;
            var skin = mapping.skinTrackerAsset.GetSkin(entry.CharacterKey);
            var itemOverlayList = mapping.GetBodyItemOverlays(key, skin);

            var leftHandTags = mapping.GetLeftHandGestureMapping(key, skin, entry.LeftHand).tags;
            var rightHandTags = mapping.GetRightHandGestureMapping(key, skin, entry.RightHand).tags;
            
            UpdateOverlayItemList(itemOverlayList, leftHandTags, rightHandTags);
            
            foreach (var overlay in itemOverlayList)
            {
                string overlayTag = overlay.tag;
                if (leftHandTags.Contains(overlayTag) || rightHandTags.Contains(overlayTag))
                    continue; // Skip if gestures have the tag
                
                if (overlay.onTopOfHeadLayer)
                {
                    AddOverlayItemIfNotCreated(overlay, overlay.tag, headLayer, ref activeOverlayItemsHead);
                }
                else // Add on body layer
                {
                    AddOverlayItemIfNotCreated(overlay, overlay.tag, bodyLayer, ref activeOverlayItemsBody);
                }
            }
        }

        private void UpdateOverlayItemList(List<JDialogueAssetMapping.BodyItemOverlay> itemOverlayList,
            List<string> leftHandTags, List<string> rightHandTags)
        {
            // Collect tags that should be visible in current frame
            HashSet<string> activeItemTags = new HashSet<string>();
            foreach (var overlay in itemOverlayList)
            {
                string overlayTag = overlay.tag;
                if (leftHandTags.Contains(overlayTag) || rightHandTags.Contains(overlayTag))
                    continue; // Skip if gestures have the tag
                activeItemTags.Add(overlayTag);
            }

            DeleteInActiveOverlayItemObjects(activeItemTags, bodyLayer, ref activeOverlayItemsBody);
            DeleteInActiveOverlayItemObjects(activeItemTags, headLayer, ref activeOverlayItemsHead);
        }

        private void DeleteInActiveOverlayItemObjects(HashSet<string> activeTags, Transform layer,
            ref HashSet<string> activeTagsLayer)
        {
            var toRemove = new List<string>();

            // Check each existing overlay
            foreach (Transform child in layer)
            {
                string childTag = child.gameObject.name; // Assuming name is the tag
                if (!activeTags.Contains(childTag) && activeTagsLayer.Contains(childTag))
                {
                    toRemove.Add(childTag);
                }
            }

            // Remove inactive overlays
            foreach (var tag in toRemove)
            {
                var child = layer.Find(tag);
                if (child)
                {
                    GameObjectUtils.SmartDestroy(child.gameObject);
                }
                activeTagsLayer.Remove(tag);
            }
        }

        private void AddOverlayItemIfNotCreated(JDialogueAssetMapping.BodyItemOverlay overlay, string overlayTag,
            Transform parent, ref HashSet<string> activeTagsLayer)
        {
            if (!activeTagsLayer.Contains(overlayTag))
            {
                CreateOverlayItemObject(overlay.sprite, overlayTag, parent);
                activeTagsLayer.Add(overlayTag);
            }
        }
        
        private void CreateOverlayItemObject(Sprite sprite, string tag, Transform parent)
        {
            if (parent.Find(tag)) return; // skip if already exists

            GameObject go = new GameObject(tag, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var img = go.AddComponent<UnityEngine.UI.Image>();
            img.sprite = sprite;
        }
    }
}