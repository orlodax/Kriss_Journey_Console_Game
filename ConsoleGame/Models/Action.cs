using kriss.Classes;
using System.Collections.Generic;

namespace kriss.Models
{
    public class Action
    {
        public List<Word> Verbs { get; set; } //verb of the action
        public int ChildId { get; set; } //key for matching next node
        public string Answer { get; set; } //answer for incomplete player requests 
        public List<Object> Objects { get; set; } = new List<Object>(); //objects for the verbs
        public Condition Condition { get; set; } //condition for the viability of the action. normally an item
        public Effect Effect { get; set; } //consequence from the base action
        public string GetOpinion(string word)           // to get response message when action requires an object and player does not provide any valid
        {
            if (Answer != null)
                return Answer;
            else
            {
                if (word != null && !string.IsNullOrWhiteSpace(word))
                    switch (word)
                    {
                        case "look":
                            return "What shoud I look at? Where?";
                        case "take":
                            return "What shoud I take?";
                        case "go":
                            return "Where should I go?";
                        case "search":
                            return "Where should I search? For what?";
                        case "remove":
                            return "What will I remove? from where?";
                        case "wear":
                            return "What could I wear?";
                        case "rest":
                            return "Where could I lay down?";
                        case "drink":
                            return "What will I drink?";
                        case "eat":
                            return "What will I eat?";
                        case "sleep":
                            return "Where can I sleep?";
                        case "say":
                            return "What could I say?";
                        case "ask":
                            return "What will I ask?";
                    }
                return "Sorry can't do that.";
            }
        }


        public bool EvaluateSimple()                            // check according to the condition
        {
            if (Condition != null)
            {
                var storedItem = DataLayer.Status.Inventory.Find(i => i.Name == Condition.Item);
                if (storedItem != null)
                {
                    if (storedItem.Had & Condition.Value)
                        return true;
                }
                return false;
            }
            return true;
        }
        public bool EvaluateCombination(Models.Object o)                       // check according to the condition
        {
            if (o.Condition != null)
            {
                var storedItem = DataLayer.Status.Inventory.Find(i => i.Name == o.Condition.Item);
                if (storedItem != null)
                {
                    if (storedItem.Had & o.Condition.Value)
                        return true;
                }
                return false;
            }
            return true;
        }
        public void StoreItem(Effect effect)       // consequent modify of inventory
        {
            var itemToStore = new Item() { Name = effect.Item, Had = effect.Value };
            DataLayer.Status.Inventory.Add(itemToStore);
        }
    }
    public class Object
    {
        public List<Word> Objs { get; set; }         //objects of the action
        public string Answer { get; set; }      //answer for incomplete player requests 
        public int ChildId { get; set; }     //key for matching next node
        public Effect Effect { get; set; }      //consequence from the object
        public Condition Condition { get; set; }//combinations of actions and objects can have conditions too
    }
    public class Word
    { 
        public string Term { get; set; }
    }
}
