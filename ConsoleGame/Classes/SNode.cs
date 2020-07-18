using ConsoleGame.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleGame.Classes
{
    /// <summary>
    /// The main class. It represents every step of the story, every screen which will or won't have several types of player interactions
    /// </summary>
    public class SNode
    {
        #region Properties
        internal string ID { get; set; }
        internal string Text { get; set; } = string.Empty;
        internal string AltText { get; set; } = string.Empty;
        internal string ChildId { get; set; }
        internal List<Choice> Choices { get; set; }
        internal List<Models.Action> Actions { get; set; }
        internal List<Dialogue> Dialogues { get; set; }

        readonly NodeBase node;
        readonly bool DEBUG;
        #endregion

        #region CTOR
        /// <summary>
        /// At its creation, an instantiated node should clear the screen, print its text and prepare to receive player's input.
        /// This root node loads text resources for everybody
        public SNode(NodeBase nb)
        {
            //decomment to disable flow effect
            DEBUG = true;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan; //narrator, default color

            node = nb;

            ID = node.Id;
            Text = node.Text;
            AltText = node.AltText;
            ChildId = node.ChildId;
            Choices = node.Choices;
            Actions = node.Actions;
            Dialogues = node.Dialogues;

            if (!node.IsVisited)     
                TextFlow(true);
            else
                TextFlow(false);
        }
        internal void SaveStatusOnExit()  //this to be called on exit from node to mark it as not new
        {
            string[] number = node.Id.Split("_");
            var updatingNode = DataLayer.DB.Chapters[Convert.ToInt32(number[0])].Find(n => n.Id == node.Id);
            updatingNode.IsVisited = true;
            DataLayer.SaveProgress(0);
        }
        #endregion

        #region TextFlow
        /// <summary>
        /// Mimics the flow of text of old console games. 
        /// </summary>
        internal int FlowDelay { get; set; } = 30; // fine-tunes the speed of TextFlow
        internal int ParagraphBreak { get; set; } = 1000; // # arbitrary pause
        internal int ShortPause { get; set; } = 700; // comma pause
        internal int LongPause { get; set; } = 1200; // dot pause
        internal List<string> NotToPause = new List<string>(){ ".", "!", "?", "\"", ">" }; // symbols after . that must not trigger a pause
        internal List<string> ToShortPause = new List<string>() { ":", ";", "," }; // symbols after which trigger a short pause

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
                if (node.IsVisited && AltText != null)
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
                
                char prevChar = new Char();

                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    if (prevChar.ToString().Equals("."))
                        if (!NotToPause.Contains(c.ToString()))
                            Thread.Sleep(longPause);

                    if (ToShortPause.Contains(prevChar.ToString()))
                        Thread.Sleep(shortPause);

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
    }
}