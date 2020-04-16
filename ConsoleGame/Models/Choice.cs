using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Models
{
    public class Choice
    {
        public string Desc { get; set; }
        public string ChildId { get; set; }
        public Condition Condition { get; set; } //condition for the viability of the action. normally an item
        public Effect Effect { get; set; } //consequence from the base action
    }
}
