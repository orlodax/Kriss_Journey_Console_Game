namespace kriss.Models
{
    public class Condition                      // condition for the viability of the action. normally an item
    {
        public string Type { get; set; }        // is it an item? a previous node to be visited?
        public string Item { get; set; }        // name of the resource
        public bool Value { get; set; }         // value of the resource
        public string Refusal { get; set; }     // message for condition not met
    }
}
