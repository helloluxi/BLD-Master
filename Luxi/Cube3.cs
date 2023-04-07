using System.Collections.Generic;
using System.Text;
using CS.Min2Phase;
using static Luxi.Move;
using static Luxi.Tools;

namespace Luxi
{
    public class Cube3
    {
        public Edge edge;
        public Corner corner;
        private const string faces = "urfdlb";
        private readonly static Search search = new();
        private static readonly Alg[] choice1 = { new Alg(), new Alg{Rw},
            new Alg{Rw2}, new Alg{Rw_}, new Alg{Fw}, new Alg{Fw_} },
            choice2 = { new Alg(), new Alg { Uw }, new Alg { Uw2 }, new Alg { Uw_ } },
            suffix = { new Alg(), new Alg{D_}, new Alg{D2}, new Alg{D},
                    new Alg{L_}, new Alg{B_, L_}, new Alg{B2, L_}, new Alg{B, L_},
                    new Alg{L2}, new Alg{U_, L2}, new Alg{U2, L2}, new Alg{U, L2},
                    new Alg{L}, new Alg{F_, L}, new Alg{F2, L}, new Alg{F, L},
                    new Alg{B_}, new Alg{R_, B_}, new Alg{R2, B_}, new Alg{R, B_},
                    new Alg{B}, new Alg{L_, B}, new Alg{L2, B}, new Alg{L, B}
            };
        
        public Cube3()
        {
            edge = new Edge();
            corner = new Corner();
        }
        public static Cube3 RandomCube3(bool edge=true, bool corner=true)
        {
            Cube3 cube = new Cube3{
                edge = edge ? Edge.Random() : new Edge(),
                corner = corner ? Corner.Random() : new Corner()
            };
            // Random parity
            if (edge && corner && rd.Next(2) == 0)
                cube.Turn(U);
            return cube;
        }
        public void Turn(IEnumerable<Move> s)
        {
            foreach (Move m in s) Turn(m);
        }
        public void Turn(Move m)
        {
            switch (m)
            {
                case U: edge.Turn(U); corner.Turn(U); break;
                case U2: edge.Turn(U2); corner.Turn(U2); break;
                case U_: edge.Turn(U_); corner.Turn(U_); break;
                case D: edge.Turn(D); corner.Turn(D); break;
                case D2: edge.Turn(D2); corner.Turn(D2); break;
                case D_: edge.Turn(D_); corner.Turn(D_); break;
                case R: edge.Turn(R); corner.Turn(R); break;
                case R2: edge.Turn(R2); corner.Turn(R2); break;
                case R_: edge.Turn(R_); corner.Turn(R_); break;
                case L: edge.Turn(L); corner.Turn(L); break;
                case L2: edge.Turn(L2); corner.Turn(L2); break;
                case L_: edge.Turn(L_); corner.Turn(L_); break;
                case F: edge.Turn(F); corner.Turn(F); break;
                case F2: edge.Turn(F2); corner.Turn(F2); break;
                case F_: edge.Turn(F_); corner.Turn(F_); break;
                case B: edge.Turn(B); corner.Turn(B); break;
                case B2: edge.Turn(B2); corner.Turn(B2); break;
                case B_: edge.Turn(B_); corner.Turn(B_); break;
                case E: edge.Turn(E); break;
                case E2: edge.Turn(E2); break;
                case E_: edge.Turn(E_); break;
                case M: edge.Turn(M); break;
                case M2: edge.Turn(M2); break;
                case M_: edge.Turn(M_); break;
                case S: edge.Turn(S); break;
                case S2: edge.Turn(S2); break;
                case S_: edge.Turn(S_); break;
                case Uw: case u: Turn(U); Turn(E_); break;
                case Uw2: case u2: Turn(U2); Turn(E2); break;
                case Uw_: case u_: Turn(U_); Turn(E); break;
                case Dw: case d: Turn(D); Turn(E); break;
                case Dw2: case d2: Turn(D2); Turn(E2); break;
                case Dw_: case d_: Turn(D_); Turn(E_); break;
                case Fw: case f: Turn(F); Turn(S); break;
                case Fw2: case f2: Turn(F2); Turn(S2); break;
                case Fw_: case f_: Turn(F_); Turn(S_); break;
                case Bw: case b: Turn(B); Turn(S_); break;
                case Bw2: case b2: Turn(B2); Turn(S2); break;
                case Bw_: case b_: Turn(B_); Turn(S); break;
                case Rw: case r: Turn(R); Turn(M_); break;
                case Rw2: case r2: Turn(R2); Turn(M2); break;
                case Rw_: case r_: Turn(R_); Turn(M); break;
                case Lw: case l: Turn(L); Turn(M); break;
                case Lw2: case l2: Turn(L2); Turn(M2); break;
                case Lw_: case l_: Turn(L_); Turn(M_); break;
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
        public void Solve()
        {
            edge.Solve();
            corner.Solve();
        }
        public bool IsSolved() => edge.IsSolved() && corner.IsSolved();
        public string GetScramble() => search.Solution(ToString(), 21, 100000000, 0, 2);
        public string GetScramble(int bfOri)
        {
            Cube3 t = new Cube3(){
                edge = edge.Copy(),
                corner = corner.Copy()
            };
            t.Turn(suffix[bfOri]);
            Alg s = new Alg(t.GetScramble());
            s.AddRange(choice1[bfOri / 4]);
            s.AddRange(choice2[bfOri % 4]);
            return s.ToString();
        }
        
        
        private static readonly int[] EdgeIndex = {
            7, 19, 3, 37, 1, 46, 5, 10, 28, 25, 30, 43, 34, 52, 32, 16, 23, 12, 21, 41, 50, 39, 48, 14
        }, CornerIndex = {
            6, 18, 38, 0, 36, 47, 2, 45, 11, 8, 9, 20, 27, 44, 24, 33, 53, 42, 35, 17, 51, 29, 26, 15
        };

        public override string ToString()
        {
            char[] state = new char[54];
            for (int i = 0; i < 12; i++)
            {
                State s = edge[i, 0];
                state[EdgeIndex[i * 2]] = faces[EdgeIndex[s.perm << 1 | s.ori] / 9];
                state[EdgeIndex[i * 2 + 1]] = faces[EdgeIndex[s.perm << 1 | s.ori ^ 1] / 9];
            }
            for (int i = 0; i < 8; i++)
            {
                State s = corner[i, 0];
                state[CornerIndex[i * 3]] = faces[CornerIndex[s.perm * 3 + s.ori] / 9];
                state[CornerIndex[i * 3 + 1]] = faces[CornerIndex[s.perm * 3 + (s.ori + 1) % 3] / 9];
                state[CornerIndex[i * 3 + 2]] = faces[CornerIndex[s.perm * 3 + (s.ori + 2) % 3] / 9];
            }
            for (int i = 0; i < 6; i++)
            {
                state[9 * i + 4] = faces[i];
            }
            return new string(state);
        }
    }
}