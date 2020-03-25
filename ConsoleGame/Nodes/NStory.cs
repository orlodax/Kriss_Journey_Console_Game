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
            ///go to bottom line
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 2;
            Console.CursorLeft = Console.WindowLeft;

            Console.Write("Press a key to continue...");
            
            while (Console.KeyAvailable) 
                Console.ReadKey(true);

            Console.ReadKey(true);

            Destructor();
            NodeFactory.CreateNode(this.Children[0].id);
        }
    }
}
