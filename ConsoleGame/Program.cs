using System;

namespace ConsoleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "KRISS' JOURNEY";
            Menu MainMenu = new Menu();

            Console.ReadLine();
        }

        //MONNEZZE
        //private static void ReadKeys()
        //{
        //    ConsoleKeyInfo key = new ConsoleKeyInfo();

        //    while (!Console.KeyAvailable && key.Key != ConsoleKey.Escape)
        //    {

        //        key = Console.ReadKey(true);

        //        switch (key.Key)
        //        {
        //            case ConsoleKey.UpArrow:
        //                Console.WriteLine("UpArrow was pressed");
        //                break;
        //            case ConsoleKey.DownArrow:
        //                Console.WriteLine("DownArrow was pressed");
        //                break;

        //            case ConsoleKey.RightArrow:
        //                Console.WriteLine("RightArrow was pressed");
        //                break;

        //            case ConsoleKey.LeftArrow:
        //                Console.WriteLine("LeftArrow was pressed");
        //                break;

        //            case ConsoleKey.Escape:
        //                break;

        //            default:
        //                if (Console.CapsLock && Console.NumberLock)
        //                {
        //                    Console.WriteLine(key.KeyChar);
        //                }
        //                break;
        //        }
        //    }
        //}
    }
}
