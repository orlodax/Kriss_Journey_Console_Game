using ConsoleGame.Nodes.N01;

namespace ConsoleGame.Classes
{
    public static class ChapsFactory
    {
        public static SNode CreateChapter(int number)
        {
            switch (number)
            {
                case 1:
                    return new N01_01();
                
                default:
                    return new N01_01();
            }
        }
    }
}
