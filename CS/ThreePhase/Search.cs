using System;
using static CS.ThreePhase.Center1;
using static CS.ThreePhase.Center2;
using static CS.ThreePhase.Util;
using static CS.ThreePhase.Moves;

namespace CS.ThreePhase;

/**
Copyright (C) 2014  Shuang Chen

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
public class Search
{
    public const int PHASE1_SOLUTIONS = 10000;
    public const int PHASE2_ATTEMPTS = 500;
    public const int PHASE2_SOLUTIONS = 100;
    public const int PHASE3_ATTEMPTS = 100;

    public PriorityQueue<FullCube> p1sols = new(PHASE2_ATTEMPTS, new FullCube.ValueComparator());

    public static int[] count = new int[1];

    public int[] move1 = new int[15];
    public int[] move2 = new int[20];
    public int[] move3 = new int[20];
    public int Length1 = 0;
    public int Length2 = 0;
    public int maxLength2;
    public bool add1 = false;
    public FullCube c;
    public FullCube c1 = new();
    public FullCube c2 = new();
    public Center2 ct2 = new();
    public Center3 ct3 = new();
    public Edge3 e12 = new();
    public Edge3[] tempe = new Edge3[20];

    public int valid1 = 0;
    public string solution = "";

    public int p1SolsCnt = 0;
    public FullCube[] arr2 = new FullCube[PHASE2_SOLUTIONS];
    public int arr2idx = 0;

    public bool inverse_solution = true;
    public bool with_rotation = true;

    public Search()
    {
        for (int i = 0; i < 20; i++)
        {
            tempe[i] = new Edge3();
        }
    }
    //[MethodImpl(MethodImplOptions.Synchronized)]
    static Search()
    {
        ////System.out.println("Initialize Center1 Solver...");

        InitSym();
        raw2sym = new int[735471];
        InitSym2Raw();
        CreateMoveTable();
        raw2sym = null;
        CreatePrun();

        ////System.out.println("Initialize Center2 Solver...");

        Init();

        ////System.out.println("Initialize Center3 Solver...");

        Center3.Init();

        ////System.out.println("Initialize Edge3 Solver...");

        Edge3.InitMvrot();
        Edge3.InitRaw2Sym();
        Edge3.CreatePrun();

        ////System.out.println("OK");
    }

    public string RandomMove(Random r)
    {
        int[] moveseq = new int[40];
        int lm = 36;
        for (int i = 0; i < moveseq.Length;)
        {
            int m = r.Next(27);
            if (!Moves.ckmv[lm, m])
            {
                moveseq[i++] = m;
                lm = m;
            }
        }
        ////System.out.println(tostr(moveseq));
        return Solve(moveseq);
    }

    public string RandomState(Random r)
    {
        c = new FullCube(r);
        DoSearch();
        return solution;
    }

    public string Solution(string facelet)
    {
        sbyte[] f = new sbyte[96];
        for (int i = 0; i < 96; i++)
        {
            f[i] = (sbyte)"URFDLB".IndexOf(facelet[i]);
        }
        c = new FullCube(f);
        DoSearch();
        return solution;
    }

    public string Solve(string scramble)
    {
        int[] moveseq = Tomove(scramble);
        return Solve(moveseq);
    }

    public string Solve(int[] moveseq)
    {
        c = new FullCube(moveseq);
        DoSearch();
        return solution;
    }

    public int totlen = 0;

    public void DoSearch()
    {
        solution = "";
        int ud = new Center1(c.GetCenter(), 0).Getsym();
        int fb = new Center1(c.GetCenter(), 1).Getsym();
        int rl = new Center1(c.GetCenter(), 2).Getsym();
        int udprun = csprun[ud >> 6];
        int fbprun = csprun[fb >> 6];
        int rlprun = csprun[rl >> 6];

        p1SolsCnt = 0;
        arr2idx = 0;
        p1sols.Clear();

        for (Length1 = Math.Min(Math.Min(udprun, fbprun), rlprun); Length1 < 100; Length1++)
        {
            if (rlprun <= Length1 && Search1(rl >> 6, rl & 0x3f, Length1, -1, 0) //>
                    || udprun <= Length1 && Search1(ud >> 6, ud & 0x3f, Length1, -1, 0) //>
                    || fbprun <= Length1 && Search1(fb >> 6, fb & 0x3f, Length1, -1, 0)) //>
                break;
        }

        FullCube[] p1SolsArr = p1sols.ToArray();
        Array.Sort(p1SolsArr);//, 0, p1SolsArr.Length

        int MAX_Length2 = 9;
        int Length12;
        do
        {
            bool out1 = false;
            for (Length12 = p1SolsArr[0].value; Length12 < 100; Length12++)
            {
                for (int i = 0; i < p1SolsArr.Length; i++)
                {
                    if (p1SolsArr[i].value > Length12)
                    {
                        break;
                    }
                    if (Length12 - p1SolsArr[i].Length1 > MAX_Length2)
                    {
                        continue;
                    }
                    c1.Copy(p1SolsArr[i]);
                    ct2.Set(c1.GetCenter(), c1.GetEdge().GetParity());
                    int s2ct = ct2.Getct();
                    int s2rl = ct2.Getrl();
                    Length1 = p1SolsArr[i].Length1;
                    Length2 = Length12 - p1SolsArr[i].Length1;

                    if (Search2(s2ct, s2rl, Length2, 28, 0))
                    {
                        out1 = true; break;
                    }
                }
                if (out1) break;
            }
            MAX_Length2++;
        } while (Length12 == 100);
        Array.Sort(arr2);//, 0, arr2idx
        int Length123, index = 0;
        int solcnt = 0;

        int MAX_Length3 = 13;
        do
        {
            bool out2 = false;
            for (Length123 = arr2[0].value; Length123 < 100; Length123++)
            {
                for (int i = 0; i < Math.Min(arr2idx, PHASE3_ATTEMPTS); i++)
                {
                    if (arr2[i].value > Length123)
                    {
                        break;
                    }
                    if (Length123 - arr2[i].Length1 - arr2[i].Length2 > MAX_Length3)
                    {
                        continue;
                    }
                    int eparity = e12.Set(arr2[i].GetEdge());
                    ct3.Set(arr2[i].GetCenter(), eparity ^ arr2[i].GetCorner().GetParity());
                    int ct = ct3.Getct();
                    int edge = e12.Get(10);
                    int prun = Edge3.Getprun(e12.Getsym());
                    int lm = 20;

                    if (prun <= Length123 - arr2[i].Length1 - arr2[i].Length2
                            && Search3(edge, ct, prun, Length123 - arr2[i].Length1 - arr2[i].Length2, lm, 0))
                    {
                        solcnt++;
                        //					if (solcnt == 5) {
                        index = i;
                        out2 = true;
                        break;
                        //					}
                    }
                }
                if (out2) break;
            }
            MAX_Length3++;
        } while (Length123 == 100);

        FullCube solcube = new(arr2[index]);
        Length1 = solcube.Length1;
        Length2 = solcube.Length2;
        int Length = Length123 - Length1 - Length2;

        for (int i = 0; i < Length; i++)
        {
            solcube.Move(move3std[move3[i]]);
        }

        string facelet = solcube.To333Facelet();
        string sol = Global.Search3.Solution(facelet, 21, 1000000, 500, 0);
        int len333 = Global.Search3.Length;
        if (sol.StartsWith("Error"))
        {
            //System.out.println(sol);
            //System.out.println(solcube);
            //System.out.println(facelet);
            throw new Exception();
        }
        int[] sol333 = Tomove(sol);
        for (int i = 0; i < sol333.Length; i++)
        {
            solcube.Move(sol333[i]);
        }

        solution = solcube.GetMovestring(inverse_solution, with_rotation);

        totlen = Length1 + Length2 + Length + len333;
    }

    public void Calc(FullCube s)
    {
        c = s;
        DoSearch();
    }

    public bool Search1(int ct, int sym, int maxl, int lm, int depth)
    {
        if (ct == 0 && maxl < 5)
        {
            return maxl == 0 && Init2(sym, lm);
        }
        for (int axis = 0; axis < 27; axis += 3)
        {
            if (axis == lm || axis == lm - 9 || axis == lm - 18)
            {
                continue;
            }
            for (int power = 0; power < 3; power++)
            {
                int m = axis + power;
                int ctx = Center1.ckmv[ct, symmove[sym, m]];
                int prun = csprun[ctx >> 6];//>
                if (prun >= maxl)
                {
                    if (prun > maxl)
                    {
                        break;
                    }
                    continue;
                }
                int symx = symmult[sym, ctx & 0x3f];
                ctx >>= 6;//>
                move1[depth] = m;
                if (Search1(ctx, symx, maxl - 1, axis, depth + 1))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool Init2(int sym, int _)
    {
        c1.Copy(c);
        for (int i = 0; i < Length1; i++)
        {
            c1.Move(move1[i]);
        }

        switch (finish[sym])
        {
            case 0:
                c1.Move(fx1);
                c1.Move(bx3);
                move1[Length1] = fx1;
                move1[Length1 + 1] = bx3;
                add1 = true;
                sym = 19;
                break;
            case 12869:
                c1.Move(ux1);
                c1.Move(dx3);
                move1[Length1] = ux1;
                move1[Length1 + 1] = dx3;
                add1 = true;
                sym = 34;
                break;
            case 735470:
                add1 = false;
                sym = 0;
                break;
        }
        ct2.Set(c1.GetCenter(), c1.GetEdge().GetParity());
        int s2ct = ct2.Getct();
        int s2rl = ct2.Getrl();
        int ctp = ctprun[s2ct * 70 + s2rl];

        c1.value = ctp + Length1;
        c1.Length1 = Length1;
        c1.add1 = add1;
        c1.sym = sym;
        p1SolsCnt++;

        FullCube next;
        if (p1sols.Count < PHASE2_ATTEMPTS)
        {
            next = new FullCube(c1);
        }
        else
        {
            next = p1sols.Pop();
            if (next.value > c1.value)
            {
                next.Copy(c1);
            }
        }
        p1sols.Push(next);

        return p1SolsCnt == PHASE1_SOLUTIONS;
    }

    public bool Search2(int ct, int rl, int maxl, int lm, int depth)
    {
        if (ct == 0 && ctprun[rl] == 0 && maxl == 0)
        {
            return maxl == 0 && Init3();
        }
        for (int m = 0; m < 23; m++)
        {
            if (ckmv2[lm, m])
            {
                m = skipAxis2[m];
                continue;
            }
            int ctx = ctmv[ct, m];
            int rlx = rlmv[rl, m];

            int prun = ctprun[ctx * 70 + rlx];
            if (prun >= maxl)
            {
                if (prun > maxl)
                {
                    m = skipAxis2[m];
                }
                continue;
            }

            move2[depth] = move2std[m];
            if (Search2(ctx, rlx, maxl - 1, m, depth + 1))
            {
                return true;
            }
        }
        return false;
    }

    public bool Init3()
    {
        c2.Copy(c1);
        for (int i = 0; i < Length2; i++)
        {
            c2.Move(move2[i]);
        }
        if (!c2.CheckEdge())
        {
            return false;
        }
        int eparity = e12.Set(c2.GetEdge());
        ct3.Set(c2.GetCenter(), eparity ^ c2.GetCorner().GetParity());
        int ct = ct3.Getct();
        _ = e12.Get(10);
        int prun = Edge3.Getprun(e12.Getsym());

        if (arr2[arr2idx] == null)
        {
            arr2[arr2idx] = new FullCube(c2);
        }
        else
        {
            arr2[arr2idx].Copy(c2);
        }
        arr2[arr2idx].value = Length1 + Length2 + Math.Max(prun, Center3.prun[ct]);
        arr2[arr2idx].Length2 = Length2;
        arr2idx++;

        return arr2idx == arr2.Length;
    }

    public bool Search3(int edge, int ct, int prun, int maxl, int lm, int depth)
    {
        if (maxl == 0)
        {
            return edge == 0 && ct == 0;
        }
        tempe[depth].Set(edge);
        for (int m = 0; m < 17; m++)
        {
            if (ckmv3[lm, m])
            {
                m = skipAxis3[m];
                continue;
            }
            int ctx = Center3.ctmove[ct, m];
            int prun1 = Center3.prun[ctx];
            if (prun1 >= maxl)
            {
                if (prun1 > maxl && m < 14)
                {
                    m = skipAxis3[m];
                }
                continue;
            }
            int edgex = Edge3.Getmvrot(tempe[depth].edge, m << 3, 10);

            int cord1x = edgex / Edge3.N_RAW;
            int symcord1x = Edge3.raw2sym[cord1x];
            int symx = symcord1x & 0x7;
            symcord1x >>= 3;
            int cord2x = Edge3.Getmvrot(tempe[depth].edge, m << 3 | symx, 10) % Edge3.N_RAW;

            int prunx = Edge3.Getprun(symcord1x * Edge3.N_RAW + cord2x, prun);
            if (prunx >= maxl)
            {
                if (prunx > maxl && m < 14)
                {
                    m = skipAxis3[m];
                }
                continue;
            }

            if (Search3(edgex, ctx, prunx, maxl - 1, m, depth + 1))
            {
                move3[depth] = m;
                return true;
            }
        }
        return false;
    }
}
