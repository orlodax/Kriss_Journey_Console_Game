using kriss.Classes;
using lybra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace kriss.Nodes;

public class MiniGame01 : NAction
{
    public MiniGame01(NodeBase node) : base(node)
    {

    }

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
        if (keysPressed.Any())
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
            if(words[0].ToLower() == "stop")
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