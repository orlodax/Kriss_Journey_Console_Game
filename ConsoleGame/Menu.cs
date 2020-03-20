using ConsoleGame.Classes;
using System;
using System.Linq;
using System.Reflection;

namespace ConsoleGame
{
    public class Menu
    {
        Navigator Navigator;

        public Menu()
        {
            Navigator = new Navigator();

            Type[] typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "ConsoleGame.Nodes.N01");
            var a = typelist;
            for (int i = 0; i < typelist.Length; i++)
            {
                Console.WriteLine(typelist[i].Name);
            }

            ShowMenu();
        }
        private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
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
