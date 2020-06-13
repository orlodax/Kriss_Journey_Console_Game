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
            string id = number.ToString() + "_01";

            return CurrentNode = CreateNode(id);
        }

        public static SNode CreateNode(string id, SNode previous = null)
        {
            if (previous != null)
                previous.SaveStatusOnExit();

            if (id != null)
            {
                //testing purposes, debug 
                if (id == "test")
                    return CurrentNode = new NStory(DataLayer.DB.Chapters[0].Find(n => n.Id == "test"));

                //-----------------
               
                (NodeBase nb, int chapIndex, int nodeIndex) = SearchNodeById(id);

                if (nb != null)
                {
                    //if first node of chapter, save progress
                    if (nodeIndex == 1)
                        DataLayer.SaveProgress(chapIndex); //chapter index 0 based

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
                        case "MiniGame01":
                            return CurrentNode = new MiniGame01(nb);
                        default:
                            break;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Parses the id provided to extract the matching NodeBase from DataLayer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static (NodeBase nb, int chapIndex, int nodeIndex) SearchNodeById(string id)
        {
            string[] numbers = id.Split("_");

            int chapIndex = Convert.ToInt32(numbers[0]);
            int nodeIndex = Convert.ToInt32(numbers[1]);
            NodeBase nb = DataLayer.DB.Chapters[chapIndex].Find(n => n.Id == id);

            return (nb, chapIndex, nodeIndex);
        }
    }
}
