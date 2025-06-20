using System;

namespace KrissJourney.Kriss.Models;

public enum Characters
{
    Narrator,
    Kriss,
    Corolla,
    Smiurl,
    Theo,
    Efeliah,
    Math,
    Elder,
    Jeorghe,
    Chief,
    Person,
    Saberinne
}

public static class CharactersHelper
{
    public static ConsoleColor Color(this Characters character)
    {
        return character switch
        {
            Characters.Narrator => ConsoleColor.DarkCyan,
            Characters.Kriss => ConsoleColor.Cyan,
            Characters.Corolla => ConsoleColor.Red,
            Characters.Smiurl => ConsoleColor.Yellow,
            Characters.Theo => ConsoleColor.Blue,
            Characters.Efeliah => ConsoleColor.DarkGreen,
            Characters.Math => ConsoleColor.DarkMagenta,
            Characters.Elder => ConsoleColor.Magenta,
            Characters.Jeorghe => ConsoleColor.DarkMagenta,
            Characters.Chief => ConsoleColor.Magenta,
            Characters.Person => ConsoleColor.DarkYellow,
            Characters.Saberinne => ConsoleColor.Green,
            _ => ConsoleColor.DarkCyan, // default color
        };
    }
}