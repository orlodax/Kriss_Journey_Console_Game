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
    }

    public class NodeContainer
    {
        public List<List<NodeBase>> chapters { get; set; }
    }
    public class NodeBase
    {
        public string id { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public List<Child> children { get; set; }
        public List<Choice> choices { get; set; }
    }

    public class Child
    {
        public string id { get; set; }
    }
    public class Choice
    {
        public string desc { get; set; }
    }

}
