using kriss.Classes;
using lybra;
using System;
using System.Linq;
using System.Threading;

namespace kriss.Nodes
{
    public class NDialogue : NodeBase
    {
        ConsoleKeyInfo key;
        int selectedRow = 0;

        public NDialogue(NodeBase node) : base(node)
        {
            this.Init();
            RecursiveDialogues();
        }

        void RecursiveDialogues(int lineId = 0, bool isLineFlowing = true)           //lineid iterates over elements of dialogues[] 
        {
            if (IsVisited)
                isLineFlowing = false;

            var currentLine = Dialogues[lineId];                                    //cureent object selected in the iteration

        #region Drawing base element of the Dialog object (speech part)

            if(currentLine.PreComment != null)
            {
                NodeMethods.TextFlow(isLineFlowing, currentLine.PreComment);
                if (isLineFlowing)
                    Thread.Sleep(NodeMethods.ParagraphBreak);

                Console.WriteLine();
                Console.WriteLine();
            }

            if (currentLine.Line != null)
            {
                //var p = (ConsoleColor)currentLine.Actor;
                if (currentLine.IsTelepathy)
                    NodeMethods.TextFlow(isLineFlowing, "<<" + currentLine.Line + ">> ", (ConsoleColor)currentLine.Actor);
                else
                {
                    //EnActorsColors actor = EnActorsColors.Saberinne;
                    //var pop = (string)actor;
                    //var b = EnActorsColors.Parse(Console.ForegroundColor, currentLine.Actor);
                    //EnActorsColors.
                    //var a = (ConsoleColor)10;
                    NodeMethods.TextFlow(isLineFlowing, "\"" + currentLine.Line + "\" ", (ConsoleColor)currentLine.Actor);
                }
            }
            
            if(currentLine.Comment != null)      
            {
                if (isLineFlowing)
                {
                    Thread.Sleep(NodeMethods.ParagraphBreak);

                    NodeMethods.TextFlow(isLineFlowing, currentLine.Comment);

                    Thread.Sleep(NodeMethods.ParagraphBreak);
                }
                else
                    NodeMethods.TextFlow(isLineFlowing, currentLine.Comment);
            }
            else
                if (isLineFlowing)
                    Thread.Sleep(NodeMethods.ParagraphBreak);                                   //if there was no comment after the line, wait a bit

            Console.WriteLine();
            Console.WriteLine();
            
            if (currentLine.Break)
            {
                HoldScreen();
                Console.Clear();
            }
        #endregion

            if (currentLine.ChildId.HasValue)                                        //if it encounters a link, jump to the node
            {      
                HoldScreen();
                this.AdvanceToNext(currentLine.ChildId.Value);
            }

            if (currentLine.Replies!= null && currentLine.Replies.Any())       //if there are replies inside, display choice
            {
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
                    {
                        selectedRow--;
                        Console.Clear();
                        RecursiveDialogues(0, false);                                //redraw the node to allow the selection effect
                    }
                    if ((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow) && selectedRow < Dialogues[lineId].Replies.Count - 1)
                    {
                        selectedRow++;
                        Console.Clear();
                        RecursiveDialogues(0, false);                                //redraw the node to allow the selection effect
                    }

                } while (key.Key != ConsoleKey.Enter);

                Console.Clear();

                if (currentLine.Replies[selectedRow].ChildId.HasValue)                 //on selecion, either 
                {
                    this.AdvanceToNext(currentLine.Replies[selectedRow].ChildId.Value); //navigate to node specified in selected reply
                }
                else
                {
                    var nextLineId = Dialogues.FindIndex(l => l.LineName == currentLine.Replies[selectedRow].NextLine);
                    RecursiveDialogues(nextLineId);                                   //step to the next line
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(currentLine.NextLine))
                {
                    var nextLineId = Dialogues.FindIndex(l => l.LineName == currentLine.NextLine);
                    RecursiveDialogues(nextLineId, isLineFlowing);
                }
                else 
                    if (Dialogues.Count > lineId + 1)                       
                        RecursiveDialogues(lineId + 1, isLineFlowing);

                this.AdvanceToNext(ChildId);
            }
        }

        void HoldScreen()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Press any key...");

            while (Console.KeyAvailable)
                Console.ReadKey(true);

            Console.ReadKey(true);
        }
    }
}
