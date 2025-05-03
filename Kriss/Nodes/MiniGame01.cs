using System;
using System.Collections.Generic;
using KrissJourney.Kriss.Classes;
using KrissJourney.Models;

namespace KrissJourney.Kriss.Nodes;

public class MiniGame01(NodeBase node) : NAction(node)
{
    internal override void TabPressed()
    {
        CursorTop = WindowHeight - 4;
        CursorLeft = WindowLeft;

        ForegroundColor = ConsoleColor.DarkGray;

        WriteLine("Type something for Efeliah to guess. Type 'stop' to stop the guessing session.");

        CursorTop = WindowHeight - 1;
        CursorLeft = 0;

        Typist.RenderPrompt(keysPressed);
    }
    internal override void EnterPressed(List<ConsoleKeyInfo> keysPressed)
    {
        if (keysPressed.Count != 0)
        {
            string[] words = ExtractWords();

            keysPressed.Clear();

            EfGuesses(words);
        }
    }

    void EfGuesses(string[] words)
    {
        if (!string.IsNullOrWhiteSpace(words[0]))
        {
            if (words[0].Equals("stop", StringComparison.OrdinalIgnoreCase))
                this.AdvanceToNext(ChildId);

            RedrawNode();

            BottomMessage = string.Empty;

            foreach (string word in words)
                BottomMessage += "...# " + word;

            BottomMessage = "\"You are thinking: " + BottomMessage + "...?\"#";

            CursorTop = MeasureMessage(BottomMessage);
            CursorLeft = WindowLeft;

            Typist.FlowingText(BottomMessage, ConsoleColor.DarkGreen);
            WriteLine();
            WriteLine();
            Typist.FlowingText("Efeliah opens her eyes,# smiling warmly at you.#");

        }

        PrepareForAction(true); //display prompt without standard refuse
    }
}