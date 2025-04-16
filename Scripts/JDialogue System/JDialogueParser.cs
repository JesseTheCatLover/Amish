#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public enum Languages { English = 0, Persian = 1 } // Define your supported languages here

namespace JDialogue_System
{
    public static class JDialogueParser
    {
        private static readonly Regex DialoguePattern = new Regex(
            @"^(?:(?<character>[^:]+):)?\s*(?<face>(?!lefthand_|righthand_|<)\S+)?\s*(?<left>lefthand_\w+)?\s*(?<right>righthand_\w+)?\s*(?:<(?<dialogue>[^>]+)>)?\s*$",
            RegexOptions.Compiled);
        
        private static readonly Regex CommentPattern = new Regex(@"(#|//).*");

        public static List<DialogueListEntry> ParseDialogue(TextAsset jDialogue, Languages selectedLanguage)
        {
            List<DialogueListEntry> dialogues = new List<DialogueListEntry>();

            DialogueListEntry currentDialogueList = new DialogueListEntry();
            StringBuilder currentLine = new StringBuilder();
            int selectedLanguageIndex = (int)selectedLanguage; // Get the index of the selected language

            using (StringReader reader = new StringReader(jDialogue.text))
            {
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    string trimmedLine = line.Trim();
                    
                    // Remove inline comments before processing
                    trimmedLine = CommentPattern.Replace(trimmedLine, "").Trim();

                    // Ignore empty lines
                    if (string.IsNullOrEmpty(trimmedLine))
                        continue;

                    // Handle multi-line continuation
                    if (trimmedLine.EndsWith("\\"))
                    {
                        currentLine.Append(trimmedLine.TrimEnd('\\') + "");
                        continue;
                    }
                    else // Single line
                    {
                        currentLine.Append(trimmedLine);
                    }

                    Match match = DialoguePattern.Match(currentLine.ToString());
                    if (match.Success)
                    {
                        string rawCharacter = match.Groups["character"].Success
                            ? match.Groups["character"].Value : currentDialogueList.Character;
                        if (match.Groups["character"].Success)
                        {
                            currentDialogueList.ResetDefaults();
                        }

                        string face = match.Groups["face"].Success ? match.Groups["face"].Value : currentDialogueList.Face;
                        string leftHand = match.Groups["left"].Success
                            ? match.Groups["left"].Value
                            : currentDialogueList.LeftHand;
                        string rightHand = match.Groups["right"].Success
                            ? match.Groups["right"].Value
                            : currentDialogueList.RightHand;

                        currentDialogueList.Character = rawCharacter.TrimEnd('*', ':');
                        if (rawCharacter.TrimEnd(':').EndsWith("*"))
                            currentDialogueList.IsAnonymous = true;
                        currentDialogueList.Face = face;
                        currentDialogueList.LeftHand = leftHand;
                        currentDialogueList.RightHand = rightHand;

                        // Extract multilingual dialogues
                        List<string> dialoguesText = new List<string>(match.Groups[5].Value.Split(','));

                        // Ensure the selected language index is within bounds
                        string selectedDialogue = dialoguesText.Count > selectedLanguageIndex
                            ? dialoguesText[selectedLanguageIndex].Trim().Trim('"')
                            : dialoguesText[0].Trim().Trim('"'); // Fallback to first language

                        // Replace \n with actual new line character
                        selectedDialogue = selectedDialogue.Replace("\\n", "\n");

                        currentDialogueList.Dialogue = selectedDialogue;
                        dialogues.Add(new DialogueListEntry(currentDialogueList));
                    }
                    else
                    {
                        Debug.LogError(
                            $"In File ({jDialogue.name}.jdialogue) : Line ({lineNumber}) : Skipping invalid line at : {trimmedLine}");
                    }

                    currentLine.Clear();
                }
            }

            return dialogues;
        }
    }

}