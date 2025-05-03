using KrissJourney.Kriss.Nodes;
using System.Collections.Generic;

namespace KrissJourney.Kriss.Models;

public class Chapter
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<NodeBase> Nodes { get; set; }
}