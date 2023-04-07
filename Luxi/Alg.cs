using System.Collections.Generic;

namespace Luxi
{
    public class Alg : List<Move>
    {
        public Alg() { }
        public Alg(string s)
        {
            Queue<int> q3 = new Queue<int>(), q4 = new Queue<int>();
            foreach (char c in s)
                switch (c)
                {
                    case 'U': Add(Move.U); break;
                    case 'u': Add(Move.u); break;
                    case 'D': Add(Move.D); break;
                    case 'd': Add(Move.d); break;
                    case 'R': Add(Move.R); break;
                    case 'r': Add(Move.r); break;
                    case 'L': Add(Move.L); break;
                    case 'l': Add(Move.l); break;
                    case 'F': Add(Move.F); break;
                    case 'f': Add(Move.f); break;
                    case 'B': Add(Move.B); break;
                    case 'b': Add(Move.b); break;
                    case 'E': Add(Move.E); break;
                    case 'y': Add(Move.y); break;
                    case 'M': Add(Move.M); break;
                    case 'x': Add(Move.x); break;
                    case 'S': Add(Move.S); break;
                    case 'z': Add(Move.z); break;
                    case '3': q3.Enqueue(Count); break;
                    case '4': q4.Enqueue(Count); break;
                    default:
                        if (Count == 0) break;
                        int i = (int)this[Count - 1];
                        if (c == '2') this[Count - 1] = (Move)(i / 3 * 3 + 1);
                        else if (c == '\'' && i % 3 == 0) this[Count - 1] = (Move)(i + 2);
                        else if (c == 'w' && i % 18 == 0) this[Count - 1] = (Move)(i + 6);
                        break;
                }
            foreach (var i in q3)
                if (i < Count && ((int)this[i]) % 18 / 3 == 2)
                    this[i] = (Move)((int)this[i] + 3);
            foreach (var i in q4)
                if (i < Count && ((int)this[i]) % 18 / 3 == 2)
                    this[i] = (Move)((int)this[i] + 6);
        }
        public override string ToString()
        {
            string[] strings = new string[Count];
            for (int i = 0; i < Count; i++)
                strings[i] = this[i].ToString().TrimStart('_').Replace('_', '\'');
            return string.Join(" ", strings);
        }
    }
}
