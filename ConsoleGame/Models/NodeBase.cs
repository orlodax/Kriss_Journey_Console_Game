using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Models
{
    public class NodeBase
    {
        public string Id { get; set; } //unique id primary key
        public string Type { get; set; } //story, choice, action...
        public string Text { get; set; } //text to be flown
        public string ChildId { get; set; } //possible id (if single-next)
        public List<Choice> Choices { get; set; } //list of possible choices
        public List<Action> Actions { get; set; } = new List<Action>(); //list of possible actions
        public List<Dialogue> Dialogues { get; set; } //all the lines (thus paths) of the node's dialogues

        public bool IsVisited { get; set; }
    }
    public class Child
    {
        public string Id { get; set; }
    }
    public class Choice
    {
        public string Desc { get; set; }
        public string ChildId { get; set; }
    }

}
