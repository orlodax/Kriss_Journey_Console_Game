using ConsoleGame.Classes;
using ConsoleGame.Models;
using System;
using System.Collections.Generic;

namespace ConsoleGame.Nodes
{
    public class NAction : SNode
    {
        Models.Action act = null;

        public NAction(NodeBase nb) : base(nb)
        {
            PrepareForAction(true);
        }
        void PrepareForAction(bool isFirst)
        {
            ///go to bottom line and prepare prompt
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 2;
            Console.CursorLeft = Console.WindowLeft + Console.WindowWidth - 1;

            Console.ForegroundColor = ConsoleColor.Gray;
            if (!isFirst)
            {
                Console.CursorTop -= 1;
                Console.WriteLine(" You can't or won't do that. Try again.");
            }
            Console.Write(" \\> ");

            List<ConsoleKeyInfo> keysPressed = new List<ConsoleKeyInfo>();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey();
                if (key.Key.Equals(ConsoleKey.Tab)) //if player presses tabs looking for help
                {
                    TabPressed();
                }
                else if (key.Key.Equals(ConsoleKey.Backspace))  //to erase
                {
                    BackSpacePressed(keysPressed);
                }
                else
                    if (!key.Key.Equals(ConsoleKey.Enter))  //normal keys are registered
                        keysPressed.Add(key);

                if (key.Key == ConsoleKey.Enter)
                {
                    EnterPressed(keysPressed);
                }

            } while (true);
        }

        void TabPressed() 
        {
            Console.CursorLeft -= 5;
            Console.CursorTop -= 3;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Possible actions here: ");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            foreach (var action in Actions)
                Console.Write(action.Verb + " ");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.CursorTop += 2;
            Console.CursorLeft = 0;

            if (ID == "1_02") //first action node. this if clause is to mock player just the first time they use help
            {
                Console.WriteLine("\\> you pressed tab for help. noob.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\\>");
                Console.CursorLeft += 3;
            }

            Console.CursorLeft = Console.WindowLeft + 3;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        void BackSpacePressed(List<ConsoleKeyInfo> keysPressed)
        {
            if (keysPressed.Count > 0)
            {
                keysPressed.RemoveAt(keysPressed.Count - 1);
                Console.Write("\b");
                Console.Write(" ");
                Console.Write("\b");
            }
        }
        void EnterPressed(List<ConsoleKeyInfo> keysPressed)
        {
            act = null;
            //reconstruct
            string typed = string.Empty;

            for (int i = 0; i < keysPressed.Count; i++)
                typed += keysPressed[i].KeyChar.ToString().ToLower();

            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '!', '\r' };

            string[] words = typed.Split(delimiterChars);

            foreach (string word in words)                                  //is there one word matching one action?
            {
                act = Actions.Find(a => a.Verb == word) ?? act;             //and stop at the first you find
                if (act != null)
                    break;
            }

            if (act != null)                                                //if there's an action available...
            {
                if (act.Objects.Count == 0)                                 //...and is objectless...
                {
                    if (!act.EvaluateSimple())                              //if for some reason Kriss can't do it, say it...
                        CustomRefusal(act.Condition.Refusal);
                    else
                    {
                        if (act.Answer != null)
                            DisplaySuccess(act.Answer);

                        if (act.Effect != null)                             //in case the action has an Effect (inventory)
                            act.StoreItem(act.Effect);

                        SaveStatusOnExit();                                 //...just do it
                        NodeFactory.CreateNode(act.ChildId);
                    }
                }
                else
                {                                                           //...otherwise, examine Objects 
                    for (int i = 0; i < act.Objects.Count; i++)             //this is not O^2. only iterates over <10 x <10 items 
                    {
                        Models.Object o = act.Objects[i];

                        foreach (string word in words)                      //is there a matching object available? just hand me the first you find please
                        {
                            if (word == o.Obj)                              //the action is right, and there is a acceptable object specified
                            {
                                if (!act.EvaluateCombination(o))            //if for some reason Kriss can't do it, say it...
                                    CustomRefusal(o.Condition.Refusal);
                                    
                                else                                        //...otherwise, do it
                                {
                                    if (o.Effect != null)                   //in case the obj has an Effect (inventory)
                                        act.StoreItem(o.Effect);

                                    if (o.ChildId != null)                  //if the action leads to another node
                                    {
                                        SaveStatusOnExit();
                                        NodeFactory.CreateNode(o.ChildId);
                                    }
                                    else                                    //if the action causes only an effect
                                        DisplaySuccess(o.Answer);
                                }
                            }
                        }
                    }
                    CustomRefusal(act.GetOpinion());                         //the action is right, but no required object is specified
                }
            }
            else                                                            //if there's no action available, redraw node and display standard refuse
            {
                RedrawNode();
                PrepareForAction(false);
            }
        }
        void CustomRefusal(string refusal)
        {
            RedrawNode();

            Console.CursorTop = Console.WindowHeight - 4;
            Console.CursorLeft = Console.WindowLeft;

            Console.ForegroundColor = ConsoleColor.Cyan;
            TextFlow(true, "<<" + refusal + ">>");
            Console.WriteLine();
            Console.WriteLine();

            PrepareForAction(true); //display prompt without standard refuse
        }
        void DisplaySuccess(string answer) 
        {
            RedrawNode();

            //measure the lenght and the newlines in the answer to determine how up to go to start writing
            var newLines = System.Text.RegularExpressions.Regex.Matches(answer, "\\n").Count;
            var rows = answer.Length / Console.WindowWidth;

            var offset = Math.Min(Console.WindowHeight - (rows + newLines), Console.WindowHeight - 5);

            Console.CursorTop = offset;
            Console.CursorLeft = Console.WindowLeft;

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            TextFlow(true, answer);
            Console.WriteLine();
            Console.WriteLine();

            PrepareForAction(true); //display prompt without standard refuse
        }
        void RedrawNode()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            TextFlow(false);
        }
    }
}
