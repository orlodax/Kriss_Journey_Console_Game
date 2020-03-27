using ConsoleGame.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Nodes
{
    public class NAction : SNode
    {
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
                    Console.CursorLeft -= 5;
                    Console.CursorTop -= 3;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Possible actions here: ");

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    foreach (var action in Actions)
                        Console.Write(action.verb + " ");

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
                else if (key.Key.Equals(ConsoleKey.Backspace))  //to erase
                {
                    if (keysPressed.Count > 0)
                    {
                        keysPressed.RemoveAt(keysPressed.Count - 1);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft -= 1, Console.CursorTop);
                    }
                }
                else
                    if (!key.Key.Equals(ConsoleKey.Enter))  //normal keys are registered
                        keysPressed.Add(key);

                if (key.Key == ConsoleKey.Enter)
                {
                    //reconstruct
                    string typed = string.Empty;

                    for (int i = 0; i < keysPressed.Count; i++)
                        typed += keysPressed[i].KeyChar.ToString().ToLower();

                    string id = TryAction(typed);

                    if (String.IsNullOrWhiteSpace(id))
                    {
                        Console.Clear();
                        TextFlow(false);
                        PrepareForAction(false);
                    }
                    else
                    {
                        Destructor();
                        NodeFactory.CreateNode(id);
                    }
                }

            } while (true);
        }
        public string TryAction(string typed)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '!', '\r' };

            string[] words = typed.Split(delimiterChars);

            Classes.Action act = null;

            foreach (string word in words)
                act = Actions.Find(a => a.verb == word) ?? act;    //is there a matching action available?

            if (act != null)
            {
                if (act.objects.Count > 0)
                {
                    for (int i = 0; i < act.objects.Count; i++) //this is not O^2. only iterates over <10 x <10 items 
                        foreach (string word in words)   //is there a matching object available? just hand me the first you find please
                        { 
                            if (word == act.objects[i].obj)
                            {
                                if (!act.Evaluate())
                                {
                                    //Console.CursorLeft -= 5;
                                    Console.CursorTop -= 3;

                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.WriteLine(act.condition.refusal);
                                    Console.WriteLine("Press any key.");

                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.CursorTop += 2;
                                    Console.CursorLeft = 0;

                                    Console.CursorLeft = Console.WindowLeft + 3;
                                    Console.ForegroundColor = ConsoleColor.Gray;

                                    //TODO
                                    //e non lo so 
                                    Console.Clear();
                                    TextFlow(false);
                                    PrepareForAction(false);

                                    return act.objects[i].childid;
                                }
                            }
                           
                        }
                    return act.defaultanswer;
                }
                else
                    return act.childid; //if the action has no objects, return its own (and only) childid
            }
            else
                return string.Empty;
        }
    }
}
