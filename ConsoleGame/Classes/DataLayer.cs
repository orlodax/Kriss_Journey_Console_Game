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
        public static List<Chapter> Chapters { get; private set; } = new();

        static Status Status;
        static Chapter CurrentChapter;
        static NodeBase CurrentNode;

        public static void Init()
        {
            // Dump any exception
            AppDomain.CurrentDomain.UnhandledException += LogError;

            // Load Status
            string statusFile = Path.Combine(AppContext.BaseDirectory, "status.json");
            if (File.Exists(statusFile))
                Status = Newtonsoft.Json.JsonConvert.DeserializeObject<Status>(File.ReadAllText(statusFile));
            else
                WriteStatusToDisk(); // init file if not present

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

            //debug: start from. Comment for default start
            //CurrentChapter = Chapters[7];
            //LoadNode(9);
            //debug

            if (!Console.IsOutputRedirected)
                DisplayMenu();
        }

        /// <summary>
        /// First screen in game. Comes back to this after completing the story or a section
        /// </summary>
        public static void DisplayMenu()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("            ██╗  ██╗██████╗ ██╗███████╗███████╗              ");
            Console.WriteLine("            ██║ ██╔╝██╔══██╗██║██╔════╝██╔════╝              ");
            Console.WriteLine("            █████╔╝ ██████╔╝██║███████╗███████╗              ");
            Console.WriteLine("            ██╔═██╗ ██╔══██╗██║╚════██║╚════██║              ");
            Console.WriteLine("            ██║  ██╗██║  ██║██║███████║███████║              ");
            Console.WriteLine("            ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝╚══════╝╚══════╝              ");
            Console.WriteLine("     ██╗ ██████╗ ██╗   ██╗██████╗ ███╗   ██╗███████╗██╗   ██╗");
            Console.WriteLine("     ██║██╔═══██╗██║   ██║██╔══██╗████╗  ██║██╔════╝╚██╗ ██╔╝");
            Console.WriteLine("     ██║██║   ██║██║   ██║██████╔╝██╔██╗ ██║█████╗   ╚████╔╝ ");
            Console.WriteLine("██   ██║██║   ██║██║   ██║██╔══██╗██║╚██╗██║██╔══╝    ╚██╔╝  ");
            Console.WriteLine("╚█████╔╝╚██████╔╝╚██████╔╝██║  ██║██║ ╚████║███████╗   ██║   ");
            Console.WriteLine(" ╚════╝  ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝╚══════╝   ╚═╝   ");
            Console.WriteLine();
            Console.WriteLine("              />_________________________________");
            Console.WriteLine("     [########[]_________________________________>");
            Console.WriteLine("              \\>");
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine();

            int chapterId = 1;

            if (Status.VisitedNodes.Any())
            {
                Console.WriteLine("Welcome back, traveler. This is your journey so far.");
                Console.WriteLine("This game still features autosave, at least for now.");
                Console.WriteLine("Type a number and press enter to select a chapter.");
                Console.WriteLine();

                int lastChapter = Status.VisitedNodes.Keys.Max();

                for (int i = 0; i < lastChapter; i++)
                    Console.WriteLine(i + 1 + ". " + Chapters[i].Title);

                Console.WriteLine();

                bool isValid = false;

                do
                {
                    if (int.TryParse(Console.ReadLine(), out int digit))
                        if (isValid = digit <= lastChapter)
                            chapterId = digit;
                }
                while (!isValid);
            }
            else
            {
                Console.WriteLine("Welcome traveler. Your journey is yet to be started.");
                Console.WriteLine("This game features autosave. You just won't know when.");
                Console.WriteLine();
                Console.WriteLine("Press any key.");
                Console.ReadKey(true);
            }

            // load first node of selected chapter
            StartChapter(chapterId);
        }

        /// <summary>
        /// Starts the n-th chapter at the beginning (first node)
        /// </summary>
        /// <param name="chapterId" the chapter number></param>
        /// <returns></returns>
        static void StartChapter(int chapterId)
        {
            CurrentChapter = Chapters.Find(c => c.Id == chapterId);

            //to save chapter progress without marking any node as visited
            Status.VisitedNodes[CurrentChapter.Id] = new List<int>();
            WriteStatusToDisk();

            LoadNode(1);
        }
        public static void StartNextChapter()
        {
            StartChapter(CurrentChapter.Id + 1);
        }

        /// <summary>
        /// Find next node and uses node factory to build the proper type
        /// </summary>
        /// <param name="nodeId"></param>
        public static void LoadNode(int? nodeId)
        {
            if (nodeId.HasValue)
            {
                CurrentNode = SearchNodeById(nodeId.Value);
                CurrentNode.IsVisited = IsNodeVisited(CurrentNode.Id);

                BuildNode(CurrentNode);
            }
            else
                throw new Exception("Id was null and/or node wasn't the last in the chapter!");
        }

        /// <summary>
        /// Extract the NodeBase with given id, from DataLayer
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        static NodeBase SearchNodeById(int nodeId)
        {
            return CurrentChapter.Nodes.Find(n => n.Id == nodeId);
        }

        /// <summary>
        /// Actually build (hence display) the node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static NodeBase BuildNode(NodeBase node)
        {
            if (node != null)
            {
                return node.Type switch
                {
                    "Story" => new NStory(node),
                    "Choice" => new NChoice(node),
                    "Dialogue" => new NDialogue(node),
                    "Action" => new NAction(node),
                    "MiniGame01" => new MiniGame01(node),
                    _ => throw new Exception(node.Type + " node type does not exist.")
                };
            }
            return null;
        }

        /// <summary>
        /// Marks nodes as done, and if they are last of chapter, also chapter as done
        /// </summary>
        public static void SaveProgress()
        {
            if (!Status.VisitedNodes.ContainsKey(CurrentChapter.Id))
                Status.VisitedNodes[CurrentChapter.Id] = new List<int>();

            if (Status.VisitedNodes.TryGetValue(CurrentChapter.Id, out List<int> visitedNodes))
            {
                if (!visitedNodes.Contains(CurrentNode.Id))
                {
                    visitedNodes.Add(CurrentNode.Id);
                    Status.VisitedNodes[CurrentChapter.Id] = visitedNodes;
                }
            }

            WriteStatusToDisk();
        }

        static void WriteStatusToDisk()
        {
            if (Status == null)
                Status = new();

            string statusPath = Path.Combine(AppContext.BaseDirectory, $"status.json");
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
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
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
                return Condition.Type switch
                {
                    "isNodeVisited" => IsNodeVisited(Convert.ToInt32(Condition.Item)),
                    _ => Status.Inventory.Contains(Condition.Item),
                };
            }
            return true;
        }

        /// <summary>
        /// You picked something up
        /// </summary>
        /// <param name="effect"></param>
        public static void StoreItem(Effect effect)       // consequent modify of inventory
        {
            Status.Inventory.Add(effect.GainItem);
        }

        /// <summary>
        /// Dump any unhandled exception to txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void LogError(object sender, UnhandledExceptionEventArgs e)
        {
            string path = Path.Combine(AppContext.BaseDirectory, $"errorLog.txt");

            System.Text.StringBuilder sb = new();
            sb.AppendLine(e.ExceptionObject.ToString());

            if (CurrentChapter != null)
                sb.AppendLine("Chapter: " + CurrentChapter.Id);

            if (CurrentNode != null)
                sb.AppendLine("Node: " + CurrentNode.Id);

            File.WriteAllText(path, sb.ToString());
        }

        #region Jokes
        public static bool CheckChap2Node2()
        {
            //first action node. to mock player just the first time they use help
            if (CurrentChapter.Id == 2 && CurrentNode.Id == 2)
                return true;

            return false;
        }
        #endregion
    }
}
