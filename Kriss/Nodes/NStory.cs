using Kriss.Classes;
using Lybra;

namespace Kriss.Nodes;

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