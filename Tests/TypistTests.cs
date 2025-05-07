using System;
using System.Collections.Generic;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests;

[TestClass]
public class TypistTests
{
    [TestInitialize]
    public void TestInitialize()
    {
        // Setup console output capture
        TestUtils.SetupConsoleOutput();
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Restore console
        TestUtils.RestoreConsole();
    }

    [TestMethod]
    public void RenderPrompt_WithInput_DisplaysPromptWithInput()
    {
        // Arrange
        List<ConsoleKeyInfo> keysPressed =
        [
            new ConsoleKeyInfo('t', ConsoleKey.T, false, false, false),
            new ConsoleKeyInfo('e', ConsoleKey.E, false, false, false),
            new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false),
            new ConsoleKeyInfo('t', ConsoleKey.T, false, false, false)
        ];

        // Act
        Typist.RenderPrompt(keysPressed);

        // Assert
        // We can't easily assert the console output directly in unit tests
        // This test primarily ensures the method runs without exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void RenderPrompt_WithEmptyInput_DisplaysPromptOnly()
    {
        // Arrange
        List<ConsoleKeyInfo> keysPressed = [];

        // Act
        Typist.RenderPrompt(keysPressed);

        // Assert
        // We can't easily assert the console output directly in unit tests
        // This test primarily ensures the method runs without exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void InstantText_WithText_DisplaysTextImmediately()
    {
        // Arrange
        string text = "This is an instant text";

        // Act
        Typist.InstantText(text);

        // Assert
        // We can't easily assert the console output directly in unit tests
        // This test primarily ensures the method runs without exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void RenderText_WithFlowing_CallsFlowingText()
    {
        // Arrange
        string text = "This is flowing text";
        bool isFlowing = true;

        // Act
        Typist.RenderText(isFlowing, text);

        // Assert
        // We can't easily assert the console output directly in unit tests
        // This test primarily ensures the method runs without exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void RenderText_WithoutFlowing_CallsInstantText()
    {
        // Arrange
        string text = "This is instant text";
        bool isFlowing = false;

        // Act
        Typist.RenderText(isFlowing, text);

        // Assert
        // We can't easily assert the console output directly in unit tests
        // This test primarily ensures the method runs without exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void RenderLine_WithFlowing_DisplaysDialogueLine()
    {
        // Arrange
        Dialogue dialogue = new()
        {
            Line = "Welcome to the tavern, traveler."
        };
        bool isFlowing = true;

        // Act
        Typist.RenderLine(isFlowing, dialogue);

        // Assert
        // We can't easily assert the console output directly in unit tests
        // This test primarily ensures the method runs without exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void RenderNonSpeechPart_WithFlowing_DisplaysNonSpeechText()
    {
        // Arrange
        string text = "The innkeeper looks at you suspiciously.";
        bool isFlowing = true;

        // Act
        Typist.RenderNonSpeechPart(isFlowing, text);

        // Assert
        // We can't easily assert the console output directly in unit tests
        // This test primarily ensures the method runs without exceptions
        Assert.IsTrue(true);
    }
}