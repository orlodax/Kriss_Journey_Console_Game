using ConsoleGame.Nodes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ConsoleGame.Classes
{
    public class Responder
    {
        NAction Node;
        public Responder(NAction node)
        {
            Node = node;
        }

        public Tuple<int, int> TryAction(string typed)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '!', '\r' };
            
            string[] words = typed.Split(delimiterChars);

            Action act = null;
            Object obj = null;

            foreach (string word in words)
            {
                act = Node.Actions.Find(a => a.verb == word) ?? act;
                obj = Node.Objects.Find(o => o.obj == word) ?? obj;
            }

            int idAct = -1;
            int idObj = -1;

            if (act != null)
                idAct = Node.Actions.FindIndex(a => a == act);
            if (obj != null)
                idObj = Node.Objects.FindIndex(o => o == obj);

            return new Tuple<int, int>(idAct, idObj);
        }
    }
}