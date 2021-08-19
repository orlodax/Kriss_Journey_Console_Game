using kriss.Classes;
using lybra;

namespace kriss.Nodes;

public class NStory : NodeBase
{
    public NStory(NodeBase node) : base(node)
    {
        this.Init();

        ///go to bottom line
        CursorTop = WindowTop + WindowHeight - 2;
        CursorLeft = WindowLeft;

        Typist.WaitForKey(0);

        this.AdvanceToNext(ChildId);
    }
}