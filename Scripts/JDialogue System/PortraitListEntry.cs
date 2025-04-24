namespace JDialogue_System
{
    public class PortraitListEntry
    {
        public static readonly string DefaultCharacterKey = "amish";
        public readonly bool DefaultIsAnonymous = false;
        public static readonly string DefaultFace = "main";
        public static readonly string DefaultLeftHand = "lefthand_rest";
        public static readonly string DefaultRightHand = "righthand_rest";

        public string CharacterKey { get; set; }
        public bool IsAnonymous { get; set; }
        public string Face { get; set; }
        public string LeftHand { get; set; }
        public string RightHand { get; set; }

        public PortraitListEntry()
        {
            CharacterKey = DefaultCharacterKey;
            IsAnonymous = DefaultIsAnonymous;
            Face = DefaultFace;
            LeftHand = DefaultLeftHand;
            RightHand = DefaultRightHand;
        }

        public PortraitListEntry(string character, bool isAnonymous, string face, string leftHand, string rightHand)
        {
            CharacterKey = character;
            IsAnonymous = isAnonymous;
            Face = face;
            LeftHand = leftHand;
            RightHand = rightHand;
        }

        public PortraitListEntry(PortraitListEntry entry)
        {
            CharacterKey = entry.CharacterKey;
            IsAnonymous = entry.IsAnonymous;
            Face = entry.Face;
            LeftHand = entry.LeftHand;
            RightHand = entry.RightHand;
        }

        public void ResetDefaults()
        {
            Face = DefaultFace;
            IsAnonymous = DefaultIsAnonymous;
            LeftHand = DefaultLeftHand;
            RightHand = DefaultRightHand;
        }

        public override string ToString()
        {
            return $"{CharacterKey} [IsAnonymous:{IsAnonymous}] ({Face}, {LeftHand}, {RightHand})";
        }
    }
}