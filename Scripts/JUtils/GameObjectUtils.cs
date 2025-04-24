using System.Collections.Generic;
using UnityEngine;

namespace JUtils
{
    public static class GameObjectUtils
    {
        public static void SmartDestroy(UnityEngine.Object obj, bool forceImmediateDestroy = false)
        {
            if (obj == null)
            {
                return;
            }

            if (forceImmediateDestroy)
                Object.DestroyImmediate(obj);
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(obj);
            }
            else
#endif
            {
                Object.Destroy(obj);
            }
        }

        public static Transform FindChildWithTag(Transform transform, string tag)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag(tag))
                    return child;

                var result = FindChildWithTag(child, tag);
                if (result)
                    return result;
            }
            return null;
        }
        
        public static void ClearChildren(Transform parent, bool forceImmediateDestroy = false)
        {
            if (parent.childCount > 0)
            {
                foreach (Transform item in parent)
                {
                    GameObjectUtils.SmartDestroy(item.gameObject, forceImmediateDestroy);
                }
            }
        }
        
        /// <summary>
        /// Iterate Through All Children (Nested Included)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="result"></param>
        public static void GetAllChildren(Transform parent, List<Transform> result)
        {
            foreach (Transform child in parent)
            {
                result.Add(child);
                GetAllChildren(child, result);
            }
        }
    }
}