using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Luxi.Move;
using static Luxi.Tools;

namespace Luxi
{
    public class XCenter
    {
        private int[] state = new int[24];
        public XCenter() => Solve();
        public int this[int index] => state[index];
        public void Solve(){
            for (int i = 0; i < 24; i++)
                state[i] = i >> 2;
        }
        public bool IsSolved()
        {
            for (int i = 0; i < 24; i++)
                if (state[i] != (i >> 2))
                    return false;
            return true;
        }
        public void Cycle(int p0, int p1, int p2, int p3)
        {
            int t = state[p0];
            state[p0] = state[p3];
            state[p3] = state[p2];
            state[p2] = state[p1];
            state[p1] = t;
        }
        public void Swap(int p0, int p1)
        {
            int t = state[p0];
            state[p0] = state[p1];
            state[p1] = t;
        }
        public static XCenter Random()
        {
            return new XCenter { state = Wing.Random().state.Select(i => i >> 2).ToArray() };
        }
        public void Turn(Move m)
        {
            switch (m)
            {
                case U: Cycle(0, 1, 2, 3); break;
                case U2: Swap(0, 2); Swap(1, 3); break;
                case U_: Cycle(0, 3, 2, 1); break;
                case u: Cycle(9, 17, 21, 5); Cycle(8, 16, 20, 4); break;
                case u2: Swap(9, 21); Swap(17, 5); Swap(8, 20); Swap(4, 16); break;
                case u_: Cycle(9, 5, 21, 17); Cycle(8, 4, 20, 16); break;
                case D: Cycle(12, 13, 14, 15); break;
                case D2: Swap(12, 14); Swap(13, 15); break;
                case D_: Cycle(12, 15, 14, 13); break;
                case d: Cycle(10, 6, 22, 18); Cycle(11, 7, 23, 19); break;
                case d2: Swap(10, 22); Swap(6, 18); Swap(11, 23); Swap(7, 19); break;
                case d_: Cycle(10, 18, 22, 6); Cycle(11, 19, 23, 7); break;
                case R: Cycle(4, 5, 6, 7); break;
                case R2: Swap(4, 6); Swap(5, 7); break;
                case R_: Cycle(4, 7, 6, 5); break;
                case r: Cycle(0, 20, 14, 10); Cycle(3, 23, 13, 9); break;
                case r2: Swap(0, 14); Swap(20, 10); Swap(3, 13); Swap(23, 9); break;
                case r_: Cycle(0, 10, 14, 20); Cycle(3, 9, 13, 23); break;
                case L: Cycle(16, 17, 18, 19); break;
                case L2: Swap(16, 18); Swap(17, 19); break;
                case L_: Cycle(16, 19, 18, 17); break;
                case l: Cycle(1, 11, 15, 21); Cycle(2, 8, 12, 22); break;
                case l2: Swap(1, 15); Swap(11, 21); Swap(2, 12); Swap(8, 22); break;
                case l_: Cycle(1, 21, 15, 11); Cycle(2, 22, 12, 8); break;
                case F: Cycle(8, 9, 10, 11); break;
                case F2: Swap(8, 10); Swap(9, 11); break;
                case F_: Cycle(8, 11, 10, 9); break;
                case f: Cycle(0, 7, 12, 17); Cycle(1, 4, 13, 18); break;
                case f2: Swap(0, 12); Swap(7, 17); Swap(1, 13); Swap(4, 18); break;
                case f_: Cycle(0, 17, 12, 7); Cycle(1, 18, 13, 4); break;
                case B: Cycle(20, 21, 22, 23); break;
                case B2: Swap(20, 22); Swap(21, 23); break;
                case B_:  Cycle(20, 23, 22, 21); break;
                case b: Cycle(2, 19, 14, 5); Cycle(3, 16, 15, 6); break;
                case b2: Swap(2, 14); Swap(19, 5); Swap(3, 15); Swap(16, 6); break;
                case b_: Cycle(2, 5, 14, 19); Cycle(3, 6, 15, 16); break;
                case x: Turn(R); Turn(r); Turn(l_); Turn(L_); break;
                case x2: Turn(R2); Turn(r2); Turn(l2); Turn(L2); break;
                case x_: Turn(R_); Turn(r_); Turn(l); Turn(L); break;
                case y: Turn(U); Turn(u); Turn(d_); Turn(D_); break;
                case y2: Turn(U2); Turn(u2); Turn(d2); Turn(D2); break;
                case y_: Turn(U_); Turn(u_); Turn(d); Turn(D); break;
                case z: Turn(F); Turn(f); Turn(b_); Turn(B_); break;
                case z2: Turn(F2); Turn(f2); Turn(b2); Turn(B2); break;
                case z_: Turn(F_); Turn(f_); Turn(b); Turn(B); break;
                default: break;
            }
        }
    }
}
