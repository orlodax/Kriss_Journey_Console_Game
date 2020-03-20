using System;
using System.Collections.Generic;

namespace ConsoleGame.Classes
{
    public class Chapter : ChapterBase
    {
        public List<SNode> Nodes { get; set; } = new List<SNode>();

        public Chapter(int number)
        {
            Number = number;
            Console.Clear();
        }
    }
}
