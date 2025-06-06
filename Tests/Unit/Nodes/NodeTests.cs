using System.Collections.Generic;
using System.Linq;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Nodes;
using KrissJourney.Kriss.Services;
using KrissJourney.Tests.Infrastructure.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests.Unit.Nodes;

[TestClass]
public class NodeTests
{
    GameEngine gameEngine;

    [TestInitialize]
    public void TestInitialize()
    {
        gameEngine = GameEngineTestExtensions.Setup();
    }

    [TestCleanup]
    public void TestCleanup()
    { }

    [TestMethod]
    public void ThereAreNoLongDialougesWithReplies()
    {
        IEnumerable<DialogueNode> res = gameEngine.GetChapters().SelectMany(c => c.Nodes.OfType<DialogueNode>());

        IEnumerable<DialogueLine> longOnes = res.SelectMany(x => x.Dialogues.Where(y => y.Break == true));

        IEnumerable<DialogueLine> withReplies = longOnes.Where(l => l.Replies != null);

        Assert.IsTrue(!withReplies.Any());
    }


    [TestMethod]
    public void MiniGame01_InheritsFromActionNode()
    {
        Assert.IsInstanceOfType(new MiniGame01(), typeof(ActionNode));
    }
}

