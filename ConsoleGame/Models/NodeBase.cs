using System.Collections.Generic;

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
    public class Condition                      //condition for the viability of the action. normally an item
    {
        public string Type { get; set; }        //is it an item? a previous node to be visited?
        public string Item { get; set; }        // name of the resource
        public bool Value { get; set; }         // value of the resource
        public string Refusal { get; set; }     // message for condition not met
    }
    public class Effect                         // now it affect player. normally inventory
    {
        public string Item { get; set; }        // name of the resource 
        public bool Value { get; set; }         // value of the resource
    }
}
