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
            FetchText(this);
            PrepareForInput();
            // SelectNextNode("N01_02");
        }
    }
}
