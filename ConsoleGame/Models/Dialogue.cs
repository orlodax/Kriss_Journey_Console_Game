using System.Collections.Generic;

namespace ConsoleGame.Models
{
    public class Dialogue
    {
        public string Actor { get; set; }
        public int LineId { get; set; }
        public bool IsExchange { get; set; }
        public string PreComment { get; set; }
        public string Comment { get; set; }
        public string ChildId { get; set; }
        public string Line { get; set; }
        public List<Reply> Replies { get; set; }
    }
    public class Reply
    {
        public string Actor { get; set; }
        public string Line { get; set; }
        public string Comment { get; set; }
        public string ChildId { get; set; }
        public int NextLine { get; set; }
    }

}
