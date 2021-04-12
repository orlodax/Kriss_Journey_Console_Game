using lybra;
using kriss.Nodes;
using System;
using System.Collections.Generic;

namespace kriss.Classes
{
    public class MiniGame01 : NAction
    {
        public MiniGame01(NodeBase node) : base(node)
        {

        }

        internal override void TabPressed() 
        {
            Console.CursorTop = Console.WindowHeight - 4;
            Console.CursorLeft = Console.WindowLeft;

            Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.WriteLine("Type something for Efeliah to guess. Type 'stop' to stop the guessing session.");

            Console.CursorTop = Console.WindowHeight - 1;
            Console.CursorLeft = 0;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\\>");
            Console.CursorLeft += 1;

            if (keysPressed.Count > 0)
                for (int i = 0; i < keysPressed.Count; i++)
                    Console.Write(keysPressed[i].KeyChar.ToString());
        }
        internal override void EnterPressed(List<ConsoleKeyInfo> keysPressed)
        {
            if (keysPressed.Count > 0)
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
                    AdvanceToNext(ChildId);

                RedrawNode();

                BottomMessage = string.Empty;

                foreach (string word in words)
                    BottomMessage += "...# " + word;
                
                BottomMessage = "\"You are thinking: " + BottomMessage + "...?\"#";
                
                Console.CursorTop = MeasureMessage(BottomMessage);
                Console.CursorLeft = Console.WindowLeft;

                TextFlow(true, BottomMessage, ConsoleColor.DarkGreen);
                Console.WriteLine();
                Console.WriteLine();
                TextFlow(true, "Efeliah opens her eyes,# smiling warmly at you.#", ConsoleColor.DarkCyan);

            }
            
            PrepareForAction(true); //display prompt without standard refuse
        }
    }
}
