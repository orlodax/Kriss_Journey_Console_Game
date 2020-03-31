using ConsoleGame.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleGame.Classes
{
    public static class DataLayer
    {
        public static bool IsReady { get; private set; }
        public static Database DB { get; set; }

        public static Dictionary<string, ConsoleColor> ActorsColors { get; private set; } = new Dictionary<string, ConsoleColor>();
        public static List<string> Titles { get; private set; } = new List<string>();

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
                //chapters titles
                Titles.Add("1. THE JOURNEY STARTS");
                Titles.Add("2. THE HORDE");
                Titles.Add("3. SOME FRIENDS");
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
    
}
