using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Classes
{
    public class BaseInput
    {
        internal void WriteOnBottomLine()
        {
            ConsoleKeyInfo keyinfo;

            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            ///go to bottom line
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 2;
            Console.CursorLeft = Console.WindowLeft + Console.WindowWidth - 1;

            Console.Write(" \\> ");
            keyinfo = Console.ReadKey();

            if (keyinfo.Key.Equals(ConsoleKey.Tab))
            {
                Console.CursorLeft -= 5;
                Console.CursorTop -= 1;
                Console.WriteLine("Possible actions here: ");

                foreach (Enums.Actions action in (Enums.Actions[])Enum.GetValues(typeof(Enums.Actions)))
                    Console.Write(action + " ");

                Console.CursorTop += 1;
                Console.CursorLeft = 0;
                Console.WriteLine(" \\> you pressed tab for help. noob.");
            }
            // Restore previous position
            //  Console.SetCursorPosition(x, y);
        }
    }
}
