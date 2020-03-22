using ConsoleGame.Nodes;

namespace ConsoleGame.Classes
{
    public static class NodeFactory
    {
        public static SNode CreateChapter(int number)
        {
            switch (number)
            {
                case 1:
                    NodeBase nb = TextResource.DB.nodes.Find(n => n.id == "01_01");
                    return new NStory(nb);

                ///add case for each chapter
                default:
                    return new NStory(new NodeBase());
            }
        }

        public static SNode CreateNode(string id)
        {
            NodeBase nb = TextResource.DB.nodes.Find(n => n.id == id);

            switch (nb.type)
            {
                case "Story":
                    return new NStory(nb);
                case "Choice":
                    return new NChoice(nb);
                case "Direction":
                    return new NDirection(nb);
                case "Action":
                    return new NAction(nb);
            }

            return null;
        }
    }
}
