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
                var filePath = Path.Combine(AppContext.BaseDirectory, "Texts\\textResources.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    DB = Newtonsoft.Json.JsonConvert.DeserializeObject<NodeContainer>(json);
                }
            }
        }
    }
    public class NodeContainer
    {
        public List<NodeBase> texts { get; set; }
    }
    public class NodeBase
    {
        public string id { get; set; }
        public string text { get; set; }
    }   
}
