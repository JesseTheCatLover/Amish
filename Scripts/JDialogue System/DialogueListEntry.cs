namespace JDialogue_System
{
    public class DialogueListEntry
    {
        public PortraitListEntry MainCharacter { get; set; }
        public PortraitListEntry CompanionCharacter { get; set; }
        public string Dialogue { get; set; }
        
        public DialogueListEntry()
        {
        }
        public DialogueListEntry(PortraitListEntry character, string dialogue)
        {
            MainCharacter = character;
            CompanionCharacter = null;
            Dialogue = dialogue;
        }

        public DialogueListEntry(PortraitListEntry mainCharacter, PortraitListEntry companionCharacter, string dialogue)
        {
            MainCharacter = mainCharacter;
            CompanionCharacter = companionCharacter;
            Dialogue = dialogue;
        }
        
        public override string ToString()
        {
            return CompanionCharacter == null ? $"{MainCharacter.ToString()} -> {Dialogue}" :
                $"{MainCharacter.ToString()} & {CompanionCharacter.ToString()} -> {Dialogue}";
        }
    }
}