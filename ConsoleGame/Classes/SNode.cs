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
        internal List<Child> Children {get;set;}
        internal List<Choice> Choices { get; set; }
        internal List<Action> Actions { get; set; }
        internal List<Object> Objects { get; set; }
        #endregion

        #region CTOR
        /// <summary>
        /// At its creation, an instantiated node should clear the screen, print its text and prepare to receive player's input.
        /// This root node loads text resources for everybody
        public SNode(NodeBase nb)
        {
            Console.Clear();
            ID = nb.id;
            Text = nb.text;
            Children = nb.children;
            Choices = nb.choices;
            Actions = nb.actions;
            Objects = nb.objects;
            
            //TextFlow(true);
            //in debug turn off the effect:
            TextFlow(false);
        }
        #endregion

        #region TextFlow
        /// <summary>
        /// Mimics the flow of text of old console games. 
        /// </summary>

        internal int flowDelay { get; set; } = 15; // fine-tunes the speed of TextFlow
        internal int paragraphBreak { get; set; } = 800; // fine-tunes the pause between blocks

        internal void TextFlow(bool isFlowing)
        {
            if (!isFlowing)
            {
                flowDelay = 0;
                paragraphBreak = 0;
            }
            char prevChar = new Char();

            foreach (char c in Text)
            {
                if (prevChar.ToString().Equals("$"))
                    switch (c.ToString())
                    {
                        case "R":
                            Console.ForegroundColor = ConsoleColor.Red; //Corolla
                            break;
                        case "G":
                            Console.ForegroundColor = ConsoleColor.Green; 
                            break;
                        case "B":
                            Console.ForegroundColor = ConsoleColor.Blue; //Theo
                            break;
                        case "C":
                            Console.ForegroundColor = ConsoleColor.DarkCyan; //narrator
                            break;
                        case "c":
                            Console.ForegroundColor = ConsoleColor.Cyan; //yourself
                            break;
                        case "M":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case "Y":
                            Console.ForegroundColor = ConsoleColor.Yellow; //Smiurl
                            break;
                        case "K":
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case "W":
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case "S":
                            Console.BackgroundColor = ConsoleColor.White;
                            break;
                        case "D":
                            Console.ForegroundColor = ConsoleColor.DarkGray; //menus, help
                            break;
                        default:
                            break;
                    }
                else
                {
                    if (!c.ToString().Equals("#") && !c.ToString().Equals("$"))
                    {
                        Console.Write(c);
                        Thread.Sleep(flowDelay);
                    }
                    else
                    {
                        Thread.Sleep(paragraphBreak);
                    }
                }
                prevChar = c;
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
                //Thread.Sleep(flowDelay);
            }
        }
        #endregion
    }
}