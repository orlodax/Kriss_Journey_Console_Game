using System;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Nodes;
using KrissJourney.Kriss.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests;

[TestClass]
public class NodeJsonConverterTests
{
    [TestMethod]
    [DataRow(typeof(NodeBase))]
    [DataRow(typeof(StoryNode))]
    [DataRow(typeof(ChoiceNode))]
    [DataRow(typeof(DialogueNode))]
    [DataRow(typeof(ActionNode))]
    [DataRow(typeof(MiniGame01))]
    public void CanConvert_WithAnyNodeType_ReturnsTrue(Type nodeType)
    {
        // Arrange
        NodeJsonConverter converter = new();

        // Act
        bool canConvert = converter.CanConvert(nodeType);

        // Assert
        Assert.IsTrue(canConvert);
    }

    [TestMethod]
    public void CanConvert_WithNonNodeType_ReturnsFalse()
    {
        // Arrange
        NodeJsonConverter converter = new();

        // Act
        bool canConvert = converter.CanConvert(typeof(Status));

        // Assert
        Assert.IsFalse(canConvert);
    }
}