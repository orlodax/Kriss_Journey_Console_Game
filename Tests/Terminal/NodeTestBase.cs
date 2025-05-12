using System;
using KrissJourney.Kriss.Nodes;
using KrissJourney.Kriss.Services;
using KrissJourney.Tests.Infrastructure;
using KrissJourney.Tests.Infrastructure.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests.Terminal;

/// <summary>
/// Base class for node terminal-based tests
/// </summary>
public abstract class NodeTestBase
{
    /// <summary>
    /// The unified test runner for node testing
    /// </summary>
    protected NodeTestRunner TestRunner { get; private set; }

    /// <summary>
    /// Shorthand access to the GameEngine from the TestRunner
    /// </summary>
    protected GameEngine GameEngine => TestRunner.GameEngine;

    /// <summary>
    /// Shorthand access to the TerminalMock from the TestRunner
    /// </summary>
    protected TerminalMock TerminalMock => TestRunner.Terminal;

    [TestInitialize]
    public virtual void Setup()
    {
        // Create the test runner with terminal mock
        TestRunner = new NodeTestRunner(setupTerminalMock: true);

        // Add Enter key to queue to handle initial input
        SimulateUserInput(ConsoleKey.Enter);
    }

    /// <summary>
    /// Creates a node of the specified type with default settings
    /// </summary>
    protected T CreateNode<T>(int nodeId = 1, Action<T> configure = null) where T : NodeBase, new()
    {
        return TestRunner.CreateNode(nodeId, configure);
    }

    /// <summary>
    /// Simulates user input by providing a sequence of keys or text
    /// </summary>
    protected void SimulateUserInput(params ConsoleKey[] keys)
    {
        TestRunner.SimulateUserInput(keys);
    }

    /// <summary>
    /// Simulates typing text
    /// </summary>
    protected void SimulateTextInput(string text)
    {
        TestRunner.SimulateTextInput(text);
    }
}
