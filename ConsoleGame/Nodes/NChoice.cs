using ConsoleGame.Classes;
using ConsoleGame.Models;
using System;
using System.Threading;

namespace ConsoleGame.Nodes
{
    public class NChoice : SNode
    {
        int selectedRow = 0;
        public NChoice(NodeBase nb) : base(nb)
        {
            Thread.Sleep(ParagraphBreak);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            NodeFactory.CreateNode(Choices[WaitForChoice()].ChildId);
        }
        int WaitForChoice()
        {
            ConsoleKeyInfo key;
            do
            {
                for (int i = 0; i < Choices.Count; i++)
                {
                    if (i == selectedRow)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.Write("\t");
                    Console.Write((i + 1) + ". " + Choices[i].Desc);

                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine();
                    Console.CursorLeft = Console.WindowLeft;
                }

                while (Console.KeyAvailable)
                    Console.ReadKey(true);
                key = Console.ReadKey(true);

                if ((key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow) && selectedRow > 0)
                    selectedRow--;
                if ((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow) && selectedRow < Choices.Count - 1)
                    selectedRow++;

                Console.Clear();
                TextFlow(false);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

            } while (key.Key != ConsoleKey.Enter);

            SaveStatusOnExit();
            return selectedRow;
        }
    }
}

