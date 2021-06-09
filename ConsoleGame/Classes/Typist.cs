using System;
using System.Collections.Generic;
using System.Threading;

namespace kriss.Classes
{
    public static class Typist
    {
        static readonly int FlowDelay = 10; // fine-tunes the speed of TextFlow
        static readonly int ParagraphBreak = 1000; // # arbitrary pause
        static readonly int ShortPause = 700; // comma pause
        static readonly int LongPause = 1200; // dot pause
        static readonly List<string> NotToPause = new() { ".", "!", "?", "\"", ">", ")", "]", "}", ":" }; // symbols after short pause that must not trigger another pause
        static readonly List<string> ToShortPause = new() { ":", ";", ",", "!", "?" }; // symbols after which trigger a short pause

        /// <summary>
        /// Main method to render different kinds of text
        /// </summary>
        /// <param name="isFlowing"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void RenderText(bool isFlowing, string text, ConsoleColor color = ConsoleColor.DarkCyan)
        {
            Console.ForegroundColor = color;

            if (text != null)
            {
                int flow = FlowDelay;
                int paragraph = ParagraphBreak;
                int shortPause = ShortPause;
                int longPause = LongPause;

                if (IsDebug())
                    isFlowing = false;

                if (!isFlowing)
                    flow = paragraph = shortPause = longPause = 0;

                char prevChar = new();

                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    if (prevChar.ToString().Equals("."))
                    {
                        if (c.ToString().Equals(" ") || c.ToString().Equals("\n"))
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
                                color = ConsoleColor.Blue;          //Theo
                                break;
                            case "C":
                                color = ConsoleColor.DarkCyan;      //narrator
                                break;
                            case "c":
                                color = ConsoleColor.Cyan;          //Kriss
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
                            case "D":
                                color = ConsoleColor.DarkGray;      //menus, help
                                break;
                            case "d":
                                color = ConsoleColor.Gray;          //menus, help
                                break;
                            case "S":
                                Console.BackgroundColor = ConsoleColor.White;
                                break;
                            case "s":
                                Console.BackgroundColor = ConsoleColor.Black;
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

        public static void FlowingText(string text, ConsoleColor color = ConsoleColor.DarkCyan)
        {
            RenderText(true, text, color);
        }
        public static void InstantText(string text, ConsoleColor color = ConsoleColor.DarkCyan)
        {
            RenderText(false, text, color);
        }

        /// <summary>
        /// To display characters speech parts
        /// </summary>
        /// <param name="isFlowing"></param>
        /// <param name="line"></param>
        /// <param name="actorColor"></param>
        /// <param name="isTelepathy"></param>
        public static void RenderLine(bool isFlowing, string line, ConsoleColor actorColor, bool isTelepathy)
        {
            if (isTelepathy)
                RenderText(isFlowing, "<<" + line + ">>", actorColor);
            else
                RenderText(isFlowing, "\"" + line + "\"", actorColor);

            if(!IsDebug())
                Thread.Sleep(ParagraphBreak);
        }
        public static void RenderNonSpeechPart(bool isFlowing, string part)
        {
            RenderText(isFlowing, part);

            if (!IsDebug())
                Thread.Sleep(ParagraphBreak);
        }

        static bool IsDebug()
        {
            bool isDebug = false;
#if DEBUG
            isDebug = true;
#endif
            return isDebug;
        }
    }
}
