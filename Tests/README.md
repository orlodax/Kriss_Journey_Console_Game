# KrissJourney Testing Framework

This document explains the testing infrastructure for the KrissJourney game engine.

## Folder Structure

The testing project is organized into the following structure:

```text
Tests/
├── Infrastructure/           # Testing infrastructure & utilities
│   ├── Mocks/                # Mock implementations for testing
│   │   ├── TerminalMock.cs   # Mock implementation of ITerminal
│   │   └── TestStatusManager.cs # Mock implementation of StatusManager
│   ├── Helpers/              # Test helper classes
│   │   ├── NodeTestHelper.cs # Helper for testing nodes
│   │   └── TestUtils.cs      # General test utilities
│   └── NodeTestRunner.cs     # Unified test runner for nodes
├── Unit/                     # Unit tests for specific components
│   ├── Models/               # Tests for model classes
│   ├── Nodes/                # Tests for node behaviors (non-UI)
│   ├── Services/             # Tests for services
│   └── Converters/           # Tests for converters and serialization
├── Integration/              # Tests that span multiple components
│   └── StoryFlow/            # Tests for story flow and progression
└── Terminal/                 # Tests that interact with the terminal UI
    └── Nodes/                # Terminal-based node interaction tests
```

## Key Components

### 1. NodeTestRunner (Infrastructure)

`NodeTestRunner` is the core class that handles all the setup and management for testing nodes. It:

- Creates a test environment with a TestStatusManager that doesn't rely on the file system
- Sets up a mock terminal for testing terminal interactions (optional)
- Creates and configures test nodes with default or custom properties
- Manages the test chapter and node relationships
- Provides methods for simulating user input and verifying output

```csharp
// Example of creating and using a NodeTestRunner
var testRunner = new NodeTestRunner(setupTerminalMock: true);
var storyNode = testRunner.CreateNode<StoryNode>(configure: node => {
    node.Text = "Custom story text";
});
```

### 2. NodeTestBase (Terminal)

`NodeTestBase` is an abstract base class for terminal-based node tests that leverages the NodeTestRunner. It:

- Initializes a NodeTestRunner with terminal mock enabled
- Provides easy access to common test operations
- Simplifies the creation of terminal-based tests

```csharp
// Example of a test class that inherits from NodeTestBase
[TestClass]
public class MyNodeTests : NodeTestBase
{
    [TestMethod]
    public void MyTest()
    {
        var node = CreateNode<StoryNode>();
        SimulateUserInput(ConsoleKey.Enter);
        // ... test code ...
    }
}
```

### 3. TestStatusManager (Infrastructure/Mocks)

`TestStatusManager` is a special implementation of StatusManager that doesn't use the file system, making it ideal for unit tests. It:

- Overrides file system operations to use in-memory data structures
- Provides the same interface as the real StatusManager
- Enables testing without file system dependencies

## Usage Examples

### Basic Node Testing (Unit Tests)

```csharp
// Location: Unit/Nodes/MyNodeTests.cs
namespace KrissJourney.Tests.Unit.Nodes;

[TestMethod]
public void BasicNodeTest()
{
    var testRunner = new NodeTestRunner(); // No terminal mock needed
    
    testRunner.TestNode<StoryNode>(node => {
        // Test code goes here
        Assert.AreEqual("Test StoryNode node", node.Text);
    });
}
```

### Testing Terminal Interactions

```csharp
// Location: Terminal/Nodes/MyTerminalTests.cs
namespace KrissJourney.Tests.Terminal.Nodes;

[TestMethod]
public void TerminalInteractionTest()
{
    var testRunner = new NodeTestRunner(setupTerminalMock: true);
    
    var actionNode = testRunner.CreateNode<ActionNode>();
    
    // Simulate user pressing Tab
    testRunner.SimulateUserInput(ConsoleKey.Tab);
    
    // Process the input
    actionNode.Load();
    
    // Verify the output
    var output = testRunner.GetTerminalOutput();
    Assert.IsTrue(output.Contains("Possible actions here"));
}
```

## Best Practices

1. Place tests in the appropriate folder:
   - Put unit tests in the `Unit` folder
   - Put integration tests in the `Integration` folder
   - Put terminal-based tests in the `Terminal` folder

2. Follow the namespace convention:
   - `KrissJourney.Tests.Unit.X` for unit tests
   - `KrissJourney.Tests.Integration.X` for integration tests
   - `KrissJourney.Tests.Terminal.X` for terminal tests
   - `KrissJourney.Tests.Infrastructure.X` for infrastructure

3. Use the appropriate base classes:
   - Use `NodeTestBase` for terminal-based tests
   - Use `NodeTestRunner` directly for non-terminal tests or complex scenarios

4. Keep test logic focused on specific behaviors

5. Use the configure parameter to set up complex node structures

6. Skip terminal tests that are difficult to mock with `Assert.Inconclusive`
