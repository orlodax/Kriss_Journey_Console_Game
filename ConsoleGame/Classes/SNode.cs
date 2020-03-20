using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Classes
{
    public class SNode
    {
        public List<string> Parents { get; set; } = new List<string>();

        public string ID { get; set; }
        public string Text { get; set; }
    }
}
