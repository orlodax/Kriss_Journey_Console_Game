using kriss.Classes;
using System;

Console.Title = "KRISS' JOURNEY";

DataLayer.Init();
           
ShowMenu();

static void ShowMenu()
{
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
    //DataLayer.LoadChapter(1);
    //odeFactory.LoadNode(1);
    //debug

    int chapterId = 1;

    if (DataLayer.Status.LastChapter > 1)
    {
        Console.WriteLine("Welcome back, traveler. Press any key to start the next chapter.");
        Console.WriteLine("Press a number if you want to replay an already completed chapter.");
        Console.WriteLine("So far, you completed these chapters:");
        Console.WriteLine();

        for (int i = 0; i < DataLayer.Status.LastChapter; i++)
            Console.WriteLine(DataLayer.Chapters[i].Title);

        var key = Console.ReadKey(true);

        //TODO: cycle input waiting because number could be higher than last chapter ...
        if (char.IsDigit(key.KeyChar))
            chapterId = Convert.ToInt32(key.KeyChar);
        else
            chapterId = DataLayer.Status.LastChapter;
    }
    else
    {
        Console.WriteLine("Welcome traveler. Your journey is yet to be started.");
        Console.WriteLine("This game features autosave. You just won't know when.");
        Console.WriteLine();
        Console.WriteLine("Press any key.");
        Console.ReadKey(true);
    }

    // load first node of last (current) chapter
    DataLayer.StartChapter(chapterId);
}

