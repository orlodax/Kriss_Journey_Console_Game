using ConsoleGame.Classes;
using System;

namespace ConsoleGame
{
    public class Menu
    {
        public Menu()
        {
            DataLayer.Init();
           
            ShowMenu();
        }

        void ShowMenu()
        {
            Console.Clear();

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


            //debug: start from
            NodeFactory.CreateNode("1_22");
            //debug


            if (DataLayer.DB.Lastchapter.IsComplete && DataLayer.DB.Lastchapter.Number > 0)
            {
                Console.WriteLine("Welcome back, traveler.");
                Console.WriteLine("So far, you completed chapter no. " + (DataLayer.DB.Lastchapter.Number + 1));
                Console.WriteLine();
                Console.WriteLine("Press any key to start the next one.");
                Console.ReadKey(true);

                NodeFactory.CreateChapter(DataLayer.DB.Lastchapter.Number + 1); 
            }
            else
            {
                Console.WriteLine("Welcome traveler.");
                Console.WriteLine("Your journey is yet to be started.");
                Console.WriteLine();
                Console.WriteLine("This game features autosave. You just won't know when.");
                Console.WriteLine();
                Console.WriteLine("Press any key.");
                Console.ReadKey(true);

                NodeFactory.CreateChapter(1);
            }
        }
    }
}
