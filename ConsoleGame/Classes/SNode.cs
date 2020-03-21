using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ConsoleGame.Classes
{
    /// <summary>
    /// The main class. It represents every step of the story, every screen which will or won't have several types of player interactions
    /// </summary>
    public class SNode
    {
        //still don't know about this
        //internal List<string> Parents { get; set; } = new List<string>();
        // internal List<SNode> Children { get; set; } = new List<SNode>();

        internal string Text { get; set; } = string.Empty;

        #region TextFlow
        /// <summary>
        /// Mimics the flow of text of old console games. 
        /// </summary>

        internal int FlowDelay { get; set; } = 10; // fine-tunes the speed of TextFlow

        internal void TextFlow()
        {
            foreach (char c in Text)
            {
                Console.Write(c);
                Thread.Sleep(FlowDelay);
            }
        }
        /// <summary>
        /// Overload of the above, it displays custom text, not just the Text property in the SNode instance
        /// </summary>
        /// <param name="text"> guess what. the text to print</param>
        internal void TextFlow(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(FlowDelay);
            }
        }
        #endregion

        #region NextNodes
        /// <summary>
        /// Uses the nodeID provided from parent node to instantiate the matching node with that class name
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        internal SNode SelectNextNode(string nodeID) 
        {
            Type type = Type.GetType($"ConsoleGame.Nodes.N01.{nodeID}");
            
            return (SNode)Activator.CreateInstance(type);
        }
        #endregion

        #region CTOR
        /// <summary>
        /// At its creation, an instantiated node should clear the screen, print its text and prepare to receive player's input.
        /// This root node loads text resources for everybody

        public SNode()
        {
            //Console.Clear();
            //TextFlow();
            //PrepareForInput();
        }

        internal string FetchText(SNode node)
        {
            string[] typeWords = (node.ToString()).Split(".");
            string className = typeWords[typeWords.Length - 1];
            NodeBase nb = TextResource.DB.texts.Find(n => n.id == className);
            return Text = nb.text;
        }
        void PrepareForInput() 
        {
            ConsoleKeyInfo keyinfo;

            ///go to bottom line
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 2;
            Console.CursorLeft = Console.WindowLeft + Console.WindowWidth - 1;

            Console.Write(" \\> ");
            keyinfo = Console.ReadKey();

            if (keyinfo.Key.Equals(ConsoleKey.Tab))
            {
                Console.CursorLeft -= 5;
                Console.CursorTop -= 1;
                Console.WriteLine("Possible actions here: ");

                foreach (Enums.Actions action in (Enums.Actions[])Enum.GetValues(typeof(Enums.Actions)))
                    Console.Write(action + " ");

                Console.CursorTop += 1;
                Console.CursorLeft = 0;
                Console.WriteLine(" \\> you pressed tab for help. noob.");
            }
        }
        #endregion
    }
}
