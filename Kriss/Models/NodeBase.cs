using System;
using System.Collections.Generic;
using KrissJourney.Kriss.Classes;

namespace KrissJourney.Kriss.Models;

public class NodeBase
{
    public int Id { get; set; } //unique id primary key
    public string Recap { get; set; } //just to sum it up
    public string Type { get; set; } //story, choice, action...
    public string Text { get; set; } //text to be flown
    public string AltText { get; set; } //other text to be flown/displayed (i.e. if the node is already visited)
    public int ChildId { get; set; } //possible id (if single-next)
    public List<Choice> Choices { get; set; } //list of possible choices
    public List<Action> Actions { get; set; } //list of possible actions
    public List<Dialogue> Dialogues { get; set; } //all the lines (thus paths) of the node's dialogues
    public bool IsVisited { get; set; } //have we ever played this node?
    public bool IsLast { get; set; } //more than one node per chapter could be its last
    public bool IsClosing { get; set; } //close game or return to menu if last node of last chapter/section

    public NodeBase(NodeBase n)
    {
        Id = n.Id;
        Recap = n.Recap;
        Type = n.Type;
        Text = n.Text;
        AltText = n.AltText;
        ChildId = n.ChildId;
        Choices = n.Choices;
        Actions = n.Actions;
        Dialogues = n.Dialogues;
        IsVisited = n.IsVisited;
        IsLast = n.IsLast;
        IsClosing = n.IsClosing;
    }
    public NodeBase()
    {

    }

    protected void Init()
    {
        // start text
        Clear();
        ForegroundColor = ConsoleColor.DarkCyan; //narrator, default color

        string text;
        if (IsVisited && AltText != null)
            text = AltText;
        else
            text = Text;

        Typist.RenderText(!IsVisited, text);
    }

    protected void AdvanceToNext(int childId)
    {
        // mark caller as visited
        DataLayer.SaveProgress();

        // if it closes story or section, go back to menu
        if (IsClosing)
            DataLayer.DisplayMenu();

        // if it closes chapter load the next chapter, else load next node
        if (IsLast)
            DataLayer.StartNextChapter();

        DataLayer.LoadNode(childId);
    }
}