using KrissJourney.Kriss.Helpers;

namespace KrissJourney.Kriss.Nodes;

public class FightNode : NodeBase
{
    public override void Load()
    {
        Init();
        WriteLine();
        WriteLine();

        if (IsVisited)
        {
            Typist.InstantText("You already defeated this enemy. You're off the hook this time.", System.ConsoleColor.DarkRed);
            Typist.WaitForKey(3);

            AdvanceToNext(ChildId);
            return;
        }

        Fight();
    }

    void Fight()
    {
        if (IsThisNode(chapterId: 9, nodeId: 1)) // To display tutorial the first time player encounters a fight
        {
            // display tutorial
        }

        Typist.RenderText(isFlowing: true, "Prepare to fight!", System.ConsoleColor.Red);
        WriteLine();


        Typist.WaitForKey(3);
        AdvanceToNext(ChildId);
    }
}
