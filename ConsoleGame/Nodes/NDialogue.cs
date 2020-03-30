using ConsoleGame.Classes;
using System;
using System.Threading;

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

            //if the dialogue is "fake" = just a line with a new node id, enter here and navigate to the new node
            if (Dialogues[lineId].ChildId != null)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Press any key...");
                Console.ReadKey(true);
                NodeFactory.CreateNode(Dialogues[lineId].ChildId);
            }

            //if the current dialogue does not have possible replies in it, and it's not the last of the sequence, step to the next
            if (Dialogues[lineId].Replies == null)
            {
                if (Dialogues.Count > lineId + 1)
                    DisplayLines(lineId + 1);
            }
            else
            {
                ConsoleKeyInfo key;
                int selectedRow = 0;

                do
                {
                    for (int i = 0; i < Dialogues[lineId].Replies.Count; i++)       //draw the replies, select them
                    {
                        if (i == selectedRow)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkCyan;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        Console.Write("\t");
                        Console.Write((i + 1) + ". \"" + Dialogues[lineId].Replies[i].Line + "\"");

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

                    Refresh(lineId, false);                                         //redraw the node to allow the selection effect

                } while (key.Key != ConsoleKey.Enter);

                if (Dialogues[lineId].Replies[selectedRow].ChildId != null)                 //on selecion, either 
                    NodeFactory.CreateNode(Dialogues[lineId].Replies[selectedRow].ChildId); //navigate to node specified in selected reply
                else
                    DisplayLines(Dialogues[lineId].Replies[selectedRow].NextLine);          //step to the next line
            }
        }

        void Refresh(int lineId, bool isLineFlowing = true)
        {
            Console.Clear();
            TextFlow(false);

            if (!Dialogues[lineId].IsExchange)      //standard dialogue with possible player answers or not
            {
                Console.ForegroundColor = DataLayer.ActorsColors[Dialogues[lineId].Actor];
                TextFlow(isLineFlowing, "\"" + Dialogues[lineId].Line + "\"");

                if (Dialogues[lineId].Comment != null)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.ForegroundColor = DataLayer.ActorsColors["Narrator"];
                    TextFlow(isLineFlowing, Dialogues[lineId].Comment);
                }

                if (Dialogues[lineId].Replies == null)
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
            else                                    //no answers, it's a sequence between NPCs (IsExchange)
            {
                //start with the optional precomment
                if (Dialogues[lineId].PreComment != null)
                {
                    TextFlow(isLineFlowing, Dialogues[lineId].PreComment);
                    Console.WriteLine();
                    Console.WriteLine();
                }

                bool everyOther = false;

                //iterate the exchange
                foreach (var reply in Dialogues[lineId].Replies)
                {
                    Console.ForegroundColor = DataLayer.ActorsColors[reply.Actor];
                    TextFlow(isLineFlowing, reply.Line);

                    if (reply.Comment != null)
                    {
                        Console.ForegroundColor = DataLayer.ActorsColors["Narrator"];
                        TextFlow(isLineFlowing, " " + reply.Comment);
                    }
                    Thread.Sleep(ParagraphBreak);
                    Console.WriteLine();
                    Console.WriteLine();

                    if (everyOther)
                    {
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("Press any key...");
                        Console.ReadKey(true);

                        Console.Clear();
                    }

                    if (reply.ChildId != null)
                        NodeFactory.CreateNode(reply.ChildId);

                    everyOther = !everyOther;
                }
            }
        }
    }
}
