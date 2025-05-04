using System;
using KrissJourney.Kriss.Services.Terminal;

namespace KrissJourney.Kriss.Services;

public static class TerminalFacade
{
    private static ITerminal _terminal = new Terminal.Terminal();

    public static void SetTestTerminal(ITerminal testTerminal)
    {
        _terminal = testTerminal;
    }

    public static ITerminal GetCurrentTerminal()
    {
        return _terminal;
    }

    public static int WindowWidth => _terminal.WindowWidth;
    public static int WindowHeight => _terminal.WindowHeight;
    public static int CursorLeft
    {
        get => _terminal.CursorLeft;
        set => _terminal.CursorLeft = value;
    }
    public static int CursorTop
    {
        get => _terminal.CursorTop;
        set => _terminal.CursorTop = value;
    }
    public static int WindowLeft => _terminal.WindowLeft;
    public static int WindowTop => _terminal.WindowTop;
    public static ConsoleColor ForegroundColor
    {
        get => _terminal.ForegroundColor;
        set => _terminal.ForegroundColor = value;
    }
    public static ConsoleColor BackgroundColor
    {
        get => _terminal.BackgroundColor;
        set => _terminal.BackgroundColor = value;
    }
    public static string ReadLine() => _terminal.ReadLine();
    public static void WriteLine(string message = null) => _terminal.WriteLine(message);
    public static void Write(string message) => _terminal.Write(message);
    public static void WriteLine(string message, ConsoleColor color) => _terminal.WriteLine(message, color);
    public static void Write(string message, ConsoleColor color) => _terminal.Write(message, color);
    public static void Clear() => _terminal.Clear();
    public static void SetCursorPosition(int left, int top) => _terminal.SetCursorPosition(left, top);
    public static ConsoleKeyInfo ReadKey(bool intercept = false) => _terminal.ReadKey(intercept);
    public static void ResetColor() => _terminal.ResetColor();
    public static bool KeyAvailable => _terminal.KeyAvailable;
}
