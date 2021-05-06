using kriss.Classes;
using System;
using System.Linq;

Console.Title = "KRISS' JOURNEY";

DataLayer.Init();
           
Console.Clear();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("            ██╗  ██╗██████╗ ██╗███████╗███████╗              ");
Console.WriteLine("            ██║ ██╔╝██╔══██╗██║██╔════╝██╔════╝              ");
Console.WriteLine("            █████╔╝ ██████╔╝██║███████╗███████╗              ");
Console.WriteLine("            ██╔═██╗ ██╔══██╗██║╚════██║╚════██║              ");
Console.WriteLine("            ██║  ██╗██║  ██║██║███████║███████║              ");
Console.WriteLine("            ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝╚══════╝╚══════╝              ");
Console.WriteLine("     ██╗ ██████╗ ██╗   ██╗██████╗ ███╗   ██╗███████╗██╗   ██╗");
Console.WriteLine("     ██║██╔═══██╗██║   ██║██╔══██╗████╗  ██║██╔════╝╚██╗ ██╔╝");
Console.WriteLine("     ██║██║   ██║██║   ██║██████╔╝██╔██╗ ██║█████╗   ╚████╔╝ ");
Console.WriteLine("██   ██║██║   ██║██║   ██║██╔══██╗██║╚██╗██║██╔══╝    ╚██╔╝  ");
Console.WriteLine("╚█████╔╝╚██████╔╝╚██████╔╝██║  ██║██║ ╚████║███████╗   ██║   ");
Console.WriteLine(" ╚════╝  ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝╚══════╝   ╚═╝   ");
Console.WriteLine();
Console.WriteLine("              />_________________________________");
Console.WriteLine("     [########[]_________________________________>");
Console.WriteLine("              \\>");
Console.WriteLine();
Console.WriteLine("-------------------------------------------------------------");
            
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine();

//debug: start from. Comment for default start
DataLayer.CurrentChapter = DataLayer.Chapters[1];
DataLayer.LoadNode(27);
//debug

int chapterId = 1;

if (DataLayer.Status.VisitedNodes.Count > 1)
{
    Console.WriteLine("Welcome back, traveler. This is your journey so far.");
    Console.WriteLine("This game still features autosave, at least for now.");
    Console.WriteLine("Press a number to select a chapter.");
    Console.WriteLine();

    int lastChapter = DataLayer.Status.VisitedNodes.Keys.Max();

    for (int i = 0; i < lastChapter; i++)
        Console.WriteLine(i + 1 + ". " + DataLayer.Chapters[i].Title);

    bool isValid = false;

    do
    {
        ConsoleKeyInfo key = Console.ReadKey(true);

        if (int.TryParse(key.KeyChar.ToString(), out int digit))
            if (isValid = digit <= lastChapter)
                chapterId = digit;
    } 
    while (!isValid);
}
else
{
    Console.WriteLine("Welcome traveler. Your journey is yet to be started.");
    Console.WriteLine("This game features autosave. You just won't know when.");
    Console.WriteLine();
    Console.WriteLine("Press any key.");
    Console.ReadKey(true);
}

// load first node of selected chapter
DataLayer.StartChapter(chapterId);