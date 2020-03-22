using ConsoleGame.Classes;
using System;

namespace ConsoleGame
{
    public class Menu
    {
        Navigator Navigator;

        public Menu()
        {
            TextResource.Init();

            Navigator = new Navigator();

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
                Console.WriteLine("You completed chapter no. " + Navigator.LastChapter.Number.ToString());
                Console.WriteLine("Press any key to start the next one.");
                Console.ReadLine();

                Navigator.LoadChapter(Navigator.LastChapter.Number + 1); 
            }
            else
            {
                Console.WriteLine("This game features autosave. You just won't know when.");
                Console.WriteLine();
                Console.WriteLine("Your journey is yet to be started.");
                Console.WriteLine("Press any key.");
                Console.ReadLine();

                Navigator.LoadChapter(1);
            }

            //Console.Clear();
            //Console.ResetColor();

            //Input.WriteOnBottomLine();
        }




    }
}
