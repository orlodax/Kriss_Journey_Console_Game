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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("            ██╗  ██╗██████╗ ██╗███████╗███████╗              ");
            Console.WriteLine("            ██║ ██╔╝██╔══██╗██║██╔════╝██╔════╝              ");
            Console.WriteLine("            █████╔╝ ██████╔╝██║███████╗███████╗              ");
            Console.WriteLine("            ██╔═██╗ ██╔══██╗██║╚════██║╚════██║              ");
            Console.WriteLine("            ██║  ██╗██║  ██║██║███████║███████║              ");
            Console.WriteLine("            ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝╚══════╝╚══════╝              ");
            Console.WriteLine("     ██╗ ██████╗ ██╗   ██╗██████╗ ███╗   ██╗███████╗██╗   ██╗");
            Console.WriteLine("     ██║██╔═══██╗██║   ██║██╔══██╗████╗  ██║██╔════╝╚██╗ ██╔╝");
            Console.WriteLine("     ██║██║   ██║██║   ██║██████╔╝██╔██╗ ██║█████╗   ╚████╔╝ ");
            Console.WriteLine("██   ██║██║   ██║██║   ██║██╔══██╗██║╚██╗██║██╔══╝    ╚██╔╝  ");
            Console.WriteLine("╚█████╔╝╚██████╔╝╚██████╔╝██║  ██║██║ ╚████║███████╗   ██║   ");
            Console.WriteLine(" ╚════╝  ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝╚══════╝   ╚═╝   ");
            Console.WriteLine();
            Console.WriteLine("              />_________________________________");
            Console.WriteLine("     [########[]_________________________________>");
            Console.WriteLine("              \\>");
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------------");
            
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine();
            //debug: start from
            //NodeFactory.CreateNode("3_05");
            //debug


            if (DataLayer.DB.Lastchapter.IsComplete && DataLayer.DB.Lastchapter.Number > 0)
            {
                Console.WriteLine("Welcome back, traveler. Press any key to start the next chapter.");
                Console.WriteLine("Press a number if you want to replay an already completed chapter.");
                Console.WriteLine("So far, you completed these chapters:");
                Console.WriteLine();

                for (int i = 0; i < DataLayer.DB.Lastchapter.Number; i++)
                    Console.WriteLine(DataLayer.Titles[i]);

                var key = Console.ReadKey(true);

                if (char.IsDigit(key.KeyChar))
                    NodeFactory.CreateChapter(Convert.ToInt32(key.KeyChar.ToString()) - 1);
                else
                    NodeFactory.CreateChapter(DataLayer.DB.Lastchapter.Number); 
            }
            else
            {
                Console.WriteLine("Welcome traveler. Your journey is yet to be started.");
                Console.WriteLine("This game features autosave. You just won't know when.");
                Console.WriteLine();
                Console.WriteLine("Press any key.");
                Console.ReadKey(true);

                NodeFactory.CreateChapter(0);
            }
        }

    }
}
