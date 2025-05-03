using KrissJourney.Kriss.Classes;
using KrissJourney.Kriss.Nodes;
using NUnit.Framework;
using System.Linq;

namespace KrissJourney.Tests;

public class Misc
{
    [SetUp]
    public void Setup()
    {
        DataLayer.Init();
    }

    [Test]
    public void ThereAreNoLongDialougesWithReplies()
    {
        var res = DataLayer.Chapters.SelectMany(c => c.Nodes.OfType<DialogueNode>());

        var longOnes = res.SelectMany(x => x.Dialogues.Where(y => y.Break == true));

        var withReplies = longOnes.Where(l => l.Replies != null);

        Assert.IsTrue(!withReplies.Any());
    }
}