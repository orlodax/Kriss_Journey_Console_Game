using kriss.Classes;
using lybra;
using System;

namespace kriss.Nodes
{
    public class NStory : SNode
    {
        public NStory(NodeBase node) : base (node)
        {
            ///go to bottom line
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 2;
            Console.CursorLeft = Console.WindowLeft;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Press a key to continue...");
            
            while (Console.KeyAvailable) 
                Console.ReadKey(true);

            Console.ReadKey(true);

            NodeFactory.LoadNode(ChildId);
        }
    }
}
