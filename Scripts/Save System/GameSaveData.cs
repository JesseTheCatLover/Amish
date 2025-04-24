using System.Collections.Generic;
using JDialogue_System;

namespace Save_System
{
    [System.Serializable]
    public class GameSaveData
    {
        public List<CharacterSkinTrackerAsset.CharacterSkinData> savedSkins = new();
        // ...
    }
}