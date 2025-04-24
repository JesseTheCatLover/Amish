using System.Collections.Generic;
using JUtils;
using UnityEngine;

namespace Background_System
{
    [ExecuteInEditMode]
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private BackgroundSceneData defaultBackgroundSceneData;
        private SpriteRenderer backgroundRenderer;
        private Transform itemGroups;
        private Transform lightGroups;

        private readonly List<GameObject> _currentItems = new();
        private readonly List<GameObject> _currentLights = new();
        
        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            if (backgroundRenderer == null)
                backgroundRenderer = GetComponent<SpriteRenderer>();

            if (itemGroups == null)
                itemGroups = transform.Find("ItemGroups");

            if (lightGroups == null)
                lightGroups = transform.Find("LightGroups");
        }
        
#if UNITY_EDITOR
        public void LoadDefaultInEditor()
        {
            LoadBackground(defaultBackgroundSceneData);
        }
#endif
        
        public void LoadBackground(BackgroundSceneData data)
        {
            if (!data) return;
            if(backgroundRenderer) backgroundRenderer.sprite = data.backgroundSprite;

            DestroyListObjects(_currentItems, itemGroups);
            DestroyListObjects(_currentLights, lightGroups);

            SpawnListObjects(data.itemsPrefab, itemGroups, _currentItems);
            SpawnListObjects(data.lightsPrefab, lightGroups, _currentLights);
        }

        void DestroyListObjects(List<GameObject> list, Transform parent)
        {
            if (list == null) // lost reference
            {
                GameObjectUtils.ClearChildren(parent);
            }
            foreach (var item in list)
            {
                if (item)
                    GameObjectUtils.SmartDestroy(item);
            }
            list.Clear();
        }
        private void SpawnListObjects(List<GameObject> prefabs, Transform parent, List<GameObject> targetList)
        {
            if (prefabs == null || !parent)
                return;

            foreach (var prefab in prefabs)
            {
                if (!prefab) continue;

                var instance = Instantiate(prefab, parent);
                targetList.Add(instance);
            }
        }
    }
}
