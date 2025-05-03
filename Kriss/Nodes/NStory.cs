using KrissJourney.Kriss.Classes;
using KrissJourney.Kriss.Models;

namespace KrissJourney.Kriss.Nodes;

public class NStory : NodeBase
{
    public NStory(NodeBase node) : base(node)
    {
        Init();

        ///go to bottom line
        CursorTop = WindowTop + WindowHeight - 2;
        CursorLeft = WindowLeft;

        Typist.WaitForKey(0);

        AdvanceToNext(ChildId);
    }
}