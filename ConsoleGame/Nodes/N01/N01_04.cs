using ConsoleGame.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Nodes.N01
{
    public class N01_04 : SNode
    {
        public N01_04()
        {
            ID = "01_04";
            Parents.Add("01_03");
            Parents.Add("01_02");
        }
    }
}
