using lybra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace kriss.Classes
{
    public static class DataLayer
    {
        public static Status Status { get; set; }
        public static List<Chapter> Chapters { get; set; } = new();
        public static Chapter CurrentChapter { get; set; }
        public static Dictionary<string, ConsoleColor> ActorsColors { get; private set; } = new Dictionary<string, ConsoleColor>();

        private static Assembly Assembly;

        public static void Init()
        {
            Assembly = Assembly.GetExecutingAssembly();

            // Load Status
            var statusFile = Path.Combine(AppContext.BaseDirectory, "TextResources/status.json");
            if (File.Exists(statusFile))
            {
                string json = File.ReadAllText(statusFile);
                Status = Newtonsoft.Json.JsonConvert.DeserializeObject<Status>(json);
            }

            // Load all Chapters
            int id = 1;
            do
            {
                string jChapter = LoadResource($"kriss.TextResources.Chapters.c{id}.json");
                
                if (string.IsNullOrEmpty(jChapter))
                    break;
                
                Chapters.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Chapter>(jChapter));
                id++;
            }
            while (true);

            CurrentChapter = Chapters.Find(c => c.Id == Status.LastChapter);
       

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

        /// <summary>
        /// Starts the n-th chapter at the beginning (first node)
        /// </summary>
        /// <param name="chapterId" the chapter number></param>
        /// <returns></returns>
        public static SNode StartChapter(int chapterId)
        {
            CurrentChapter = Chapters.Find(c => c.Id == chapterId);

            Status.VisitedNodes.Add(new VisitedNode() { ChapterId = CurrentChapter.Id, Nodes = new List<int>() });

            return NodeFactory.LoadNode(1);
        }

        /// <summary>
        /// Marks nodes as done, and if they are last of chapter, also chapter as done
        /// </summary>
        public static void SaveProgress(SNode currentNode)
        {
            // save chapter status
            if (currentNode.IsLast)
                Status.LastChapter = CurrentChapter.Id;

            // save node status
            var historyChapter = Status.VisitedNodes.Find(c => c.ChapterId == CurrentChapter.Id);
            historyChapter.Nodes.Add(currentNode.Id);

            var statusPath = Path.Combine(AppContext.BaseDirectory, $"TextResources/status.json");
            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Status, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(statusPath, status);
        }

        /// <summary>
        /// Check if selected node was visited in the past
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static bool IsNodeVisited(int nodeId)
        {
            var historyChapter = Status.VisitedNodes.Find(c => c.ChapterId == CurrentChapter.Id);
            if (historyChapter != null && historyChapter.Nodes.Contains(nodeId))
                return true;

            return false;
        }

        /// <summary>
        /// Extracts resources (chapters) from the compiled DLL
        /// </summary>
        /// <param name="resourceName" name-path of the resource></param>
        /// <returns></returns>
        static string LoadResource(string resourceName)
        {
            using Stream stream = Assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using StreamReader reader = new(stream);
                return reader.ReadToEnd();
            }
            return string.Empty;
        }

        /// <summary>
        /// Decides upon the condition of a choice, action, object etc
        /// </summary>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public static bool Evaluate(Condition Condition)                            // check according to the condition
        {
            if (Condition != null)
            {
                if (Condition.Type != "isNodeVisited")
                {
                    var storedItem = Status.Inventory.Find(i => i.Name == Condition.Item);
                    if (storedItem != null)
                    {
                        if (storedItem.Had & Condition.Value)
                            return true;
                    }
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// You picked something up
        /// </summary>
        /// <param name="effect"></param>
        public static void StoreItem(Effect effect)       // consequent modify of inventory
        {
            var itemToStore = new Item() { Name = effect.Item, Had = effect.Value };
            Status.Inventory.Add(itemToStore);
        }
    }
}
