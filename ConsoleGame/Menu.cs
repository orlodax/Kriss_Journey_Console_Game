using ConsoleGame.Classes;
using System;

namespace ConsoleGame
{
    public class Menu
    {
        public Menu()
        {
            TextResource.Init();
           
            Navigator.LoadProgress();

            ShowMenu();
        }

        void ShowMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("         />_________________________________");
            Console.WriteLine("[########[]_________________________________>");
            Console.WriteLine("         \\>");
            Console.WriteLine();
            Console.WriteLine("                 KRISS' JOURNEY");
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            if (Navigator.LastChapter.IsComplete && Navigator.LastChapter.Number > 0)
            {
                Console.WriteLine("So far, you completed chapter no. " + Navigator.LastChapter.Number.ToString());
                Console.WriteLine("Press any key to start the next one.");
                Console.ReadLine();

                Navigator.LoadChapter(Navigator.LastChapter.Number + 1); 
            }
            else
            {
                Console.WriteLine("Welcome traveler.");
                Console.WriteLine("Your journey is yet to be started.");
                Console.WriteLine();
                Console.WriteLine("This game features autosave. You just won't know when.");
                Console.WriteLine();
                Console.WriteLine("Press any key.");
                Console.ReadLine();

                Navigator.LoadChapter(1);
                //debug: start from
                //NodeFactory.CreateNode("0_08");
            }
        }
    }
}
