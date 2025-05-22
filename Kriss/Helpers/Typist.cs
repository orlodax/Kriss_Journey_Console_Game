using System;
using System.Collections.Generic;
using System.Threading;
using KrissJourney.Kriss.Models;

namespace KrissJourney.Kriss.Helpers;

public static class Typist
{
    static readonly int FlowDelay = 10; // fine-tunes the speed of TextFlow
    static readonly int ParagraphBreak = 1000; // "#" arbitrary pause
    static readonly int ShortPause = 700; // comma pause
    static readonly int LongPause = 1200; // dot pause
    static readonly List<string> NotToPause = [".", "!", "?", "\"", ">", ")", "]", "}", ":"]; // symbols after short pause that must not trigger another pause
    static readonly List<string> ToShortPause = [":", ";", ",", "!", "?"]; // symbols after which trigger a short pause

    // Steam Deck / xterm color compatibility
    static readonly bool UseHighContrast = DetectHighContrastMode();
    static readonly bool DebugColors = Environment.GetEnvironmentVariable("KRISS_DEBUG_COLORS") == "true";

    // Color remapping for high contrast mode
    static readonly Dictionary<ConsoleColor, ConsoleColor> HighContrastMap = new()
    {
        { ConsoleColor.DarkGray, ConsoleColor.Gray },
        { ConsoleColor.DarkBlue, ConsoleColor.Blue },
        { ConsoleColor.DarkCyan, ConsoleColor.Cyan },
        { ConsoleColor.DarkGreen, ConsoleColor.Green },
        { ConsoleColor.DarkMagenta, ConsoleColor.Magenta },
        { ConsoleColor.DarkRed, ConsoleColor.Red },
        { ConsoleColor.DarkYellow, ConsoleColor.Yellow },
        { ConsoleColor.Black, ConsoleColor.DarkGray }
    };

    /// <summary>
    /// Main method to render different kinds of text
    /// </summary>
    /// <param name="isFlowing"></param>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public static void RenderText(bool isFlowing, string text, ConsoleColor color = ConsoleColor.DarkCyan)
    {
        // Apply high contrast mapping if needed
        color = GetMappedColor(color);

        ForegroundColor = color;

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

            string prevChar = string.Empty;

            for (int i = 0; i < text.Length; i++)
            {
                string c = text[i].ToString(); ;

                if (prevChar.Equals("."))
                {
                    if (c.Equals(" ") || c.Equals("\n"))
                        Thread.Sleep(longPause);
                }
                else
                {
                    if (ToShortPause.Contains(prevChar))
                        if (!NotToPause.Contains(c))
                            Thread.Sleep(shortPause);
                }

                if (prevChar.Equals("$"))
                    switch (c)
                    {
                        case "R":
                            color = GetMappedColor(ConsoleColor.Red);           //Corolla
                            break;
                        case "r":
                            color = GetMappedColor(ConsoleColor.DarkRed);
                            break;
                        case "G":
                            color = GetMappedColor(ConsoleColor.Green);
                            break;
                        case "g":
                            color = GetMappedColor(ConsoleColor.DarkGreen);     //Efeliah
                            break;
                        case "B":
                            color = GetMappedColor(ConsoleColor.Blue);          //Theo
                            break;
                        case "C":
                            color = GetMappedColor(ConsoleColor.DarkCyan);      //narrator
                            break;
                        case "c":
                            color = GetMappedColor(ConsoleColor.Cyan);          //Kriss
                            break;
                        case "M":
                            color = GetMappedColor(ConsoleColor.Magenta);
                            break;
                        case "m":
                            color = GetMappedColor(ConsoleColor.DarkMagenta);   //Math
                            break;
                        case "Y":
                            color = GetMappedColor(ConsoleColor.Yellow);        //Smiurl
                            break;
                        case "y":
                            color = GetMappedColor(ConsoleColor.DarkYellow);    //Console answers
                            break;
                        case "K":
                            color = GetMappedColor(ConsoleColor.Black);
                            break;
                        case "W":
                            color = GetMappedColor(ConsoleColor.White);         //highlight
                            break;
                        case "D":
                            color = GetMappedColor(ConsoleColor.DarkGray);      //menus, help
                            break;
                        case "d":
                            color = GetMappedColor(ConsoleColor.Gray);          //menus, help
                            break;
                        case "S":
                            if (!UseHighContrast) // Skip background changes in high contrast mode
                                BackgroundColor = ConsoleColor.White;
                            break;
                        case "s":
                            if (!UseHighContrast) // Skip background changes in high contrast mode
                                BackgroundColor = ConsoleColor.Black;
                            break;
                        default:
                            break;
                    }
                else
                {
                    if (!c.Equals("#") && !c.Equals("$"))
                    {
                        ForegroundColor = color;
                        Write(c);
                        Thread.Sleep(flow);
                    }
                    else if (c.Equals("#"))
                        Thread.Sleep(paragraph);
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
    public static void RenderLine(bool isFlowing, Dialogue dialogue)
    {
        ConsoleColor actorColor = dialogue.Actor switch
        {
            "Narrator" => ConsoleColor.DarkCyan,
            "Kriss" => ConsoleColor.Cyan,
            "Corolla" => ConsoleColor.Red,
            "Smiurl" => ConsoleColor.Yellow,
            "Theo" => ConsoleColor.Blue,
            "Efeliah" => ConsoleColor.DarkGreen,
            "Math" => ConsoleColor.DarkMagenta,
            "Elder" => ConsoleColor.Magenta,
            "Jeorghe" => ConsoleColor.DarkMagenta,
            "Chief" => ConsoleColor.Magenta,
            "Person" => ConsoleColor.DarkYellow,
            "White" => ConsoleColor.White,
            "Saberinne" => ConsoleColor.Green,
            _ => ConsoleColor.DarkCyan, //default color
        };

        if (dialogue.IsTelepathy)
            RenderText(isFlowing, "<<" + dialogue.Line + ">> ", actorColor);
        else
            RenderText(isFlowing, "\"" + dialogue.Line + "\" ", actorColor);

        if (!IsDebug())
            Thread.Sleep(ParagraphBreak);
    }

    public static void RenderNonSpeechPart(bool isFlowing, string part)
    {
        RenderText(isFlowing, part);

        if (!IsDebug())
            Thread.Sleep(ParagraphBreak);
    }

    public static void RenderPrompt(List<ConsoleKeyInfo> keysPressed)
    {
        ForegroundColor = GetMappedColor(ConsoleColor.Gray);
        Write("\\>");
        CursorLeft += 1;

        //if redrawing after backspacing, rewrite stack
        if (keysPressed.Count != 0)
            for (int i = 0; i < keysPressed.Count; i++)
                Write(keysPressed[i].KeyChar.ToString());
    }

    public static void WaitForKey(int numberOfNewLines)
    {
        for (int i = 0; i < numberOfNewLines; i++)
            WriteLine();

        // Use light gray instead of dark gray on terminals with poor contrast
        ForegroundColor = GetMappedColor(ConsoleColor.DarkGray);

        // Debug color display if enabled
        if (DebugColors)
        {
            Write("Color test: ");
            ConsoleColor originalColor = ForegroundColor;

            foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
            {
                ForegroundColor = color;
                Write($"[{color}] ");
            }

            ForegroundColor = originalColor;
            WriteLine();
        }

        Write("Press a key to continue...");
        ReadKey(true);
    }

    /// <summary>
    /// Maps colors to high-contrast alternatives when needed
    /// </summary>
    public static ConsoleColor GetMappedColor(ConsoleColor original)
    {
        if (UseHighContrast && HighContrastMap.TryGetValue(original, out ConsoleColor value))
            return value;

        return original;
    }

    /// <summary>
    /// Detects if we should use high contrast mode based on environment
    /// </summary>
    private static bool DetectHighContrastMode()
    {
        // Check if explicitly set by launcher
        string highContrast = Environment.GetEnvironmentVariable("KRISS_USE_HIGH_CONTRAST");
        if (!string.IsNullOrEmpty(highContrast))
            return highContrast.Equals("true", StringComparison.CurrentCultureIgnoreCase);

        // Auto-detect terminal type that might need high contrast
        string termProgram = Environment.GetEnvironmentVariable("TERM_PROGRAM") ?? "";
        string term = Environment.GetEnvironmentVariable("TERM") ?? "";

        // Steam Deck or xterm detected
        return termProgram.Contains("steamdeck") ||
            term.Contains("xterm") ||
            Environment.GetEnvironmentVariable("SteamDeck") == "1";
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
