namespace JDialogue_System
{
    public class DialogueListEntry
    {
        public static readonly string DefaultCharacter = "amish";
        public readonly bool DefaultIsAnonymous = false;
        public static readonly string DefaultFace = "main";
        public static readonly string DefaultLeftHand = "lefthand_rest";
        public static readonly string DefaultRightHand = "righthand_rest";

        public string Character { get; set; }
        public bool IsAnonymous { get; set; }
        public string Face { get; set; }
        public string LeftHand { get; set; }
        public string RightHand { get; set; }
        public string Dialogue { get; set; }

        public DialogueListEntry()
        {
            Character = DefaultCharacter;
            IsAnonymous = DefaultIsAnonymous;
            Face = DefaultFace;
            LeftHand = DefaultLeftHand;
            RightHand = DefaultRightHand;
            Dialogue = "";
        }

        public DialogueListEntry(string character, bool isAnonymous, string face, string leftHand, string rightHand, string dialogue)
        {
            Character = character;
            IsAnonymous = isAnonymous;
            Face = face;
            LeftHand = leftHand;
            RightHand = rightHand;
            Dialogue = dialogue;
        }

        public DialogueListEntry(DialogueListEntry entry)
        {
            Character = entry.Character;
            IsAnonymous = entry.IsAnonymous;
            Face = entry.Face;
            LeftHand = entry.LeftHand;
            RightHand = entry.RightHand;
            Dialogue = entry.Dialogue;
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
            return $"{Character} [IsAnonymous:{IsAnonymous}] ({Face}, {LeftHand}, {RightHand}) -> \"{Dialogue}\"";
        }
    }
}