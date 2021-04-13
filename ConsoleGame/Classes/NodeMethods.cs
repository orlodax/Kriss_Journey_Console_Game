using lybra;
using System;
using System.Collections.Generic;
using System.Threading;

namespace kriss.Classes
{
    public static class NodeMethods
    {
        #region Extensions
        public static void Init(this NodeBase node)
        {
            // start text
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan; //narrator, default color

            string text;
            if (node.IsVisited && node.AltText != null)
                text = node.AltText;
            else
                text = node.Text;
            
            #if DEBUG
            node.IsVisited = true; 
            #endif

            TextFlow(!node.IsVisited, text);
        }

        public static void AdvanceToNext(this NodeBase node, int childId)
        {
            // mark as visited
            DataLayer.SaveProgress(node);

            // if it closes chapter load the next chapter, else load next node
            if (node.IsLast)
                DataLayer.StartChapter(DataLayer.CurrentChapter.Id + 1);

            DataLayer.LoadNode(childId);
        }
        #endregion

        #region Other methods
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
                    var storedItem = DataLayer.Status.Inventory.Find(i => i.Name == Condition.Item);
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
            DataLayer.Status.Inventory.Add(itemToStore);
        }
        #endregion

        #region TextFlow
        /// <summary>
        /// Mimics the flow of text of old console games. 
        /// </summary>
        static readonly int FlowDelay = 20; // fine-tunes the speed of TextFlow
        internal static readonly int ParagraphBreak = 1000; // # arbitrary pause
        static readonly int ShortPause = 700; // comma pause
        static readonly int LongPause = 1200; // dot pause
        static readonly List<string> NotToPause = new() { ".", "!", "?", "\"", ">", ")" }; // symbols after short pause that must not trigger another pause
        static readonly List<string> ToShortPause = new() { ":", ";", ",", "!", "?" }; // symbols after which trigger a short pause

        public static void TextFlow(bool isFlowing, string text, ConsoleColor color = ConsoleColor.DarkCyan)
        {
            Console.ForegroundColor = color;

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
    }
}
