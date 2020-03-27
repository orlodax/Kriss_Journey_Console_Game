using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Classes
{
    public class Action
    {
        private string _verb;

        public string verb { get => _verb; set { _verb = value; SetAnswer(); } } //verb of the action
        public string childid { get; set; } //key for matching next node
        public string defaultanswer { get; set; } //answer for incomplete player requests 
        public List<Object> objects { get; set; } = new List<Object>(); //objects for the verbs
        public Condition condition { get; set; }
        void SetAnswer()
        {
            if (verb != null && string.IsNullOrWhiteSpace(verb))
                switch (verb)
                {
                    case "look":
                        defaultanswer = "What shoud I look at? Where?";
                        break;
                    case "take":
                        defaultanswer = "What shoud I take?";
                        break;
                    case "go":
                        defaultanswer = "Where should I go?";
                        break;
                    case "search":
                        defaultanswer = "Where should I search? For what?";
                        break;
                    case "remove":
                        defaultanswer = "What will I remove? from where?";
                        break;
                    case "wear":
                        defaultanswer = "What could I wear?";
                        break;
                    case "rest":
                        defaultanswer = "Where could I lay down?";
                        break;
                    case "drink":
                        defaultanswer = "What will I drink?";
                        break;
                    case "eat":
                        defaultanswer = "What will I eat?";
                        break;
                }
        }
        public class Condition
        {
            public string item { get; set; }
            public bool value { get; set; }
            public string refusal { get; set; }
        }

        public Action()
        {
        
        }

        public bool Evaluate()
        {
            var storedItem = TextResource.DB.inventory.Find(i => i.name == condition.item);
            if (storedItem.had & condition.value)
                return true;
            else
                return false;
        }
    }
}
