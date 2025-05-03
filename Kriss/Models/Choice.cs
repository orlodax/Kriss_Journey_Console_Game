
namespace KrissJourney.Models; // Kriss Journey - Lybra - Condition.cs

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
}