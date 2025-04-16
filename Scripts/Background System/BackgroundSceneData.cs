using System.Collections.Generic;
using UnityEngine;

namespace Background_System
{
    [CreateAssetMenu(menuName = "Background System/Background Scene Data")]
    public class BackgroundSceneData : ScriptableObject
    {
        public Sprite backgroundSprite;
        public List<GameObject> itemsPrefab;   // A prefab to put under ItemGroups that contains all props or scene elements for that background
        public List<GameObject> lightsPrefab;    // A prefab to put under LightGroups that contains all the lights dedicated to the scene
    }
}