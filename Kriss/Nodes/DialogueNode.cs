﻿using System;
using System.Collections.Generic;
using KrissJourney.Kriss.Classes;
using KrissJourney.Kriss.Models;

namespace KrissJourney.Kriss.Nodes;

public class DialogueNode : NodeBase
{
    ConsoleKeyInfo key;
    int selectedRow = 0;

    public List<Dialogue> Dialogues { get; set; } // all the lines (thus paths) of the node's dialogues

    public override void Load()
    {
        Init();
        RecursiveDialogues(isFirstDraw: true);
    }

    void RecursiveDialogues(int lineId = 0, bool isLineFlowing = true, bool isFirstDraw = false)           //lineid iterates over elements of dialogues[] 
    {
        if (lineId == 0 && !isFirstDraw)
            Init();

        if (IsVisited)
            isLineFlowing = false;
        // else, it depends

        Dialogue currentLine = Dialogues[lineId];                               //current object selected in the iteration

        #region Drawing base element of the Dialog object (speech part)

        if (currentLine.PreComment != null)
        {
            Typist.RenderNonSpeechPart(isLineFlowing, currentLine.PreComment);

            WriteLine();
            WriteLine();
        }

        if (currentLine.Line != null)
            Typist.RenderLine(isLineFlowing, currentLine);

        if (currentLine.Comment != null)
            Typist.RenderNonSpeechPart(isLineFlowing, currentLine.Comment);

        WriteLine();
        WriteLine();

        if (currentLine.Break || IsLast && Dialogues.Count == Dialogues.IndexOf(currentLine) + 1)
        {
            Typist.WaitForKey(2);                                 //pause if it's marked as break or if it's last line of chapter
            Clear();
        }
        #endregion

        if (currentLine.ChildId.HasValue)                                       //if it encounters a link, jump to the node
        {
            Typist.WaitForKey(2);
            AdvanceToNext(currentLine.ChildId.Value);
        }

        if (currentLine.Replies != null && currentLine.Replies.Count != 0)            //if there are replies inside, display choice
        {
            for (int i = 0; i < Dialogues[lineId].Replies.Count; i++)           //draw the replies, select them
            {
                if (i == selectedRow)
                {
                    BackgroundColor = Typist.GetMappedColor(ConsoleColor.DarkCyan); ;
                    ForegroundColor = Typist.GetMappedColor(ConsoleColor.White); ;
                }
                Write("\t");
                Write(i + 1 + ". " + Dialogues[lineId].Replies[i].Line);

                ResetColor();
                ForegroundColor = Typist.GetMappedColor(ConsoleColor.DarkCyan); ;
                WriteLine();
                CursorLeft = WindowLeft;
            }

            while (KeyAvailable)
                ReadKey(true);
            key = ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                Clear();

                if (currentLine.Replies[selectedRow].ChildId.HasValue)                  //on selection, either 
                    AdvanceToNext(currentLine.Replies[selectedRow].ChildId.Value); //navigate to node specified in selected reply
                else                                                                    //or jump to the next line
                    RecursiveDialogues(Dialogues.FindIndex(l => l.LineName == currentLine.Replies[selectedRow].NextLine));
            }

            if ((key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow) && selectedRow > 0)
                selectedRow--;
            if ((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow) && selectedRow < Dialogues[lineId].Replies.Count - 1)
                selectedRow++;

            do
            {
                if (lineId <= 0)
                    break;

                lineId--;
            }
            while (Dialogues[lineId].Break == false);

            if (Dialogues[lineId].Break)
                lineId++;

            Clear();
            RecursiveDialogues(lineId, false);               //redraw the node to allow the selection effect
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(currentLine.NextLine))
            {
                int nextLineId = Dialogues.FindIndex(l => l.LineName == currentLine.NextLine);
                RecursiveDialogues(nextLineId, isLineFlowing);
            }
            else
                if (Dialogues.Count > lineId + 1)
                RecursiveDialogues(lineId + 1, isLineFlowing);

            AdvanceToNext(ChildId);
        }
    }
}