using ConsoleGame.Nodes;
using System;

namespace ConsoleGame.Classes
{
    public static class NodeFactory
    {
        public static SNode CurrentNode; //reference to displayed node
        public static SNode CreateChapter(int number)
        {
            
            switch (number)
            {
                case 1:
                    NodeBase nb = TextResource.DB.chapters[0].Find(n => n.id == "0_01");
                    return CurrentNode = new NStory(nb);

                ///add case for each chapter
                default:
                    return CurrentNode = new NStory(new NodeBase());
            }
        }

        public static SNode CreateNode(string id)
        {
            var chapIndex = Convert.ToInt32(id[0].ToString());
            NodeBase nb = TextResource.DB.chapters[chapIndex].Find(n => n.id == id);

            switch (nb.type)
            {
                case "Story":
                    return CurrentNode = new NStory(nb);
                case "Choice":
                    return CurrentNode = new NChoice(nb);
                case "Direction":
                    return CurrentNode = new NDirection(nb);
                case "Action":
                    return CurrentNode = new NAction(nb);
                default:
                    break;
            }

            return null;
        }
    }
}
