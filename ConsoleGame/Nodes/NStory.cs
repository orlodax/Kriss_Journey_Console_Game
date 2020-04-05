using ConsoleGame.Classes;
using ConsoleGame.Models;
using System;

namespace ConsoleGame.Nodes
{
    public class NStory : SNode
    {
        public NStory(NodeBase nb) : base(nb)
        {
            ///go to bottom line
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 2;
            Console.CursorLeft = Console.WindowLeft;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Press a key to continue...");
            
            while (Console.KeyAvailable) 
                Console.ReadKey(true);

            Console.ReadKey(true);

            SaveStatusOnExit();
            NodeFactory.CreateNode(ChildId);
        }
    }
}
