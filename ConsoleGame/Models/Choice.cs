using kriss.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace kriss.Models
{
    public class Choice
    {
        public string Desc { get; set; }
        public int ChildId { get; set; }
        public Condition Condition { get; set; }//condition for the viability of the action. normally an item
        public Effect Effect { get; set; }      //consequence from the base action
        public string Refusal { get; set; }
        public bool IsHidden { get; set; }      //to unlock choices as effect of others
        public int? UnHide { get; set; }        //points to the id of the choice to unlock
        public bool IsNotRepeatable { get; set; }  //should this be hidden after playing it
        public bool IsPlayed { get; set; }  //should this be hidden after playing it

        public bool Evaluate()
        {
            if (Condition != null)
            {
                if (Condition.Type != "isNodeVisited")
                {
                    var storedItem = DataLayer.Status.Inventory.Find(i => i.Name == Condition.Item);
                    if (storedItem != null)
                    {
                        if (storedItem.Had & Condition.Value)
                            return true;
                    }
                    return false;
                }
            }
            return true;
        }
        public void StoreItem(Effect effect)       // consequent modify of inventory
        {
            var itemToStore = new Item() { Name = effect.Item, Had = effect.Value };
            DataLayer.Status.Inventory.Add(itemToStore);
        }
    }
}
