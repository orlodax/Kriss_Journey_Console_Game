using ConsoleGame.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Nodes.N01
{
    public class N01_01 : SNode
    {
        public N01_01()
        {
            Console.ReadLine();
            FetchText(this);
            TextFlow();
            SelectNextNode("N01_02");
        }
    }
}
