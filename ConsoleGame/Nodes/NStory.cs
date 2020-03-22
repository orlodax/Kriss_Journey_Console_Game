using ConsoleGame.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Nodes
{
    public class NStory : SNode
    {
        public NStory(NodeBase nb) : base(nb)
        {
            TextFlow();

            ///go to bottom line
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 2;
            Console.CursorLeft = Console.WindowLeft;

            Console.Write("Press a key to continue...");
            Console.ReadKey();
        }
    }
}
