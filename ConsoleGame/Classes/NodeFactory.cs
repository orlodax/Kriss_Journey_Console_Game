using ConsoleGame.Nodes;
using System;

namespace ConsoleGame.Classes
{
    public static class NodeFactory
    {
        public static SNode CurrentNode; //reference to displayed node (if it's ever needed)
        public static SNode CreateChapter(int number)
        {
            string id = number.ToString() + "_01";

            NodeBase nb = TextResource.DB.chapters[0].Find(n => n.id == id);
            return CurrentNode = new NStory(nb);
        }

        public static SNode CreateNode(string id)
        {
            var chapIndex = Convert.ToInt32(id[0].ToString());
            NodeBase nb = TextResource.DB.chapters[chapIndex].Find(n => n.id == id);

            //if first node of chapter, save progress
            string[] numbers = id.Split("_");
            if (Convert.ToInt32(numbers[1]) == 1)
                TextResource.SaveProgress(Convert.ToInt32(numbers[0]) - 1); //chapter index 0 based

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
