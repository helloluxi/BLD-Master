using System;
using System.Collections.Generic;
using System.Text;
using static CS.ThreePhase.Center1;
using static CS.ThreePhase.Moves;

namespace CS.ThreePhase
{
    /*
    Edge Cubies: 
					    14	2	
				    1			15
				    13			3
					    0	12	
	    1	13			0	12			3	15			2	14	
    9			20	20			11	11			22	22			9
    21			8	8			23	23			10	10			21
	    17	5			18	6			19	7			16	4	
					    18	6	
				    5			19
				    17			7
					    4	16	

    Center Cubies: 
			    0	1
			    3	2

    20	21		8	9		16	17		12	13
    23	22		11	10		19	18		15	14

			    4	5
			    7	6

                |************|
                |*U1**U2**U3*|
                |************|
                |*U4**U5**U6*|
                |************|
                |*U7**U8**U9*|
                |************|
    ************|************|************|************|
    *L1**L2**L3*|*F1**F2**F3*|*R1**R2**F3*|*B1**B2**B3*|
    ************|************|************|************|
    *L4**L5**L6*|*F4**F5**F6*|*R4**R5**R6*|*B4**B5**B6*|
    ************|************|************|************|
    *L7**L8**L9*|*F7**F8**F9*|*R7**R8**R9*|*B7**B8**B9*|
    ************|************|************|************|
                |************|
                |*D1**D2**D3*|
                |************|
                |*D4**D5**D6*|
                |************|
                |*D7**D8**D9*|
                |************|


                    |****************|
                    |*u0**u1**u2**u3*|
                    |****************|
                    |*u4**u5**u6**u7*|
                    |****************|
                    |*u8**u9**ua**ub*|
                    |****************|
                    |*uc**ud**ue**uf*|
                    |****************|
    ****************|****************|****************|****************|
    *l0**l1**l2**l3*|*f0**f1**f2**f3*|*r0**r1**r2**r3*|*b0**b1**b2**b3*|
    ****************|****************|****************|****************|
    *l4**l5**l6**l7*|*f4**f5**f6**f7*|*r4**r5**r6**r7*|*b4**b5**b6**b7*|
    ****************|****************|****************|****************|
    *l8**l9**la**lb*|*f8**f9**fa**fb*|*r8**r9**ra**rb*|*b8**b9**ba**bb*|
    ****************|****************|****************|****************|
    *lc**ld**le**lf*|*fc**fd**fe**ff*|*rc**rd**re**rf*|*bc**bd**be**bf*|
    ****************|****************|****************|****************|
                    |****************|
                    |*d0**d1**d2**d3*|
                    |****************|
                    |*d4**d5**d6**d7*|
                    |****************|
                    |*d8**d9**da**db*|
                    |****************|
                    |*dc**dd**de**df*|
                    |****************|
	 */
    public class FullCube : IComparable<FullCube>
    {
        public static readonly sbyte[] centerFacelet = { u5, u6, ua,
            u9, d5, d6, da, d9, f5, f6, fa,
            f9, b5, b6, ba, b9, r5, r6, ra,
            r9, l5, l6, la, l9 };
        public static readonly sbyte[,] edgeFacelet = {
        { ud, f1}, { u4, l1}, { u2, b1},
        { ub, r1}, { dd, be}, { d4, le},
        { d2, fe}, { db, re}, { lb, f8},
        { l4, b7}, { rb, b8}, { r4, f7},
        { f2, ue}, { l2, u8}, { b2, u1},
        { r2, u7}, { bd, de}, { ld, d8},
        { fd, d1}, { rd, d7}, { f4, l7},
        { bb, l8}, { b4, r7}, { fb, r8}};
        public static readonly sbyte[,] cornerFacelet = { {  uf, r0, f3 },
            {  uc, f0, l3 }, {  u0, l0, b3 },
            {  u3, b0, r3 }, {  d3, ff, rc },
            {  d0, lf, fc }, {  dc, bf, lc },
            {  df, rf, bc } };


        public FullCube(sbyte[] f)
        {
            edge = new EdgeCube();
            center = new CenterCube();
            corner = new CornerCube();
            for (int i = 0; i < 24; i++)
            {
                center.ct[i] = f[centerFacelet[i]];
            }
            for (int i = 0; i < 24; i++)
            {
                for (sbyte j = 0; j < 24; j++)
                {
                    if (f[edgeFacelet[i, 0]] == edgeFacelet[j, 0] / 16 && f[edgeFacelet[i, 1]] == edgeFacelet[j, 1] / 16)
                    {
                        edge.ep[i] = j;
                    }
                }
            }
            sbyte col1, col2, ori;
            for (sbyte i = 0; i < 8; i++)
            {
                // get the colors of the cubie at corner i, starting with U/D
                for (ori = 0; ori < 3; ori++)
                    if (f[cornerFacelet[i, ori]] == u0 / 16 || f[cornerFacelet[i, ori]] == d0 / 16)
                        break;
                col1 = f[cornerFacelet[i, (ori + 1) % 3]];
                col2 = f[cornerFacelet[i, (ori + 2) % 3]];

                for (sbyte j = 0; j < 8; j++)
                {
                    if (col1 == cornerFacelet[j, 1] / 16 && col2 == cornerFacelet[j, 2] / 16)
                    {
                        // in cornerposition i we have cornercubie j
                        corner.cp[i] = j;
                        corner.co[i] = (sbyte)(ori % 3);
                        break;
                    }
                }
            }
        }

        void ToFacelet(sbyte[] f)
        {
            for (int i = 0; i < 24; i++)
            {
                f[centerFacelet[i]] = center.ct[i];
            }
            for (int i = 0; i < 24; i++)
            {
                f[edgeFacelet[i, 0]] = (sbyte)(edgeFacelet[edge.ep[i], 0] / 16);
                f[edgeFacelet[i, 1]] = (sbyte)(edgeFacelet[edge.ep[i], 1] / 16);
            }
            for (sbyte c = 0; c < 8; c++)
            {
                sbyte j = corner.cp[c];
                sbyte ori = corner.co[c];
                for (sbyte n = 0; n < 3; n++)
                    f[cornerFacelet[c, (n + ori) % 3]] = (sbyte)(cornerFacelet[j, n] / 16);
            }
        }
        public override string ToString()
        {
            GetEdge();
            GetCenter();
            GetCorner();

            sbyte[] f = new sbyte[96];
            StringBuilder sb = new StringBuilder();
            ToFacelet(f);
            for (int i = 0; i < 96; i++)
            {
                sb.Append("URFDLB"[f[i]]);
                if (i % 4 == 3)
                {
                    sb.Append('\n');
                }
                if (i % 16 == 15)
                {
                    sb.Append('\n');
                }
            }
            return sb.ToString();
        }

        public class ValueComparator : IComparer<FullCube>
        {
            public int Compare(FullCube c1, FullCube c2)
            {
                return c2.value - c1.value;
            }
        }

        private readonly EdgeCube edge;
        private readonly CenterCube center;
        private readonly CornerCube corner;

        public int value = 0;
        public bool add1 = false;
        public int Length1 = 0;
        public int Length2 = 0;
        public int Length3 = 0;
        public int CompareTo(FullCube c) => value - c.value;

        public FullCube()
        {
            edge = new EdgeCube();
            center = new CenterCube();
            corner = new CornerCube();
        }

        public FullCube(FullCube c) : this()
        {
            Copy(c);
        }

        public FullCube(Random r)
        {
            edge = new EdgeCube(r);
            center = new CenterCube(r);
            corner = new CornerCube(r);
        }

        public FullCube(int[] moveseq) : this()
        {
            foreach (int m in moveseq)
            {
                DoMove(m);
            }
        }

        public void Copy(FullCube c)
        {
            edge.Copy(c.edge);
            center.Copy(c.center);
            corner.Copy(c.corner);

            value = c.value;
            add1 = c.add1;
            Length1 = c.Length1;
            Length2 = c.Length2;
            Length3 = c.Length3;

            sym = c.sym;

            for (int i = 0; i < 60; i++)
            {
                moveBuffer[i] = c.moveBuffer[i];
            }
            moveLength = c.moveLength;
            edgeAvail = c.edgeAvail;
            centerAvail = c.centerAvail;
            cornerAvail = c.cornerAvail;
        }

        public bool CheckEdge()
        {
            return GetEdge().CheckEdge();
        }

        public string GetMovestring(bool inverse, bool rotation)
        {
            int[] fixedMoves = new int[moveLength - (add1 ? 2 : 0)];
            int idx = 0;
            for (int i = 0; i < Length1; i++)
            {
                fixedMoves[idx++] = moveBuffer[i];
            }
            int sym = this.sym;
            for (int i = Length1 + (add1 ? 2 : 0); i < moveLength; i++)
            {
                if (symmove[sym, moveBuffer[i]] >= dx1)
                {
                    fixedMoves[idx++] = symmove[sym, moveBuffer[i]] - 9;
                    int rot = move2rot[symmove[sym, moveBuffer[i]] - dx1];
                    sym = symmult[sym, rot];
                }
                else
                {
                    fixedMoves[idx++] = symmove[sym, moveBuffer[i]];
                }
            }
            int finishSym = symmult[syminv[sym], GetSolvedSym(GetCenter())];

            StringBuilder sb = new StringBuilder();
            sym = finishSym;
            if (inverse)
            {
                for (int i = idx - 1; i >= 0; i--)
                {
                    int Move = fixedMoves[i];
                    Move = Move / 3 * 3 + (2 - Move % 3);
                    if (symmove[sym, Move] >= dx1)
                    {
                        sb.Append(move2str[symmove[sym, Move] - 9]).Append(' ');
                        int rot = move2rot[symmove[sym, Move] - dx1];
                        sym = symmult[sym, rot];
                    }
                    else
                    {
                        sb.Append(move2str[symmove[sym, Move]]).Append(' ');
                    }
                }
                if (rotation)
                {
                    sb.Append(rot2str[syminv[sym]] + " ");//cube rotation after solution. for wca scramble, it should be omitted.
                }
            }
            else
            {
                for (int i = 0; i < idx; i++)
                {
                    sb.Append(move2str[fixedMoves[i]]).Append(' ');
                }
                if (rotation)
                {
                    sb.Append(rot2str[finishSym]);//cube rotation after solution.
                }
            }
            return sb.ToString();
        }

        private static readonly int[] move2rot = { 35, 1, 34, 2, 4, 6, 22, 5, 19 };

        public string To333Facelet()
        {
            char[] ret = new char[54];
            GetEdge().Fill333Facelet(ret);
            GetCenter().Fill333Facelet(ret);
            GetCorner().Fill333Facelet(ret);
            return new string(ret);
        }

        public sbyte[] moveBuffer = new sbyte[60];
        private int moveLength = 0;
        private int edgeAvail = 0;
        private int centerAvail = 0;
        private int cornerAvail = 0;

        public int sym = 0;

        public void Move(int m)
        {
            moveBuffer[moveLength++] = (sbyte)m;
            return;
        }

        public void DoMove(int m)
        {
            GetEdge().Move(m);
            GetCenter().Move(m);
            GetCorner().Move(m % 18);
        }

        public EdgeCube GetEdge()
        {
            while (edgeAvail < moveLength)
            {
                edge.Move(moveBuffer[edgeAvail++]);
            }
            return edge;
        }

        public CenterCube GetCenter()
        {
            while (centerAvail < moveLength)
            {
                center.Move(moveBuffer[centerAvail++]);
            }
            return center;
        }

        public CornerCube GetCorner()
        {
            while (cornerAvail < moveLength)
            {
                corner.Move(moveBuffer[cornerAvail++] % 18);
            }
            return corner;
        }
    }
}
