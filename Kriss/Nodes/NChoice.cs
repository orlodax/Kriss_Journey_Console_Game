using System;
using System.Collections.Generic;
using KrissJourney.Kriss.Classes;
using KrissJourney.Models;

namespace KrissJourney.Kriss.Nodes;

public class NChoice : NodeBase
{
    int selectedRow = 0;
    readonly List<Choice> visibleChoices = [];

    public NChoice(NodeBase node) : base(node)
    {
        this.Init();

        WriteLine();
        WriteLine();
        WriteLine();

        DisplayChoices();

        WaitForChoice();
    }

    /// <summary>
    /// Displays the choices that satisfy a possible condition and that are not hidden
    /// </summary>
    void DisplayChoices()
    {
        List<Choice> notHiddenChoices = Choices.FindAll(c => c.IsHidden == false);   //first filter out all non hidden ones

        foreach (Choice c in notHiddenChoices)                                 //crawl trough looking for those which does not satisfy possible condition
        {
            Condition cond = c.Condition;
            if (cond != null)
            {
                if (cond.Type == "isNodeVisited")
                {
                    if (int.TryParse(cond.Item, out int nodeId))            //item in this case contains node id
                    {
                        if (DataLayer.IsNodeVisited(nodeId))
                            visibleChoices.Add(c);
                    }
                    else
                        throw new Exception("IsNodeVisited Condition wasn't an integer!!");
                }
                else
                    visibleChoices.Add(c);
            }
            else
                visibleChoices.Add(c);
        }
    }

    /// <summary>
    /// Recursive method to capture user choice
    /// </summary>
    void WaitForChoice()
    {
        ConsoleKeyInfo key;
        do
        {
            for (int i = 0; i < visibleChoices.Count; i++)
            {
                ConsoleColor foreground = ConsoleColor.DarkCyan;
                ConsoleColor background = ConsoleColor.Black;

                if (Choices[i].IsPlayed)
                {
                    foreground = ConsoleColor.DarkGray;
                    if (i == selectedRow)
                    {
                        background = ConsoleColor.DarkGray;
                        foreground = ConsoleColor.White;
                    }
                }
                else
                {
                    if (i == selectedRow)
                    {
                        background = ConsoleColor.DarkBlue;
                        foreground = ConsoleColor.White;
                    }
                }

                Write("\t");
                ForegroundColor = foreground;
                BackgroundColor = background;
                Write(i + 1 + ". " + visibleChoices[i].Desc);

                ResetColor();
                WriteLine();
                CursorLeft = WindowLeft;
            }

            while (KeyAvailable)
                ReadKey(true);
            key = ReadKey(true);

            if ((key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow) && selectedRow > 0)
                selectedRow--;
            if ((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow) && selectedRow < visibleChoices.Count - 1)
                selectedRow++;

            Clear();

            Typist.InstantText(Text);

            WriteLine();
            WriteLine();
            WriteLine();
        }
        while (key.Key != ConsoleKey.Enter);

        Choice choice = visibleChoices[selectedRow];

        if (choice.IsPlayed)
        {
            if (choice.IsNotRepeatable)
            {
                RedrawNode();
                WaitForChoice();
            }
        }
        if (DataLayer.Evaluate(choice.Condition))
        {
            if (choice.Effect != null)
                DataLayer.StoreItem(choice.Effect);

            if (choice.UnHide.HasValue)                  //if this choice unlocks others
            {
                int UnHide = (int)choice.UnHide;
                Choices[UnHide].IsHidden = false;
            }

            choice.IsPlayed = true;

            this.AdvanceToNext(choice.ChildId);
        }
        else
        {
            CursorTop = WindowHeight - 4;
            CursorLeft = WindowLeft;

            Typist.FlowingText(choice.Refusal, ConsoleColor.DarkYellow);
            Typist.WaitForKey(2);

            RedrawNode();
            WaitForChoice();
        }
    }
    void RedrawNode()
    {
        Clear();

        Typist.InstantText(Text);

        WriteLine();
        WriteLine();
        WriteLine();
    }
}