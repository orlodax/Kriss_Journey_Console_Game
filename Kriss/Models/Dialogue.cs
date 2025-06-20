using System.Collections.Generic;

namespace KrissJourney.Kriss.Models;

public class DialogueLine
{
    public EnCharacter Actor { get; set; }       //who speaks
    public string LineName { get; set; }      //name of the speech part (used to link to this)
    public string NextLine { get; set; }    //if ever needed, name of the speech part to jump to
    public string PreComment { get; set; }
    public string Line { get; set; }
    public string Comment { get; set; }
    public List<Reply> Replies { get; set; }
    public int? ChildId { get; set; }
    public bool Break { get; set; }
    public bool IsTelepathy { get; set; }
}

public class Reply
{
    public string Line { get; set; }
    public int? ChildId { get; set; }
    public string NextLine { get; set; }    //speech part to jump to        
}