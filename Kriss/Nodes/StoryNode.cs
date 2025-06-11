using KrissJourney.Kriss.Helpers;

namespace KrissJourney.Kriss.Nodes;

public class StoryNode : NodeBase
{
    public override void Load()
    {
        Init();

        int desiredCursorTop = WindowTop + WindowHeight - 2;
        CursorLeft = WindowLeft;

        // Check if the cursor is already at or below the desired position
        // There's space, so we can set the cursor to the bottom
        if (CursorTop < desiredCursorTop)
        {
            CursorTop = desiredCursorTop;
            Typist.WaitForKey(0);
        }
        else
            Typist.WaitForKey(2);

        AdvanceToNext(ChildId);
    }
}