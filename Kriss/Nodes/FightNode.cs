using System.Collections.Generic;
using System.Linq;
using KrissJourney.Kriss.Helpers;
using KrissJourney.Kriss.Models;

namespace KrissJourney.Kriss.Nodes;

public class FightNode : NodeBase
{
    Prowess Prowess;

    public Encounter Encounter { get; set; }

    public override void Load()
    {
        Prowess = ProwessHelper.GetProwess(GameEngine.CurrentChapter.Id);

        Init();
        WriteLine();
        WriteLine();

        if (IsVisited)
        {
            Typist.InstantText("You already defeated this enemy. You're off the hook this time.", System.ConsoleColor.DarkRed);
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

        Typist.RenderText(isFlowing: true, "Prepare to fight!", System.ConsoleColor.Red);
        WriteLine();
        WriteLine();

        int numberOfHits = 0;
        RecursiveRounds(Encounter.Foes, numberOfHits);

        Typist.RenderText(isFlowing: true, Encounter.VictoryMessage, System.ConsoleColor.Red);
        Typist.WaitForKey(3);
        AdvanceToNext(ChildId);
    }

    void RecursiveRounds(IEnumerable<Foe> foes, int numberOfHits)
    {
        foreach (Foe foe in foes)
        {
            WriteLine($"You are facing {foe.Name}: (Health: {foe.Health}, Damage: {foe.Damage})");
            for (int i = 0; i < foe.NumberOfAttacks; i++)
            {
                WriteLine($"Foe attacks you for {foe.Damage} damage!");
                numberOfHits++;
            }

            // Player attacks back  
        }

        if (Prowess.Health <= 0)
            GameOver();

        IEnumerable<Foe> remainingFoes = foes.Where(f => f.Health > 0);

        if (remainingFoes.Any())
            RecursiveRounds(remainingFoes, numberOfHits);
    }

    void GameOver()
    {
        Typist.RenderText(isFlowing: true, Encounter.DefeatMessage, System.ConsoleColor.Red);

        Typist.WaitForKey(numberOfNewLines: 3);
        AdvanceToNext(childId: 1);
    }
}
