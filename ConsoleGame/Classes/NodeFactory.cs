using ConsoleGame.Models;
using ConsoleGame.Nodes;
using System;

namespace ConsoleGame.Classes
{
    public static class NodeFactory
    {
        public static SNode CurrentNode; //reference to displayed node (if it's ever needed)
        public static SNode CreateChapter(int number)
        {
            string id = (number).ToString() + "_01";

            NodeBase nb = DataLayer.DB.Chapters[number].Find(n => n.Id == id);
            return CurrentNode = new NStory(nb);
        }

        public static SNode CreateNode(string id)
        {
            if (id != null)
            {
                //testing purposes
                if (id == "test")
                    return CurrentNode = new NStory(DataLayer.DB.Chapters[0].Find(n => n.Id == "test"));

                //-----------------


                var chapIndex = Convert.ToInt32(id[0].ToString());
                NodeBase nb = DataLayer.DB.Chapters[chapIndex].Find(n => n.Id == id);

                if (nb != null)
                {
                    //if first node of chapter, save progress
                    string[] numbers = id.Split("_");
                    if (Convert.ToInt32(numbers[1]) == 1)
                        DataLayer.SaveProgress(Convert.ToInt32(numbers[0])); //chapter index 0 based

                    switch (nb.Type)
                    {
                        case "Story":
                            return CurrentNode = new NStory(nb);
                        case "Choice":
                            return CurrentNode = new NChoice(nb);
                        case "Dialogue":
                            return CurrentNode = new NDialogue(nb);
                        case "Action":
                            return CurrentNode = new NAction(nb);
                        default:
                            break;
                    }
                }
            }
            return null;
        }
    }
}
