using ConsoleGame.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Nodes
{
    public class NDialogue : SNode
    {
        public NDialogue(NodeBase nb) : base(nb)
        {
            DisplayLines();
        }

        void DisplayLines(int lineId = 0) 
        {
            //redraw node
            Refresh(lineId);

            if (Dialogues[lineId].ChildId != null)
                NodeFactory.CreateNode(Dialogues[lineId].ChildId);

            if (Dialogues[lineId].Replies.Count == 0)
                DisplayLines(lineId++);
            else
            {
                ConsoleKeyInfo key;
                int selectedRow = 0;

                do
                {
                    for (int i = 0; i < Dialogues[lineId].Replies.Count; i++)
                    {
                        if (i == selectedRow)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkCyan;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        Console.Write("\t");
                        Console.Write((i + 1) + ". " + Dialogues[lineId].Replies[i].Line);

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
                    if ((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow) && selectedRow < Dialogues[lineId].Replies.Count - 1)
                        selectedRow++;

                    Refresh(lineId, false);
     
                } while (key.Key != ConsoleKey.Enter);

                if (Dialogues[lineId].Replies[selectedRow].ChildId != null)
                    NodeFactory.CreateNode(Dialogues[lineId].Replies[selectedRow].ChildId);
                else
                    DisplayLines(Dialogues[lineId].Replies[selectedRow].NextLine);
            }
        }

        void Refresh(int lineId, bool isLineFlowing = true)
        {
            Console.Clear();
            TextFlow(false);

            Console.ForegroundColor = DataLayer.ActorsColors[Dialogues[lineId].Actor];
            TextFlow(isLineFlowing, Dialogues[lineId].Line);

            if (Dialogues[lineId].Replies.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Press any key...");
                Console.ReadKey(true);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
