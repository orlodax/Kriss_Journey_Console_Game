using kriss.Classes;
using lybra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace kriss.Nodes
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

            if (keysPressed.Any())
                for (int i = 0; i < keysPressed.Count; i++)
                    Console.Write(keysPressed[i].KeyChar.ToString());
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
                
                Console.CursorTop = MeasureMessage(BottomMessage);
                Console.CursorLeft = Console.WindowLeft;

                Typist.FlowingText(BottomMessage, ConsoleColor.DarkGreen);
                Console.WriteLine();
                Console.WriteLine();
                Typist.FlowingText("Efeliah opens her eyes,# smiling warmly at you.#");

            }
            
            PrepareForAction(true); //display prompt without standard refuse
        }
    }
}
