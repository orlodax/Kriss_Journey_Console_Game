using System;

namespace ConsoleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "KRISS' JOURNEY";
            _ = new Menu();
          //  Console.ReadLine();
        }

        //MONNEZZE


        //lista classi

        //private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        //{
        //    return
        //      assembly.GetTypes()
        //              .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
        //              .ToArray();
        //}

        //Type[] typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "ConsoleGame.Nodes.N01");
        //var a = typelist;
        //    for (int i = 0; i<typelist.Length; i++)
        //    {
        //        Console.WriteLine(typelist[i].Name);
        //    }



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
