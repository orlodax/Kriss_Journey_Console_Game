using System;
using System.Collections.Generic;
using KrissJourney.Kriss.Helpers;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Services;
using Action = KrissJourney.Kriss.Models.Action;

namespace KrissJourney.Kriss.Nodes;

public partial class ActionNode : NodeBase
{
    Action act = null;
    protected readonly List<ConsoleKeyInfo> keysPressed = [];
    protected string BottomMessage = string.Empty;
    ConsoleColor BottomMessageColor = Typist.GetMappedColor(ConsoleColor.DarkCyan);

    public List<Action> Actions { get; set; } // list of all possible actions

    public override void Load()
    {
        Init();
        PrepareForAction();
    }

    protected void PrepareForAction(bool isFirstTimeDisplayed = true)
    {
        ///go to bottom line and prepare prompt
        CursorTop = WindowTop + WindowHeight - 1;
        CursorLeft = WindowLeft;

        ForegroundColor = Typist.GetMappedColor(ConsoleColor.DarkGray);
        if (!isFirstTimeDisplayed)
        {
            CursorTop -= 1;
            WriteLine("You can't or won't do that. Try again.");
        }

        Typist.RenderPrompt(keysPressed);

        do
        {
            ConsoleKeyInfo input = ReadKey();

            switch (input.Key)
            {
                case ConsoleKey.Tab:                //if player presses tabs looking for help
                    TabPressed();
                    break;

                case ConsoleKey.Backspace:          //to erase
                    BackSpacePressed(keysPressed);
                    break;

                case ConsoleKey.Enter:
                    EnterPressed(keysPressed);
                    break;

                default:
                    keysPressed.Add(input);          //normal keys are registered
                    break;
            }
        }
        while (true);
    }

    #region Special keys pressed
    protected virtual void TabPressed()
    {
        BottomMessage = string.Empty;
        RedrawNode();

        List<string> helpObjects = [];
        string opinion = string.Empty;                                      //if this gets populated, show object help not verbs

        string[] words = ExtractWords();

        string matchingVerb = string.Empty;

        if (!string.IsNullOrWhiteSpace(words[0]))
        {
            foreach (Action action in Actions)
            {
                string verb = action.Verbs.Find(v => v.Equals(words[0]));       //look into each action's verbs to see if there is our typed word
                if (verb != null)
                {
                    if (action.Objects.Count != 0)
                    {
                        foreach (ActionObject objContainer in action.Objects)         //when the action is found, iterate through every object term
                            if (objContainer.Objs is not null && objContainer.Objs.Count != 0 && !helpObjects.Contains(objContainer.Objs[0]))
                                helpObjects.Add(objContainer.Objs[0]);            //if the object is not already in the list, add it
                            else
                                opinion = Action.GetHelpObject(verb); //if the object is empty, add the verb opinion
                    }
                    else
                        helpObjects.Add("Just do it.");

                    break;
                }
            }
        }

        CursorTop = WindowHeight - 4;
        CursorLeft = WindowLeft;

        ForegroundColor = Typist.GetMappedColor(ConsoleColor.DarkGray);

        if (helpObjects.Count != 0)
        {
            WriteLine("Possible objects for the action typed: ");

            ForegroundColor = Typist.GetMappedColor(ConsoleColor.DarkYellow); ;
            foreach (string term in helpObjects)
                Write(term + " ");
        }
        else if (!string.IsNullOrEmpty(opinion))
        {
            ForegroundColor = Typist.GetMappedColor(ConsoleColor.DarkYellow); ;
            Write(opinion);
        }
        else
        {
            WriteLine("Possible actions here: ");

            ForegroundColor = Typist.GetMappedColor(ConsoleColor.DarkYellow); ;
            foreach (Action action in Actions)
                Write(action.Verbs[0] + " ");
        }

        ForegroundColor = Typist.GetMappedColor(ConsoleColor.DarkGray); ;
        CursorTop = WindowHeight - 1;
        CursorLeft = 0;

        if (GameEngine.CheckChap2Node2())
        {
            CursorTop -= 1;
            WriteLine("\\> you pressed tab for help. noob.");
        }

        Typist.RenderPrompt(keysPressed);
    }
    void BackSpacePressed(List<ConsoleKeyInfo> keysPressed)
    {
        if (keysPressed.Count != 0)
        {
            keysPressed.RemoveAt(keysPressed.Count - 1);

            RedrawNode(true);
            PrepareForAction(true);
        }
    }
    protected string[] ExtractWords()
    {
        //reconstruct
        string typed = string.Empty;

        for (int i = 0; i < keysPressed.Count; i++)
            typed += keysPressed[i].KeyChar.ToString().ToLower();

        char[] delimiterChars = [' ', ',', '.', ':', '\t', '!', '\r'];

        return typed.Split(delimiterChars);
    }
    protected virtual void EnterPressed(List<ConsoleKeyInfo> keysPressed)
    {
        bool isFirstTimeDisplayed = true;

        if (keysPressed.Count != 0)
        {
            act = null;

            string[] words = ExtractWords();

            keysPressed.Clear();                                            //clear the stack after giving command

            string matchingVerb = string.Empty;

            foreach (string word in words)                                  //is there one word matching one action?
            {
                foreach (Action action in Actions)
                {
                    if (action.Verbs.Contains(word))
                    {
                        act = action;
                        matchingVerb = word;                                //store the typed verb which triggered the action
                        break;
                    }
                }
            }

            if (act != null)                                                //if there's an action available...
            {
                if (act.Objects.Count == 0)                                     //...and is objectless...
                    ProcessAction(act);
                else
                {                                                           //...otherwise, examine Objects 
                    foreach (ActionObject o in act.Objects)
                        foreach (string word in words)                      //is there a matching object available? just hand me the first you find please
                            if (o.Objs is not null && o.Objs.Contains(word))
                                ProcessAction(o);                           //the action is right, and there is a acceptable object specified

                    if (act.Answer != null)
                        DisplaySuccess(act.Answer, act.ChildId);
                    else
                        CustomRefusal(act.GetAnswer(matchingVerb));        //the action is right, but no required object is specified
                }
            }
            else
                isFirstTimeDisplayed = false;
        }

        //if there's no action available/no keys are pressed, redraw node and display standard refuse
        RedrawNode();
        BottomMessage = string.Empty;
        PrepareForAction(isFirstTimeDisplayed);
    }
    #endregion

    void ProcessAction(IAction action)
    {
        if (!GameEngine.Evaluate(action.Condition))              //if for some reason Kriss can't do it, say it...
            CustomRefusal(action.Condition.Refusal);
        else                                                    //...otherwise, do it
        {
            if (action.Effect != null)                          //in case the obj has an Effect (inventory)
                GameEngine.AddItemToInventory(action.Effect);

            DisplaySuccess(action.Answer, action.ChildId);
        }
    }

    void CustomRefusal(string refusal)
    {
        RedrawNode();

        CursorTop = WindowHeight - 4;
        CursorLeft = WindowLeft;

        refusal = "<<" + refusal + ">>";

        BottomMessage = refusal;
        BottomMessageColor = ConsoleColor.Cyan;

        Typist.FlowingText(refusal, BottomMessageColor);
        WriteLine();
        WriteLine();

        PrepareForAction(true); //display prompt without standard refuse
    }
    void DisplaySuccess(string answer, int? childId = null)
    {
        if (answer != null)
        {
            RedrawNode();

            BottomMessage = answer;
            BottomMessageColor = ConsoleColor.DarkYellow;

            CursorTop = MeasureMessage(answer);
            CursorLeft = WindowLeft;

            Typist.FlowingText(answer, BottomMessageColor);

            if (childId.HasValue)
            {
                Typist.WaitForKey(3);

                AdvanceToNext(childId.Value);
                return;
            }
        }
        if (childId.HasValue)
        {
            AdvanceToNext(childId.Value);
            return;
        }

        //if everything fails:
        PrepareForAction(true); //display prompt without standard refuse
    }
    protected void RedrawNode(bool isDeleting = false)
    {
        Clear();
        Typist.InstantText(Text);

        if (isDeleting)
            if (!string.IsNullOrWhiteSpace(BottomMessage))                      //decide if there is a Bottom Message and of which type
            {
                switch (BottomMessageColor)
                {
                    case ConsoleColor.DarkYellow:
                        CursorTop = MeasureMessage(BottomMessage);
                        CursorLeft = WindowLeft;
                        break;

                    case ConsoleColor.Cyan:
                        CursorTop = WindowHeight - 4;
                        CursorLeft = WindowLeft;
                        break;
                }
                Typist.InstantText(BottomMessage, BottomMessageColor);
            }
    }

    protected static int MeasureMessage(string answer)
    {
        //measure the length and the newlines in the answer to determine how up to go to start writing
        int newLines = NewLineRegex().Matches(answer).Count;
        int rows = answer.Length / WindowWidth;

        return Math.Min(WindowHeight - (rows + newLines), WindowHeight - 5) - 2;
    }

    [System.Text.RegularExpressions.GeneratedRegex("\\n")]
    private static partial System.Text.RegularExpressions.Regex NewLineRegex();
}