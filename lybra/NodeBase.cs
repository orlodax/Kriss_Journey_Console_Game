using System.Collections.Generic;

namespace lybra
{
    public class NodeBase
    {
        public int Id { get; set; } //unique id primary key
        public string Recap { get; set; } 
        public string Type { get; set; } //story, choice, action...
        public string Text { get; set; } //text to be flown
        public string AltText { get; set; } //other text to be flown/displayed (i.e. if the node is already visited)
        public int ChildId { get; set; } //possible id (if single-next)
        public List<Choice> Choices { get; set; } //list of possible choices
        public List<Action> Actions { get; set; } = new List<Action>(); //list of possible actions
        public List<Dialogue> Dialogues { get; set; } //all the lines (thus paths) of the node's dialogues
        public bool IsVisited { get; set; }
        public bool IsLast { get; set; }

        public NodeBase(NodeBase n)
        {
            Id = n.Id;
            Recap = n.Recap;
            Type = n.Type;
            Text = n.Text;
            AltText = n.AltText;
            ChildId = n.ChildId;
            Choices = n.Choices;
            Actions = n.Actions;
            Dialogues = n.Dialogues;
            IsVisited = n.IsVisited;
            IsLast = n.IsLast;
        }
        public NodeBase()
        {

        }
    }
}
