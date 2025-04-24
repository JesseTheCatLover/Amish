#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public enum Languages { English = 0, Persian = 1 }

namespace JDialogue_System
{
    public static class JDialogueParser
    {
        private static readonly Regex DialoguePattern = new(@"^(?:(?<character>[^:]+):)?\s*(?<content>.*?)\s*<(?<dialogue>[^>]+)>\s*$", RegexOptions.Compiled);
        private static readonly Regex CommentPattern = new(@"(#|//).*");

        public static List<DialogueListEntry> ParseDialogue(TextAsset jDialogue, Languages selectedLanguage)
        {
            var dialogues = new List<DialogueListEntry>();
            var memory = new PortraitListEntry();
            var compMemory = new PortraitListEntry();
            int langIndex = (int)selectedLanguage;

            using var reader = new StringReader(jDialogue.text);
            string? line;
            int lineNumber = 0;
            var currentLine = new StringBuilder();

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                var trimmed = CommentPattern.Replace(line.Trim(), "").Trim(); // Remove comments before processing
                if (string.IsNullOrEmpty(trimmed)) continue; // Ignore empty lines

                if (trimmed.EndsWith("\\")) // Handle multi-line continuation
                {
                    currentLine.Append(trimmed.TrimEnd('\\') + " ");
                    continue;
                }

                currentLine.Append(trimmed);
                currentLine.Replace("\\n", "\n"); // Replace literal `\n` with actual newline characters

                Match match = DialoguePattern.Match(currentLine.ToString());
                if (!match.Success)
                {
                    LogError(jDialogue.name, lineNumber, "Syntax parsing error", trimmed);
                    currentLine.Clear();
                    continue;
                }

                string charBlock = match.Groups["character"].Value.Trim();
                string content = match.Groups["content"].Value.Trim();
                string dialogue = GetDialogueForLanguage(match.Groups["dialogue"].Value, langIndex);

                if (!string.IsNullOrEmpty(charBlock) && charBlock.Contains("&"))
                    HandleDualCharacterLine(charBlock, content, dialogue, ref memory, ref compMemory, dialogues, jDialogue.name, lineNumber);
                else
                    HandleSingleCharacterLine(charBlock, content, dialogue, ref memory, ref compMemory, dialogues);

                currentLine.Clear();
            }

            return dialogues;
        }

        private static void HandleSingleCharacterLine(string charBlock, string content, string dialogue,
            ref PortraitListEntry memory, ref PortraitListEntry compMemory, List<DialogueListEntry> list)
        {
            string character = string.IsNullOrEmpty(charBlock) ? memory.CharacterKey : charBlock.TrimEnd('*');
            bool isAnonymous = charBlock.EndsWith("*");

            if (memory.CharacterKey != character)
            {
                memory.ResetDefaults();
                compMemory.ResetDefaults();
            }

            var (face, left, right) = ParseContentToTokens(content);
            UpdateMemory(ref memory, character, isAnonymous, face, left, right);

            list.Add(new DialogueListEntry(new PortraitListEntry(memory), dialogue));
        }

        private static void HandleDualCharacterLine(string charBlock, string content, string dialogue,
            ref PortraitListEntry memory, ref PortraitListEntry compMemory, List<DialogueListEntry> list,
            string fileName, int lineNumber)
        {
            var chars = charBlock.Split('&');
            string mainChar = chars[0].Trim().TrimEnd('*');
            string compChar = chars[1].Trim().TrimEnd('*');
            bool mainAnon = chars[0].Trim().EndsWith("*");
            bool compAnon = chars[1].Trim().EndsWith("*");

            if (content.Count(c => c == ',') != 1)
            {
                LogError(fileName, lineNumber, "Only one ',' per dual-character line allowed", content);
                return;
            }

            var contents = content.Split(',').Select(s => s.Trim()).ToArray();
            var (mainFace, mainLeft, mainRight) = ParseContentToTokens(contents.ElementAtOrDefault(0) ?? "");
            var (compFace, compLeft, compRight) = ParseContentToTokens(contents.ElementAtOrDefault(1) ?? "");

            if (memory.CharacterKey != mainChar)
            {
                memory.ResetDefaults();
                compMemory.ResetDefaults();
            }
            else if (compMemory.CharacterKey != compChar)
            {
                compMemory.ResetDefaults();
            }

            UpdateMemory(ref memory, mainChar, mainAnon, mainFace, mainLeft, mainRight);
            UpdateMemory(ref compMemory, compChar, compAnon, compFace, compLeft, compRight);

            list.Add(new DialogueListEntry(new PortraitListEntry(memory), new PortraitListEntry(compMemory), dialogue));
        }

        private static void UpdateMemory(ref PortraitListEntry memory, string charKey, bool isAnon,
            string? face, string? left, string? right)
        {
            memory.CharacterKey = charKey;
            memory.IsAnonymous = isAnon;
            memory.Face = face ?? memory.Face;
            memory.LeftHand = left ?? memory.LeftHand;
            memory.RightHand = right ?? memory.RightHand;
        }

        private static string GetDialogueForLanguage(string raw, int index) // TODO: Learn this
        {
            var list = raw.Split(',').Select(s => s.Trim().Trim('"')).ToList();
            return index < list.Count ? list[index] : list[0];
        }

        private static (string? face, string? leftHand, string? rightHand) ParseContentToTokens(string input)
        {
            string? face = null, left = null, right = null;

            foreach (var token in input.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (token.StartsWith("lefthand_")) left = token;
                else if (token.StartsWith("righthand_")) right = token;
                else face = token;
            }

            return (face, left, right);
        }

        private static void LogError(string file, int line, string reason, string lineContent)
        {
            Debug.LogError($"In File ({file}.jdialogue) : Line ({line}) : ({reason}) Skipping => {lineContent}");
        }
    }
}
