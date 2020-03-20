using ConsoleGame.Chapters;

namespace ConsoleGame.Classes
{
    public static class ChapsFactory
    {
        public static Chapter CreateChapter(int number)
        {
            switch (number)
            {
                case 1:
                    return new C01(1);
                
                default:
                    return new C01(1);
            }
        }
    }
}
