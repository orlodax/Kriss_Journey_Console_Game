using kriss.Classes;
using lybra;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = lybra.Action;
using Object = lybra.Object;

namespace kriss.Nodes
{
    public class NAction : NodeBase
    {
        Action act = null;
        internal readonly List<ConsoleKeyInfo> keysPressed = new();
        internal string BottomMessage = string.Empty;
        ConsoleColor BottomMessageColor = ConsoleColor.DarkCyan;

        public NAction(NodeBase node) : base(node)
        {
            this.Init();
            PrepareForAction();
        }

        internal void PrepareForAction(bool isFirstTimeDisplayed = true)
        {
            ///go to bottom line and prepare prompt
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 2;
            Console.CursorLeft = Console.WindowLeft + Console.WindowWidth - 1;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (!isFirstTimeDisplayed)
            {
                Console.CursorTop -= 1;
                Console.WriteLine(" You can't or won't do that. Try again.");
            }

            Typist.RenderPrompt(keysPressed);

            do
            {
                ConsoleKeyInfo input = Console.ReadKey();

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
        internal virtual void TabPressed() 
        {
            RedrawNode();

            List<string> helpObjects = new();                                       //if this gets populated, show object help not verbs

            string[] words = ExtractWords();

            string matchingVerb = string.Empty;

            if (!string.IsNullOrWhiteSpace(words[0]))
            {
                foreach (Action action in Actions)
                {
                    string verb = action.Verbs.Find(v => v.Equals(words[0]));       //look into each action's verbs to see if there is our typed word
                    if (verb != null)
                    {
                        if (action.Objects.Any())
                            foreach (Object objContainer in action.Objects)         //when the action is found, iterate through every object term
                                helpObjects.Add(objContainer.Objs[0]);
                        else
                            helpObjects.Add("Just do it.");

                        break;
                    }
                }
            }

            Console.CursorTop = Console.WindowHeight - 4;
            Console.CursorLeft = Console.WindowLeft;

            Console.ForegroundColor = ConsoleColor.DarkGray;

            if (helpObjects.Any())
            {
                Console.WriteLine("Possible objects for the action typed: ");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                foreach (string term in helpObjects)
                     Console.Write(term + " ");
            }
            else
            {
                Console.WriteLine("Possible actions here: ");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                foreach (Action action in Actions)
                    Console.Write(action.Verbs[0] + " ");
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.CursorTop = Console.WindowHeight - 1;
            Console.CursorLeft = 0;

            if (DataLayer.CheckChap2Node2())
            {
                Console.CursorTop -= 1;
                Console.WriteLine("\\> you pressed tab for help. noob.");
            }

            Typist.RenderPrompt(keysPressed);
        }
        void BackSpacePressed(List<ConsoleKeyInfo> keysPressed)
        {
            if (keysPressed.Any())
            {
                keysPressed.RemoveAt(keysPressed.Count - 1);

                RedrawNode(true);
                PrepareForAction(true);
            }
        }
        internal string[] ExtractWords() 
        {
            //reconstruct
            string typed = string.Empty;

            for (int i = 0; i < keysPressed.Count; i++)
                typed += keysPressed[i].KeyChar.ToString().ToLower();

            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '!', '\r' };

            return typed.Split(delimiterChars);
        }
        internal virtual void EnterPressed(List<ConsoleKeyInfo> keysPressed)
        {
            bool isFirstTimeDisplayed = true;

            if (keysPressed.Any())
            {
                act = null;

                string[] words = ExtractWords();
               
                keysPressed.Clear();                                            //clear the stack after giving command

                string matchingVerb = string.Empty;

                foreach (string word in words)                                  //is there one word matching one action?
                {
                    foreach (Action action in Actions)
                    {
                        if  (action.Verbs.Contains(word))
                        {
                            act = action;
                            matchingVerb = word;                                //store the typed verb which triggered the action
                            break;
                        }
                    }
                }

                if (act != null)                                                //if there's an action available...
                {
                    if (!act.Objects.Any())                                     //...and is objectless...
                        ProcessAction(act);
                    else
                    {                                                           //...otherwise, examine Objects 
                        foreach (Object o in act.Objects)
                            foreach (string word in words)                      //is there a matching object available? just hand me the first you find please
                                if (o.Objs.Contains(word))
                                    ProcessAction(o);                           //the action is right, and there is a acceptable object specified

                        if (act.Answer != null)
                            DisplaySuccess(act.Answer, act.ChildId);
                        else
                            CustomRefusal(act.GetOpinion(matchingVerb));        //the action is right, but no required object is specified
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
            if (!DataLayer.Evaluate(action.Condition))              //if for some reason Kriss can't do it, say it...
                CustomRefusal(action.Condition.Refusal);
            else                                                    //...otherwise, do it
            {
                if (action.Effect != null)                          //in case the obj has an Effect (inventory)
                    DataLayer.StoreItem(action.Effect);

                DisplaySuccess(action.Answer, action.ChildId);
            }
        }

        void CustomRefusal(string refusal)
        {
            RedrawNode();

            Console.CursorTop = Console.WindowHeight - 4;
            Console.CursorLeft = Console.WindowLeft;

            refusal = "<<" + refusal + ">>";

            BottomMessage = refusal;
            BottomMessageColor = ConsoleColor.Cyan;

            Typist.FlowingText(refusal, BottomMessageColor);
            Console.WriteLine();
            Console.WriteLine();

            PrepareForAction(true); //display prompt without standard refuse
        }
        void DisplaySuccess(string answer, int? childId = null) 
        {
            if (answer != null)
            {
                RedrawNode();

                BottomMessage = answer;
                BottomMessageColor = ConsoleColor.DarkYellow;

                Console.CursorTop = MeasureMessage(answer);
                Console.CursorLeft = Console.WindowLeft;

                Typist.FlowingText(answer, BottomMessageColor);

                if (childId.HasValue)
                {
                    Typist.WaitForKey(3);

                    this.AdvanceToNext(childId.Value);
                }
            }
            if (childId.HasValue)
               this.AdvanceToNext(childId.Value);

            //if everything fails:
            PrepareForAction(true); //display prompt without standard refuse
        }
        internal void RedrawNode(bool isDeleting = false)
        {
            Console.Clear();
            Typist.InstantText(Text);

            if (isDeleting)
                if (!string.IsNullOrWhiteSpace(BottomMessage))                      //decide if there is a Bottom Message and of which type
                {
                    switch (BottomMessageColor)
                    {
                        case ConsoleColor.DarkYellow:
                            Console.CursorTop = MeasureMessage(BottomMessage);
                            Console.CursorLeft = Console.WindowLeft;
                            break;

                        case ConsoleColor.Cyan:
                            Console.CursorTop = Console.WindowHeight - 4;
                            Console.CursorLeft = Console.WindowLeft;
                            break;
                    }
                    Typist.InstantText(BottomMessage, BottomMessageColor);
                }
        }

        internal static int MeasureMessage(string answer)
        {
            //measure the lenght and the newlines in the answer to determine how up to go to start writing
            int newLines = System.Text.RegularExpressions.Regex.Matches(answer, "\\n").Count;
            int rows = answer.Length / Console.WindowWidth;

            return Math.Min(Console.WindowHeight - (rows + newLines), Console.WindowHeight - 5) - 2;
        }
    }
}
