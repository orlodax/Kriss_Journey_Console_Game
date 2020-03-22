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

            var filePath = Path.Combine(AppContext.BaseDirectory, "textResources.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                DB = JsonConvert.DeserializeObject<NodeContainer>(json);
            }

            //chapter I want to view
            int chapIndex = 0;
            List<NodeBase> nodes = DB.chapters[chapIndex];

            for (int i = 0; i < nodes.Count; i++)
            {
                List<Child> children = nodes[i].children;

                if (children != null)
                    for (int j = 0; j < children.Count; j++)
                        graph.AddEdge(nodes[i].id, children[j].id);
               
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
            public List<List<NodeBase>> chapters { get; set; }
        }
        public class NodeBase 
        {
            public NodeBase()
            {
            }

            public string id { get; set; }
            public string type { get; set; }
            public string text { get; set; }
            public List<Child> children { get; set; }

        }

        public class Child
        {
            public string id { get; set; }
        }
    }
}
