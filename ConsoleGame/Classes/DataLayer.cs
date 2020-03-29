using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleGame.Classes
{
    public static class DataLayer
    {
        public static bool IsReady { get; private set; }
        public static Database DB { get; set; }

        public static Dictionary<string, ConsoleColor> ActorsColors { get; private set; }
        public static void Init()
        {
            if (!IsReady)
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "textResources.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    DB = Newtonsoft.Json.JsonConvert.DeserializeObject<Database>(json);

                    IsReady = true;
                }

                //color dialogues dictionary assignment
                ActorsColors.Add("Narrator", ConsoleColor.DarkCyan);
                ActorsColors.Add("Kriss", ConsoleColor.Cyan);
                ActorsColors.Add("Corolla", ConsoleColor.Red);
                ActorsColors.Add("Smiurl", ConsoleColor.Yellow);
                ActorsColors.Add("Theo", ConsoleColor.Blue);
                ActorsColors.Add("Jeorghe", ConsoleColor.DarkMagenta);
                ActorsColors.Add("Chief", ConsoleColor.Magenta);
            }
        }
        public static void SaveProgress(int chapterNo)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "textResources.json");

            if (chapterNo > 0)
            {
                DB.Lastchapter.Number = chapterNo;
                DB.Lastchapter.IsComplete = true;
            }
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(DB, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, output);
        }
    }
    public class Database
    {
        public ChapterBase Lastchapter { get; private set; } = new ChapterBase(); //for saving progress
        public List<List<NodeBase>> Chapters { get; set; } //all of the nodes grouped by chapter
        public List<Item> Inventory { get; set; } 
    }
    public class ChapterBase
    {
        public int Number { get; set; } = 0;
        public bool IsComplete { get; set; }
    }
    public class Item
    {
        public string Name { get; set; }
        public bool Had { get; set; }
    }
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
    public class Child
    {
        public string Id { get; set; }
    }
    public class Choice
    {
        public string Desc { get; set; }
        public string ChildId { get; set; }
    }
    public class Dialogue
    { 
        public string Actor { get; set; }
        public int LineId { get; set; }
        public string ChildId { get; set; }
        public string Line { get; set; }
        public List<Reply> Replies { get; set; }
    }
    public class Reply
    { 
        public string Line { get; set; }
        public string ChildId { get; set; }
        public int NextLine { get; set; }
    }

}
