using KrissJourney.Kriss.Classes;

namespace KrissJourney.Kriss.Nodes;

public class StoryNode : NodeBase
{
    public override void Load()
    {
        Init();

        ///go to bottom line
        CursorTop = WindowTop + WindowHeight - 2;
        CursorLeft = WindowLeft;

        Typist.WaitForKey(0);

        AdvanceToNext(ChildId);
    }
}