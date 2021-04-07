using lybra;
using kriss.Nodes;

namespace kriss.Classes
{
    public static class NodeFactory
    {
        static SNode CurrentNode; //reference to displayed node 

        public static SNode LoadNode(int? nodeId)
        {
            if (nodeId.HasValue)
            {
                // mark exiting node as visited (and chapter if it's last node)
                if (CurrentNode != null)    
                    DataLayer.SaveProgress(CurrentNode);

                //testing purposes, debug 
                //if (nodeId == 777)
                //    return CurrentNode = DataLayer.Chapters[0].Nodes.Find(n => n.Id == 777) as NStory;
                //-----------------

                var newNode = SearchNodeById(nodeId.Value);
                newNode.IsVisited = DataLayer.IsNodeVisited(newNode.Id);

                var historyChapter = DataLayer.Status.VisitedNodes.Find(c => c.ChapterId == DataLayer.CurrentChapter.Id);

                return BuildNode(newNode) ?? new SNode(new NodeBase()) { Text = $"Node not found for id: {nodeId} !" }; 
            }

            if (CurrentNode.IsLast)
                return DataLayer.StartChapter(DataLayer.CurrentChapter.Id + 1);           

            return new SNode(new NodeBase()) { Text = $"Id was null and node wasn't the last in the chapter!" };
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

        /// <summary>
        /// Extract the NodeBase with given id, from DataLayer
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static NodeBase SearchNodeById(int nodeId)
        {
            return DataLayer.CurrentChapter.Nodes.Find(n => n.Id == nodeId);
        }
    }
}
