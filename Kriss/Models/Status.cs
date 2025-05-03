using System.Collections.Generic;

namespace KrissJourney.Kriss.Models;

// stores the items collected and the already visited nodes
public class Status
{
    public List<string> Inventory { get; set; } = [];
    public Dictionary<int, List<int>> VisitedNodes { get; set; } = [];
}