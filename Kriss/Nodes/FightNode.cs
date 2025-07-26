using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KrissJourney.Kriss.Helpers;
using KrissJourney.Kriss.Models;

namespace KrissJourney.Kriss.Nodes;

public class FightNode : NodeBase
{
    readonly ConsoleKey[] arrowKeys = [ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow];
    readonly string[] arrowNames = ["UP", "DOWN", "LEFT", "RIGHT"];
    readonly Random random = new();
    Prowess prowess;
    int rageBonus = 0;

    public Encounter Encounter { get; set; }

    public override void Load()
    {
        prowess = ProwessHelper.GetProwess(GameEngine.CurrentChapter.Id);

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

        Console.ForegroundColor = ConsoleColor.DarkYellow;
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
            AttackResult attackResult = FoeAttack(foe);
            if (attackResult is AttackResult.Fail)
            {
                rageBonus += prowess.RageBonus;
                prowess.Health -= foe.Damage;
                WriteLine($"You took {foe.Damage} damage! Health: {prowess.Health}");
                numberOfHits++;
            }
            Thread.Sleep(1000); // Brief pause between attacks

            // Player attacks back  
            if (prowess.Health > 0 && foe.Health > 0)
                PlayerAttack(foe, attackResult);
        }

        if (prowess.Health <= 0)
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


    private AttackResult FoeAttack(Foe foe)
    {
        // Select random arrow key
        int keyIndex = random.Next(arrowKeys.Length);
        ConsoleKey requiredKey = arrowKeys[keyIndex];
        string arrowSymbol = arrowNames[keyIndex];

        WriteLine($"{foe.Name} prepares to attack!");
        Thread.Sleep(800); // Build tension

        // Show the required key with timing window
        WriteLine($"Press {arrowSymbol} to dodge!");

        AttackResult result = ShowOscillatingCursorAndWaitForKey(requiredKey, cycles: 3, length: 20);

        WriteLine();
        string message = result switch
        {
            AttackResult.Perfect => "Perfect parry! You gained an edge.",
            AttackResult.Success => "Good dodge! You avoided the attack.",
            AttackResult.Fail => "You were hit! Your rage increases.",
            _ => throw new NotImplementedException()
        };
        WriteLine(message, ConsoleColor.DarkYellow);

        Typist.WaitForKey(2);
        RedrawNode();

        return result;
    }

    private void PlayerAttack(Foe foe, AttackResult defenseResult)
    {
        // Select random arrow key for player attack
        int keyIndex = random.Next(arrowKeys.Length);
        ConsoleKey requiredKey = arrowKeys[keyIndex];
        string arrowSymbol = arrowNames[keyIndex];

        WriteLine($"Your turn to attack {foe.Name}!");
        Thread.Sleep(600);

        WriteLine($"Press {arrowSymbol} to strike!");

        AttackResult result = ShowOscillatingCursorAndWaitForKey(requiredKey, cycles: 3, length: 20);

        int damage = prowess.BaseDamage + rageBonus;

        if (defenseResult == AttackResult.Perfect)
            damage += GetPerfectTimingBonus(); // Bonus damage for perfect dodge/parry

        switch (result)
        {
            case AttackResult.Perfect:
                damage += GetPerfectTimingBonus(); // Bonus damage for perfect timing
                foe.Health -= damage;
                WriteLine($"Critical hit! You deal {damage} damage to {foe.Name}!", ConsoleColor.DarkYellow);
                break;

            case AttackResult.Success:
                foe.Health -= damage;
                WriteLine($"You deal {damage} damage to {foe.Name}.", ConsoleColor.DarkYellow);
                break;

            case AttackResult.Fail:
                foe.Health -= damage / 2;
                WriteLine($"Weak attack! You deal {damage / 2} damage to {foe.Name}.", ConsoleColor.DarkYellow);
                break;
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

    /// <summary>
    /// Displays an oscillating cursor (O) moving back and forth between < and >, with an X as the target.
    /// The player must press the required key when the O overlaps the X.
    /// Returns true if the key is pressed at the right moment, otherwise false.
    /// </summary>
    /// <param name="requiredKey">The key the player must press.</param>
    /// <param name="cycles">How many oscillations the minigame lasts.</param>
    /// <param name="length">The number of positions between < and > (including X and O).</param>
    private AttackResult ShowOscillatingCursorAndWaitForKey(ConsoleKey requiredKey, int cycles = 3, int length = 20)
    {
        AttackResult result = AttackResult.Fail;

        int qteWidth = Encounter.Level switch
        {
            1 => 2,
            2 => 2,
            3 => 1,
            4 => 1,
            5 => 0,
            _ => 1
        };

        int sleep = 80 / Encounter.Level; // ms per frame
        int right = length - 1;
        int targetPos = random.Next(1, right - 1); // Avoid edges
        int cursorPos = 1;
        int direction = 1;
        bool isKeyPressed = false;
        int oscillations = 0;
        bool goingRight = true;

        Console.CursorVisible = false;

        while (oscillations < cycles && !isKeyPressed)
        {
            // Build the line
            Console.SetCursorPosition(0, Console.CursorTop);

            Console.Write('<');
            for (int i = 0; i < length; i++)
            {
                char symbol = '-';

                if (i == targetPos)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    symbol = 'X';
                }
                else if (i >= targetPos - qteWidth && i <= targetPos + qteWidth)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                if (i == cursorPos)
                    symbol = '█';

                Console.Write(symbol);

                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
            Console.Write('>');

            // Check for key
            int elapsed = 0;
            while (elapsed < sleep)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                    if (keyPressed.Key == requiredKey)
                    {
                        isKeyPressed = true;

                        if (cursorPos == targetPos)
                            result = AttackResult.Perfect;
                        else if (Math.Abs(cursorPos - targetPos) <= 2)
                            result = AttackResult.Success;
                        else
                            result = AttackResult.Fail;

                        break;
                    }
                    else
                    {
                        isKeyPressed = true;
                        result = AttackResult.Fail;
                        break;
                    }
                }
                Thread.Sleep(10);
                elapsed += 10;
            }

            if (isKeyPressed)
                break;

            // Move cursor
            cursorPos += direction;

            // Detect oscillation (reached an end and changed direction)
            if (cursorPos == right && goingRight)
            {
                direction *= -1;
                goingRight = false;
                oscillations++;
            }
            else if (cursorPos == 0 && !goingRight)
            {
                direction *= -1;
                goingRight = true;
                oscillations++;
            }
        }

        // Clear line
        Console.Write("\r" + new string(' ', length + 2) + "\r");
        Console.CursorVisible = true;
        return result;
    }

    private int GetPerfectTimingBonus() => new Random().Next(prowess.BaseDamage / 10, prowess.BaseDamage / 3);

    private enum AttackResult
    {
        Fail,
        Success,
        Perfect
    }
}

