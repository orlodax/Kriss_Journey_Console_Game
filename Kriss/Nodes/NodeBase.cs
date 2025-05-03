using KrissJourney.Kriss.Classes;
using System;
using System.Text.Json.Serialization;

namespace KrissJourney.Kriss.Nodes;

[JsonConverter(typeof(NodeJsonConverter))]
public abstract class NodeBase
{
    public int Id { get; set; } // unique id primary key
    public string Recap { get; set; } // just to sum it up
    public string Type { get; set; } // story, choice, action...
    public string Text { get; set; } // text to be flown
    public string AltText { get; set; } // other text to be flown/displayed (i.e. if the node is already visited)
    public int ChildId { get; set; } // possible id (if single-next)
    public bool IsVisited { get; set; } // have we ever played this node?
    public bool IsLast { get; set; } // more than one node per chapter could be its last
    public bool IsClosing { get; set; } // close game or return to menu if last node of last chapter/section

    public abstract void Load();

    protected void Init()
    {
        // start text
        Clear();
        ForegroundColor = ConsoleColor.DarkCyan; // narrator, default color

        if (IsVisited && AltText != null)
            Typist.RenderText(!IsVisited, AltText);
        else
            Typist.RenderText(!IsVisited, Text);
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