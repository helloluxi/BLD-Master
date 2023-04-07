using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Luxi.Move;
using static Luxi.Tools;

namespace Luxi
{
    public class Edge
    {
        public State[] state;

        public Edge() => state = Enumerable.Range(0, 12).Select(i => new State{ perm = i, ori = 0 }).ToArray();
        public State this[int perm, int ori] => new State { perm = state[perm].perm, ori = state[perm].ori ^ ori };
        public static Edge Random()
        {
            int epVal = Tools.rd.Next(479001600), eoVal = Tools.rd.Next(2048);
            Tools.SetNPerm(epVal, 12, out int[] ep);
            Tools.SetNFlip(eoVal, 12, out int[] eo);
            return new Edge { state = ep.Zip(eo, (p, o) => new State{ perm = p, ori = o }).ToArray() };
        }
        public Edge Copy() => new Edge { state = state.ToArray() };
        public void Solve(){
            for (int i = 0; i < 12; i++){
                state[i].perm = i;
                state[i].ori = 0;
            }
        }
        public bool IsSolved(){
            for (int i = 0; i < 12; i++){
                if (state[i].perm != i || state[i].ori != 0)
                    return false;
            }
            return true;
        }
        
        private void Cycle4(int p0, int p1, int p2, int p3, int o)
        {
            int t = state[p0].perm;
            state[p0].perm = state[p3].perm;
            state[p3].perm = state[p2].perm;
            state[p2].perm = state[p1].perm;
            state[p1].perm = t;

            t = state[p0].ori;
            state[p0].ori = state[p3].ori ^ o;
            state[p3].ori = state[p2].ori ^ o;
            state[p2].ori = state[p1].ori ^ o;
            state[p1].ori = t ^ o;
        }
        private void Swap(int p0, int p1, int o = 0)
        {
            int t = state[p0].perm;
            state[p0].perm = state[p1].perm;
            state[p1].perm = t;

            t = state[p0].ori;
            state[p0].ori = state[p1].ori ^ o;
            state[p1].ori = t ^ o;
        }
        public void Turn(Move m)
        {
            switch (m)
            {
                case U: Cycle4(0, 1, 2, 3, 0); break;
                case U2: Swap(0, 2); Swap(1, 3); break;
                case U_: Cycle4(0, 3, 2, 1, 0); break;
                case D: Cycle4(4, 7, 6, 5, 0); break;
                case D2: Swap(4, 6); Swap(7, 5); break;
                case D_: Cycle4(4, 5, 6, 7, 0); break;
                case R: Cycle4(3, 11, 7, 8, 0); break;
                case R2: Swap(3, 7); Swap(11, 8); break;
                case R_: Cycle4(3, 8, 7, 11, 0); break;
                case L: Cycle4(1, 9, 5, 10, 0); break;
                case L2: Swap(1, 5); Swap(9, 10); break;
                case L_: Cycle4(1, 10, 5, 9, 0); break;
                case F: Cycle4(0, 8, 4, 9, 1); break;
                case F2: Swap(0, 4); Swap(8, 9); break;
                case F_: Cycle4(0, 9, 4, 8, 1); break;
                case B: Cycle4(2, 10, 6, 11, 1); break;
                case B2: Swap(2, 6); Swap(10, 11); break;
                case B_: Cycle4(2, 11, 6, 10, 1); break;
                case E: Cycle4(8, 11, 10, 9, 1); break;
                case E2: Swap(8, 10); Swap(11, 9); break;
                case E_: Cycle4(8, 9, 10, 11, 1); break;
                case M: Cycle4(0, 4, 6, 2, 1); break;
                case M2: Swap(0, 6); Swap(4, 2); break;
                case M_: Cycle4(0, 2, 6, 4, 1); break;
                case S: Cycle4(1, 3, 7, 5, 1); break;
                case S2: Swap(1, 7); Swap(5, 3); break;
                case S_: Cycle4(1, 5, 7, 3, 1); break;
                case x: Turn(R); Turn(L_); Turn(M_); break;
                case x2: Turn(R2); Turn(L2); Turn(M2); break;
                case x_: Turn(R_); Turn(L); Turn(M); break;
                case y: Turn(U); Turn(D_); Turn(E_); break;
                case y2: Turn(U2); Turn(D2); Turn(E2); break;
                case y_: Turn(U_); Turn(D); Turn(E); break;
                case z: Turn(F); Turn(B_); Turn(S); break;
                case z2: Turn(F2); Turn(B2); Turn(S2); break;
                case z_: Turn(F_); Turn(B); Turn(S_); break;
                default: break;
            }
        }
    }
}
