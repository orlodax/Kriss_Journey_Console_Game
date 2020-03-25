using ConsoleGame.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Nodes
{
    public class NAction : SNode
    {
        Responder Responder;
        public NAction(NodeBase nb) : base(nb)
        {
            Responder = new Responder(this);
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

            do {
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
                else
                    if (!key.Key.Equals(ConsoleKey.Enter))  //normal keys are registered
                        keysPressed.Add(key);

            } while (key.Key != ConsoleKey.Enter);

            //reconstruct
            string typed = string.Empty;

            for (int i = 0; i < keysPressed.Count; i++)
                typed += keysPressed[i].KeyChar.ToString();

            //try action
            Tuple<int, int> ids = Responder.TryAction(typed);
                
            int actId = ids.Item1;
            int objId = ids.Item2;

            // for when I'll have multiple combinations
            //if (actId > -1 && objId > -1)

            int childId = Math.Max(actId, objId);

            if (childId < 0)
            {
                //reload and retry
                Console.Clear();
                TextFlow(false);
                PrepareForAction(false);
            }
            else
            {
                Destructor();
                NodeFactory.CreateNode(Children[childId].id);
            }
        }
    }
}
