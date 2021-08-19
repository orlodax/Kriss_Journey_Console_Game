namespace lybra;
public class Dialogue
{
    public EnActorColor Actor { get; set; }       //who speaks
    public string LineName { get; set; }      //name of the speechpart (used to link to this)
    public string NextLine { get; set; }    //if ever needed, name of the speechpart to jump to
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
    public string NextLine { get; set; }    //speechpart to jump to        
}