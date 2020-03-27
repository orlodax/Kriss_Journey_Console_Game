using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleGame.Classes
{
    public static class TextResource
    {
        public static bool IsReady { get; private set; }
        public static NodeContainer DB { get; set; }
        public static void Init()
        {
            if (!IsReady)
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "textResources.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    DB = Newtonsoft.Json.JsonConvert.DeserializeObject<NodeContainer>(json);

                    IsReady = true;
                }
            }
        }
        public static void SaveProgress(int chapterNo)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "textResources.json");

            if (chapterNo > 0)
            {
                DB.lastchapter.number = chapterNo;
                DB.lastchapter.iscomplete = true;
            }
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(DB, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, output);
        }
    }
    public class NodeContainer
    {
        public ChapterBase lastchapter { get; private set; } = new ChapterBase(); //for saving progress
        public List<List<NodeBase>> chapters { get; set; } //all of the nodes grouped by chapter
        public List<Item> inventory { get; set; } 
    }
    public class ChapterBase
    {
        public int number { get; set; } = 0;
        public bool iscomplete { get; set; }
    }
    public class Item
    {
        public string name { get; set; }
        public bool had { get; set; }
    }
    public class NodeBase
    {
        public string id { get; set; } //unique id primary key
        public string type { get; set; } //story, choice, action...
        public string text { get; set; } //text to be flown
        public List<Child> children { get; set; } //next possible nodes
        public List<Choice> choices { get; set; } //list of possible choices
        public List<Action> actions { get; set; } = new List<Action>(); //list of possible actions

        public bool isvisited { get; set; }
    }
    public class Child
    {
        public string id { get; set; }
    }
    public class Choice
    {
        public string desc { get; set; }
    }
    public class Object
    {
        public string obj { get; set; } //object of the action
        public string answer { get; set; } //answer for incomplete player requests 
        public string childid { get; set; } //key for matching next node
    }
}
