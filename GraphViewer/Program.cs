using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());


            //create a form 
            Form form = new Form();
            //create a viewer object 
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");







            NodeContainer DB = null;

            var filePath = Path.Combine(AppContext.BaseDirectory, "textResourcesG.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                DB = JsonConvert.DeserializeObject<NodeContainer>(json);
            }

            //chapter I want to view
            int chapIndex = 1;
            List<NodeBase> nodes = DB.chapters[chapIndex];

            for (int i = 0; i < nodes.Count; i++)
            {
                List<Child> children = nodes[i].children;

                if (children != null)
                    for (int j = 0; j < children.Count; j++)
                        graph.AddEdge(nodes[i].id, children[j].id);

                //var n = graph.FindNode(nodes[i].id);
                //n.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
            }

            

            ////create the graph content 
            //graph.AddEdge("A", "B");
            //graph.AddEdge("B", "C");
            //graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            //graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            //graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;
            //Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
            //c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            //c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
            //bind the graph to the viewer 
            viewer.Graph = graph;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            //show the form 
            form.ShowDialog();

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
        public class Action
        {
            private string _verb;

            public string verb { get => _verb; set { _verb = value; SetAnswer(); } } //verb of the action
            public string childid { get; set; } //key for matching next node
            public string defaultanswer { get; set; } //answer for incomplete player requests 
            public List<Object> objects { get; set; } = new List<Object>(); //objects for the verbs

            void SetAnswer()
            {
                if (verb != null && string.IsNullOrWhiteSpace(verb))
                    switch (verb)
                    {
                        case "look":
                            defaultanswer = "What shoud I look at? Where?";
                            break;
                        case "take":
                            defaultanswer = "What shoud I take?";
                            break;
                        case "go":
                            defaultanswer = "Where should I go?";
                            break;
                        case "search":
                            defaultanswer = "Where should I search? For what?";
                            break;
                        case "remove":
                            defaultanswer = "What will I remove? from where?";
                            break;
                        case "wear":
                            defaultanswer = "What could I wear?";
                            break;
                        case "rest":
                            defaultanswer = "Where could I lay down?";
                            break;
                        case "drink":
                            defaultanswer = "What will I drink?";
                            break;
                        case "eat":
                            defaultanswer = "What will I eat?";
                            break;
                    }
            }
        }
        public class Object
        {
            public string obj { get; set; } //object of the action
            public string answer { get; set; } //answer for incomplete player requests 
            public string childid { get; set; } //key for matching next node
        }
    }
}
