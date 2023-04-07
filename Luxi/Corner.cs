using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Luxi.Move;
using static Luxi.Tools;

namespace Luxi
{
    public class Corner
    {
        public State[] state;

        public Corner() => state = Enumerable.Range(0, 8).Select(i => new State{ perm = i, ori = 0 }).ToArray();
        public State this[int perm, int ori] => new State { perm = state[perm].perm, ori = state[perm].ori ^ ori };
        public static Corner Random()
        {
            int cpVal = Tools.rd.Next(40320), coVal = Tools.rd.Next(2187);
            Tools.SetNPerm(cpVal, 8, out int[] ep);
            Tools.SetNTwist(coVal, 8, out int[] eo);
            return new Corner { state = ep.Zip(eo, (p, o) => new State{ perm = p, ori = o }).ToArray() };
        }
        public Corner Copy() => new Corner { state = state.ToArray() };
        public void Solve(){
            for (int i = 0; i < 8; i++){
                state[i].perm = i;
                state[i].ori = 0;
            }
        }
        public bool IsSolved(){
            for (int i = 0; i < 8; i++){
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
            state[p0].ori = (state[p3].ori + o) % 3;
            state[p3].ori = (state[p2].ori + 3 - o) % 3;
            state[p2].ori = (state[p1].ori + o) % 3;
            state[p1].ori = (t + 3 - o) % 3;
        }
        private void Swap(int p0, int p1, int o = 0)
        {
            int t = state[p0].perm;
            state[p0].perm = state[p1].perm;
            state[p1].perm = t;

            t = state[p0].ori;
            state[p0].ori = (state[p1].ori + o) % 3;
            state[p1].ori = (t + 3 - o) % 3;
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
                case R: Cycle4(3, 2, 6, 7, 1); break;
                case R2: Swap(3, 6); Swap(2, 7); break;
                case R_: Cycle4(3, 7, 6, 2, 1); break;
                case L: Cycle4(0, 4, 5, 1, 2); break;
                case L2: Swap(0, 5); Swap(4, 1); break;
                case L_: Cycle4(0, 1, 5, 4, 2); break;
                case F: Cycle4(0, 3, 7, 4, 1); break;
                case F2: Swap(0, 7); Swap(3, 4); break;
                case F_: Cycle4(0, 4, 7, 3, 1); break;
                case B: Cycle4(1, 5, 6, 2, 2); break;
                case B2: Swap(1, 6); Swap(5, 2); break;
                case B_: Cycle4(1, 2, 6, 5, 2); break;
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

