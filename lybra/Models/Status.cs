namespace lybra;

// stores the items collected and the already visited nodes
public class Status
{
    public List<string> Inventory { get; set; }
    public Dictionary<int, List<int>> VisitedNodes { get; set; }

    public Status()
    {
        if (Inventory == null)
            Inventory = new List<string>();

        if (VisitedNodes == null)
            VisitedNodes = new Dictionary<int, List<int>>();
    }
}