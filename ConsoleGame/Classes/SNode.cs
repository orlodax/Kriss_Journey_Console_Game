using lybra;
using System;
using System.Collections.Generic;
using System.Threading;

namespace kriss.Classes
{
    /// <summary>
    /// The main class. It represents every step of the story, every screen which will or won't have several types of player interactions
    /// </summary>
    public class SNode
    {
        #region Properties
        public int Id { get; set; }
        public string Text { get; set; }
        public string AltText { get; set; }
        public int ChildId { get; set; }
        public List<Choice> Choices { get; set; } //list of possible choices
        public List<lybra.Action> Actions { get; set; } = new List<lybra.Action>(); //list of possible actions
        public List<Dialogue> Dialogues { get; set; } //all the lines (thus paths) of the node's dialogues
        public bool IsVisited { get; set; }
        public bool IsLast { get; set; }

        readonly bool DEBUG;
        #endregion

        #region CTOR
        /// <summary>
        /// At its creation, an instantiated node should clear the screen, print its text and prepare to receive player's input.
        /// This root node loads text resources for everybody
        public SNode(NodeBase node)
        {
            // decomment to disable flow effect
            DEBUG = false;

            // mapping
            Id = node.Id;
            Text = node.Text;
            AltText = node.AltText;
            ChildId = node.ChildId;
            Choices = node.Choices;
            Actions = node.Actions;
            Dialogues = node.Dialogues;
            IsVisited = node.IsVisited;
            IsLast = node.IsLast;

            // start text
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan; //narrator, default color

            if (!IsVisited)     
                TextFlow(true);
            else
                TextFlow(false);
        }
        #endregion

        #region TextFlow
        /// <summary>
        /// Mimics the flow of text of old console games. 
        /// </summary>
        internal int FlowDelay { get; set; } = 20; // fine-tunes the speed of TextFlow
        internal int ParagraphBreak { get; set; } = 1000; // # arbitrary pause
        internal int ShortPause { get; set; } = 700; // comma pause
        internal int LongPause { get; set; } = 1200; // dot pause
        internal List<string> NotToPause = new List<string>(){ ".", "!", "?", "\"", ">", ")" }; // symbols after short pause that must not trigger another pause
        internal List<string> ToShortPause = new List<string>() { ":", ";", ",", "!", "?" }; // symbols after which trigger a short pause

        internal void TextFlow(bool isFlowing, string text = "default", ConsoleColor color = ConsoleColor.DarkCyan)
        {
            if (DEBUG)
            {
                isFlowing = false;
                ParagraphBreak = 0;
            }

            Console.ForegroundColor = color;

            if (text == "default")
            {
                if (IsVisited && AltText != null)
                    text = AltText;
                else
                    text = Text;
            }

            if (text != null)
            {
                int flow = FlowDelay;
                int paragraph = ParagraphBreak;
                int shortPause = ShortPause;
                int longPause = LongPause;

                if (!isFlowing)
                {
                    flow = 0;
                    paragraph = 0;
                    shortPause = 0;
                    longPause = 0;
                }
                
                char prevChar = new char();

                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    if (prevChar.ToString().Equals("."))
                    {
                        //if (!NotToPause.Contains(c.ToString()))
                        if (c.ToString().Equals(" "))
                            Thread.Sleep(longPause);
                    }
                    else
                    {
                        if (ToShortPause.Contains(prevChar.ToString()))
                            if (!NotToPause.Contains(c.ToString()))
                                Thread.Sleep(shortPause);
                    }

                    if (prevChar.ToString().Equals("$"))
                        switch (c.ToString())
                        {
                            case "R":
                                color = ConsoleColor.Red;           //Corolla
                                break;
                            case "r":
                                color = ConsoleColor.DarkRed;
                                break;
                            case "G":
                                color = ConsoleColor.Green;
                                break;
                            case "g":
                                color = ConsoleColor.DarkGreen;     //Efeliah
                                break;
                            case "B":
                                color = ConsoleColor.Blue;
                                break;
                            case "C":
                                color = ConsoleColor.DarkCyan;      //narrator
                                break;
                            case "c":
                                color = ConsoleColor.Cyan;          //yourself
                                break;
                            case "M":
                                color = ConsoleColor.Magenta;
                                break;
                            case "m":
                                color = ConsoleColor.DarkMagenta;   //Math
                                break;
                            case "Y":
                                color = ConsoleColor.Yellow;        //Smiurl
                                break;
                            case "y":
                                color = ConsoleColor.DarkYellow;    //Console answers
                                break;
                            case "K":
                                color = ConsoleColor.Black;
                                break;
                            case "W":
                                color = ConsoleColor.White;         //highlight
                                break;
                            case "S":
                                Console.BackgroundColor = ConsoleColor.White;
                                break;
                            case "s":
                                Console.BackgroundColor = ConsoleColor.Black;
                                break;
                            case "D":
                                color = ConsoleColor.DarkGray;      //menus, help
                                break;
                            case "d":
                                color = ConsoleColor.Gray;          //menus, help
                                break;
                            default:
                                break;
                        }
                    else
                    {
                        if (!c.ToString().Equals("#") && !c.ToString().Equals("$"))
                        {
                            Console.ForegroundColor = color;
                            Console.Write(c);
                            Thread.Sleep(flow);
                        }
                        else if (c.ToString().Equals("#"))
                        {
                            Thread.Sleep(paragraph);
                        }
                    }

                    prevChar = c;
                }

            }
        }
        #endregion

        #region Forwarding methods
        public static bool Evaluate(Condition condition)
        {
            return DataLayer.Evaluate(condition);
        }
        public static void StoreItem(Effect effect)
        {
            DataLayer.StoreItem(effect);
        }
        #endregion
    }
}