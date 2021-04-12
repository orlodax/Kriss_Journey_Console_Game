using kriss.Nodes;
using lybra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            if (Status.VisitedNodes == null)
                Status.VisitedNodes = new Dictionary<int, List<int>>();

            //CurrentChapter = Chapters.Find(c => c.Id == Status.LastChapter);
            if (Status.VisitedNodes.Any())
                CurrentChapter = Chapters.Find(c => c.Id == Status.VisitedNodes.Keys.Max());
            else
                CurrentChapter = Chapters[0];

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
        public static void StartChapter(int chapterId)
        {
            CurrentChapter = Chapters.Find(c => c.Id == chapterId);

            LoadNode(1);
        }

        /// <summary>
        /// Find next node and uses node factory to build the proper type
        /// </summary>
        /// <param name="nodeId"></param>
        public static void LoadNode(int? nodeId)
        {
            if (nodeId.HasValue)
            {
                var newNode = SearchNodeById(nodeId.Value);
                newNode.IsVisited = IsNodeVisited(newNode.Id);

                BuildNode(newNode);
            }
            else
                Console.WriteLine("Id was null and/or node wasn't the last in the chapter!");
        }

        /// <summary>
        /// Extract the NodeBase with given id, from DataLayer
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static NodeBase SearchNodeById(int nodeId)
        {
            return CurrentChapter.Nodes.Find(n => n.Id == nodeId);
        }

        /// <summary>
        /// Actually build (hence display) the node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static void BuildNode(NodeBase node)
        {
            if (node != null)
            {
                switch (node.Type)
                {
                    case "Story":
                        Construct<NodeBase, NStory>(node).Activate();
                        break;
                    case "Choice":
                        Construct<NodeBase, NChoice>(node).Activate();
                        break;
                    case "Dialogue":
                        Construct<NodeBase, NDialogue>(node).Activate();
                        break;
                    case "Action":
                        Construct<NodeBase, NAction>(node).Activate();
                        break;
                    case "MiniGame01":
                        NAction actionNode = Construct<NodeBase, NAction>(node);
                        Construct<NAction, MiniGame01>(actionNode).Activate();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Marks nodes as done, and if they are last of chapter, also chapter as done
        /// </summary>
        public static void SaveProgress(SNode currentNode)
        {
            if (Status.VisitedNodes.TryGetValue(CurrentChapter.Id, out List<int> visitedNodes))
            {
                if (!visitedNodes.Contains(currentNode.Id))
                {
                    visitedNodes.Add(currentNode.Id);
                    Status.VisitedNodes[CurrentChapter.Id] = visitedNodes;
                }
            }
            else
                Status.VisitedNodes[CurrentChapter.Id] = new List<int>() { currentNode.Id };

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
            if (Status.VisitedNodes.TryGetValue(CurrentChapter.Id, out List<int> visitedNodes))
                if (visitedNodes.Contains(nodeId))
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
        /// construct a derived class of from a base class
        /// </summary>
        /// <typeparam name="F">type of base class</typeparam>
        /// <typeparam name="T">type of class you want</typeparam>
        /// <param name="Base">the instance of the base class</param>
        /// <returns></returns>
        public static T Construct<F, T>(F Base) where T : F, new()
        {
            // create derived instance
            T derived = new();
            // get all base class properties
            PropertyInfo[] properties = typeof(F).GetProperties();
            foreach (PropertyInfo bp in properties)
            {
                // get derived matching property
                PropertyInfo dp = typeof(T).GetProperty(bp.Name, bp.PropertyType);

                // this property must not be index property
                if (
                    (dp != null)
                    && (dp.GetSetMethod() != null)
                    && (bp.GetIndexParameters().Length == 0)
                    && (dp.GetIndexParameters().Length == 0)
                )
                    dp.SetValue(derived, dp.GetValue(Base, null), null);
            }

            return derived;
        }
    }
}
