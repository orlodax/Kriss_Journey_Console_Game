using kriss.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace kriss.Classes
{
    public static class DataLayer
    {
        public static Status Status { get; set; }
        public static List<Chapter> Chapters { get; set; } = new();
        public static Chapter CurrentChapter { get; set; }

        public static bool IsReady { get; private set; }

        public static Dictionary<string, ConsoleColor> ActorsColors { get; private set; } = new Dictionary<string, ConsoleColor>();

        public static void Init()
        {
            if (!IsReady)
            {
                // Load Status
                var statusFile = Path.Combine(AppContext.BaseDirectory, "TextResources/status.json");
                if (File.Exists(statusFile))
                {
                    string json = File.ReadAllText(statusFile);
                    Status = Newtonsoft.Json.JsonConvert.DeserializeObject<Status>(json);

                    IsReady = true;
                }

                //color dialogues dictionary assignment
                ActorsColors.Add("Narrator", ConsoleColor.DarkCyan);
                ActorsColors.Add("Person", ConsoleColor.DarkYellow);
                ActorsColors.Add("White", ConsoleColor.White);
                ActorsColors.Add("Kriss", ConsoleColor.Cyan);
                ActorsColors.Add("Corolla", ConsoleColor.Red);
                ActorsColors.Add("Smiurl", ConsoleColor.Yellow);
                ActorsColors.Add("Theo", ConsoleColor.Blue);
                ActorsColors.Add("Jeorghe", ConsoleColor.DarkMagenta);
                ActorsColors.Add("Chief", ConsoleColor.Magenta);
                ActorsColors.Add("Efeliah", ConsoleColor.DarkGreen);
                ActorsColors.Add("Math", ConsoleColor.DarkMagenta);
                ActorsColors.Add("Elder", ConsoleColor.Magenta);

                //chapters titles
                //Titles.Add("1. THE JOURNEY STARTS");
                //Titles.Add("2. THE HORDE");
                //Titles.Add("3. SOME FRIENDS");
                //Titles.Add("4. THE SWORD");
                //Titles.Add("5. NOI-HERT");
                //Titles.Add("6. BEACONS");
                //Titles.Add("7. SEER'S ROCK");
                //Titles.Add("8. BREAKOUT");
                //Titles.Add("9. THEO'S GIFTS");
                //Titles.Add("10. INTO THE MAZEROCK");
                //Titles.Add("11. AYONN");
            }
        }

        public static void LoadChapter(int chapterId = 1)
        {
            var jChapter = Path.Combine(AppContext.BaseDirectory, $"TextResources/Chapters/c{chapterId}.json");
            if (File.Exists(jChapter))
            {
                string json = File.ReadAllText(jChapter);
                Chapters.Add(CurrentChapter = Newtonsoft.Json.JsonConvert.DeserializeObject<Chapter>(json));

                NodeFactory.BuildNode(SearchNodeById(1));
            }
        }

        /// <summary>
        /// Marks nodes as done, and if they are last of chapter, also chapter as done
        /// </summary>
        /// <param name="chapterNo"></param>
        public static void SaveProgress()
        {
            // save node status
            var chapterPath = Path.Combine(AppContext.BaseDirectory, $"/TextResources/Chapters/c{CurrentChapter.Id}.json");
            string jChapter = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentChapter, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(chapterPath, jChapter);

            // save chapter 
            if (NodeFactory.CurrentNode.IsLast)
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "/TextResources/status.json");
                Status.LastChapter = CurrentChapter.Id;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(Status, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);
            }
        }
       
        /// <summary>
        /// Parses the id provided to extract the matching NodeBase from DataLayer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static NodeBase SearchNodeById(int nodeId)
        {
            return CurrentChapter.Nodes.Find(n => n.Id == nodeId);
        }
    }



   
}
