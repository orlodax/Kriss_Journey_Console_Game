using System.Collections.Generic;

namespace Lybra;

public class Chapter
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<NodeBase> Nodes { get; set; }
}