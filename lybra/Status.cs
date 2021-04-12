using System.Collections.Generic;

namespace lybra
{
    public class Status                             // represent the current status and the saved game
    {
        public int LastChapter { get; set; }        // to save progress
        public List<Item> Inventory { get; set; }
        //public List<VisitedNode> VisitedNodes { get; set; }
        public Dictionary<int, List<int>> VisitedNodes { get; set; }
    }
}

//public class VisitedNode
//{
//    public int ChapterId { get; set; }
//    public List<int> Nodes { get; set; }
//}
