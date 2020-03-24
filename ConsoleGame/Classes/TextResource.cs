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

            DB.lastchapter.number = chapterNo;
            DB.lastchapter.iscomplete = true;

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(DB, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, output);
        }
    }

    public class NodeContainer
    {
        public ChapterBase lastchapter { get; private set; } = new ChapterBase();
        public List<List<NodeBase>> chapters { get; set; }
    }
    public class ChapterBase
    {
        public int number { get; set; } = 0;
        public bool iscomplete { get; set; } = false;
    }
    public class NodeBase
    {
        public string id { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public List<Child> children { get; set; }
        public List<Choice> choices { get; set; }
        public List<Action> actions { get; set; } = new List<Action>();
        public List<Object> objects { get; set; } = new List<Object>();
    }

    public class Child
    {
        public string id { get; set; }
    }
    public class Choice
    {
        public string desc { get; set; }
    }
    public class Action
    { 
        public string verb { get; set; }
    }
    public class Object
    {
        public string obj { get; set; }
    }
}
