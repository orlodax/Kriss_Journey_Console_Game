using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KrissJourney.Kriss.Helpers;
using KrissJourney.Kriss.Models;

namespace KrissJourney.Kriss.Nodes;

public class FightNode : NodeBase
{
    readonly ConsoleKey[] ArrowKeys = [ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow];
    readonly string[] ArrowNames = ["Up", "Down", "Left", "Right"];
    Random Random;
    Prowess Prowess;

    public Encounter Encounter { get; set; }

    public override void Load()
    {
        Random = new();
        Prowess = ProwessHelper.GetProwess(GameEngine.CurrentChapter.Id);

        Init();
        WriteLine();
        WriteLine();

        if (IsVisited)
        {
            Typist.InstantText("You already defeated this enemy. You're off the hook this time.", ConsoleColor.DarkRed);
            Typist.WaitForKey(numberOfNewLines: 3);

            AdvanceToNext(ChildId);
        }

        Fight();
    }

    void Fight()
    {
        if (IsThisNode(chapterId: 10, nodeId: 1)) // To display tutorial the first time player encounters a fight
        {
            // display tutorial
        }

        Typist.RenderText(isFlowing: true, "Prepare to fight!", ConsoleColor.Red);
        Typist.WaitForKey(2);
        WriteLine();

        int numberOfHits = 0;
        RecursiveRounds(Encounter.Foes, numberOfHits);

        Typist.RenderText(isFlowing: true, Encounter.VictoryMessage, ConsoleColor.Red);
        Typist.WaitForKey(3);
        AdvanceToNext(ChildId);
    }

    void RecursiveRounds(IEnumerable<Foe> foes, int numberOfHits)
    {
        foreach (Foe foe in foes)
        {
            bool dodged = ProcessFoeAttack(foe);
            if (!dodged)
            {
                Prowess.Health -= foe.Damage;
                WriteLine($"You took {foe.Damage} damage! Health: {Prowess.Health}");
                numberOfHits++;
            }
            Thread.Sleep(1000); // Brief pause between attacks

            // Player attacks back  
            if (Prowess.Health > 0 && foe.Health > 0)
                ProcessPlayerAttack(foe);
        }

        if (Prowess.Health <= 0)
            GameOver();

        IEnumerable<Foe> remainingFoes = foes.Where(f => f.Health > 0);

        if (remainingFoes.Any())
            RecursiveRounds(remainingFoes, numberOfHits);
    }

    void GameOver()
    {
        Typist.RenderText(isFlowing: true, Encounter.DefeatMessage, ConsoleColor.Red);

        Typist.WaitForKey(numberOfNewLines: 3);
        AdvanceToNext(childId: 1);
    }


    private bool ProcessFoeAttack(Foe foe)
    {
        // Select random arrow key
        int keyIndex = Random.Next(ArrowKeys.Length);
        ConsoleKey requiredKey = ArrowKeys[keyIndex];
        string arrowSymbol = ArrowNames[keyIndex];

        WriteLine($"{foe.Name} prepares to attack!");
        Thread.Sleep(800); // Build tension

        // Show the required key with timing window
        WriteLine($"Press {arrowSymbol} to dodge! Quick!");

        if (WaitForTimedKeyPress(requiredKey, timeoutMs: 1500))
        {
            WriteLine("Perfect dodge! You avoided the attack!", ConsoleColor.Green);
            Typist.WaitForKey(2);
            RedrawNode();
            return true;
        }

        WriteLine("You failed to dodge the attack!", ConsoleColor.Red);
        Typist.WaitForKey(2);
        RedrawNode();

        return false;
    }

    private void ProcessPlayerAttack(Foe foe)
    {
        // Select random arrow key for player attack
        int keyIndex = Random.Next(ArrowKeys.Length);
        ConsoleKey requiredKey = ArrowKeys[keyIndex];
        string arrowSymbol = ArrowNames[keyIndex];

        WriteLine($"Your turn to attack {foe.Name}!");
        Thread.Sleep(600);

        WriteLine($"Press {arrowSymbol} to strike! Time it right!");

        bool success = WaitForTimedKeyPress(requiredKey, timeoutMs: 1500);

        if (success)
        {
            int damage = Prowess.Damage + Random.Next(1, 5); // Bonus damage for perfect timing
            foe.Health -= damage;
            WriteLine($"Critical hit! You deal {damage} damage to {foe.Name}!", ConsoleColor.Yellow);
        }
        else
        {
            int damage = Math.Max(1, Prowess.Damage / 2); // Reduced damage for missed timing
            foe.Health -= damage;
            WriteLine($"Weak attack! You deal {damage} damage to {foe.Name}.", ConsoleColor.DarkYellow);
        }

        WriteLine();

        foe.Health = Math.Max(0, foe.Health); // Ensure health doesn't go negative

        if (foe.Health <= 0)
            WriteLine($"{foe.Name} is defeated!", ConsoleColor.Green);
        else
            WriteLine($"{foe.Name} has {foe.Health} health remaining.");

        Typist.WaitForKey(2);
        RedrawNode();
    }

    private bool WaitForTimedKeyPress(ConsoleKey requiredKey, int timeoutMs)
    {
        DateTime startTime = DateTime.Now;

        // Clear any existing key presses
        while (Console.KeyAvailable)
            Console.ReadKey(true);

        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == requiredKey)
                    return true;
                else // Wrong key pressed - instant failure
                    return false;
            }
            Thread.Sleep(10); // Small delay to prevent busy waiting
        }

        return false; // Timeout
    }
}
