using kriss.Models;
using kriss.Nodes;

namespace kriss.Classes
{
    public static class NodeFactory
    {
        public static SNode CurrentNode { get; set; } //reference to displayed node (if it's ever needed)

        public static SNode LoadNode(int? nodeId)
        {
            if (nodeId.HasValue)
            {
                if (CurrentNode != null)
                {
                    // mark exiting node as visited (and chapter if it's last node)
                    CurrentNode.IsVisited = true;
                    DataLayer.SaveProgress();
                }

                //testing purposes, debug 
                if (nodeId == 777)
                    return CurrentNode = DataLayer.Chapters[0].Nodes.Find(n => n.Id == 777) as NStory;
                //-----------------

                SNode newNode = DataLayer.SearchNodeById(nodeId.Value);

                return BuildNode(newNode) ?? new SNode() { Text = $"Node not found for id: {nodeId} !" }; 
            }

            if (CurrentNode.IsLast)
            {
                DataLayer.LoadChapter(DataLayer.CurrentChapter.Id + 1);           
            }

            return new SNode() { Text = $"Id was null and node wasn't the last in the chapter!" };
        }
        public static SNode BuildNode(NodeBase node)
        {
            if (node != null)
            {
                switch (node.Type)
                {
                    case "Story":
                        return CurrentNode = new NStory(node);
                    case "Choice":
                        return CurrentNode = new NChoice(node);
                    case "Dialogue":
                        return CurrentNode = new NDialogue(node);
                    case "Action":
                        return CurrentNode = new NAction(node);
                    case "MiniGame01":
                        return CurrentNode = new MiniGame01(node);
                    default:
                        break;
                }
            }
            return null;
        }
    }


   
}
