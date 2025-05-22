using System.Collections.Generic;

namespace KrissJourney.Kriss.Models;

public interface IAction
{
    Condition Condition { get; set; }
    Effect Effect { get; set; }
    string Answer { get; set; }
    int? ChildId { get; set; }
}

public class Action : IAction
{
    public List<string> Verbs { get; set; } // verb of the action
    public int? ChildId { get; set; } // key for matching next node
    public string Answer { get; set; } // answer for incomplete player requests 
    public List<ActionObject> Objects { get; set; } = []; // objects for the verbs
    public Condition Condition { get; set; } // condition for the viability of the action. normally an item
    public Effect Effect { get; set; } // consequence from the base action
    public string GetAnswer(string word) // to get response message when action requires an object and player does not provide any valid
    {
        if (Answer != null)
            return Answer;
        else
            return GetHelpObject(word);
    }

    public static string GetHelpObject(string word) // to get response message when action requires an object and player does not provide any valid
    {
        return word switch
        {
            "look" => "What shoud I look at? Where?",
            "take" => "What shoud I take?",
            "go" => "Where should I go?",
            "search" => "Where should I search? For what?",
            "remove" => "What will I remove? from where?",
            "wear" => "What could I wear?",
            "rest" => "Where could I lay down?",
            "drink" => "What will I drink?",
            "eat" => "What will I eat?",
            "sleep" => "Where can I sleep?",
            "say" => "What could I say?",
            "ask" => "What will I ask?",
            "give" => "What will I give?",
            "use" => "What will I use?",
            "read" => "What will I read?",
            "write" => "What will I write?",
            "open" => "What will I open?",
            "close" => "What will I close?",
            "unlock" => "What will I unlock?",
            "lock" => "What will I lock?",
            "turn" => "What will I turn?",
            "push" => "What will I push?",
            "pull" => "What will I pull?",
            "hit" => "What will I hit?",
            "kick" => "What will I kick?",
            "throw" => "What will I throw?",
            "catch" => "What will I catch?",
            "climb" => "Where will I climb?",
            "jump" => "Where will I jump?",
            "swim" => "Where will I swim?",
            _ => "Uhm... I don't know about that...",
        };
    }
}

public class ActionObject : IAction
{
    public List<string> Objs { get; set; }  //objects of the action
    public string Answer { get; set; }      //answer for incomplete player requests 
    public int? ChildId { get; set; }       //key for matching next node
    public Effect Effect { get; set; }      //consequence from the object
    public Condition Condition { get; set; }//combinations of actions and objects can have conditions too
}