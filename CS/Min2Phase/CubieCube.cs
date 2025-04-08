﻿using System;
using System.Text;
namespace CS.Min2Phase;

public class CubieCube
{

    /**
    * 16 symmetries generated by S_F2, S_U4 and S_LR2
    */
    public static CubieCube[] CubeSym = new CubieCube[16];

    /**
    * 18 move cubes
    */
    public static CubieCube[] moveCube = new CubieCube[18];

    public static long[] moveCubeSym = new long[18];
    public static int[] firstMoveSym = new int[48];

    public static int[,] SymMult = new int[16, 16];
    public static int[,] SymMultInv = new int[16, 16];
    public static int[,] SymMove = new int[16, 18];
    public static int[] Sym8Move = new int[8 * 18];
    public static int[,] SymMoveUD = new int[16, 18];

    /**
        * ClassIndexToRepresentantArrays
        */
    public static char[] FlipS2R = new char[CoordCube.N_FLIP_SYM];
    public static char[] TwistS2R = new char[CoordCube.N_TWIST_SYM];
    public static char[] EPermS2R = new char[CoordCube.N_PERM_SYM];
    public static sbyte[] Perm2CombP = new sbyte[CoordCube.N_PERM_SYM];
    public static char[] PermInvEdgeSym = new char[CoordCube.N_PERM_SYM];

    /**
        * Notice that Edge Perm Coordnate and Corner Perm Coordnate are the same symmetry structure.
        * So their ClassIndexToRepresentantArray are the same.
        * And when x is RawEdgePermCoordnate, y*16+k is SymEdgePermCoordnate, y*16+(k^e2c[k]) will
        * be the SymCornerPermCoordnate of the State whose RawCornerPermCoordnate is x.
        */
    // public static sbyte[] e2c = {0, 0, 0, 0, 1, 3, 1, 3, 1, 3, 1, 3, 0, 0, 0, 0};
    public const int SYM_E2C_MAGIC = 0x00DDDD00;
    public static int ESym2CSym(int idx)
    {
        return idx ^ (SYM_E2C_MAGIC >> ((idx & 0xf) << 1) & 3);
    }

    /**
        * Raw-Coordnate to Sym-Coordnate, only for speeding up initializaion.
        */
    public static sbyte[] FlipR2S = new sbyte[CoordCube.N_FLIP_HALF + CoordCube.N_FLIP];
    public static sbyte[] TwistR2S = new sbyte[CoordCube.N_TWIST_HALF + CoordCube.N_TWIST];
    public static sbyte[] EPermR2S = new sbyte[CoordCube.N_PERM_HALF];
    public static char[] FlipS2RF = Search.USE_TWIST_FLIP_PRUN ? new char[CoordCube.N_FLIP_SYM * 8] : null;

    /**
        *
        */
    public static char[] SymStateTwist;// = new char[CoordCube.N_TWIST_SYM];
    public static char[] SymStateFlip;// = new char[CoordCube.N_FLIP_SYM];
    public static char[] SymStatePerm;// = new char[CoordCube.N_PERM_SYM];

    public static CubieCube urf1 = new(2531, 1373, 67026819, 1367);
    public static CubieCube urf2 = new(2089, 1906, 322752913, 2040);
    public static sbyte[,] urfMove = new sbyte[,] {
    {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17},
    {6, 7, 8, 0, 1, 2, 3, 4, 5, 15, 16, 17, 9, 10, 11, 12, 13, 14},
    {3, 4, 5, 6, 7, 8, 0, 1, 2, 12, 13, 14, 15, 16, 17, 9, 10, 11},
    {2, 1, 0, 5, 4, 3, 8, 7, 6, 11, 10, 9, 14, 13, 12, 17, 16, 15},
    {8, 7, 6, 2, 1, 0, 5, 4, 3, 17, 16, 15, 11, 10, 9, 14, 13, 12},
    {5, 4, 3, 8, 7, 6, 2, 1, 0, 14, 13, 12, 17, 16, 15, 11, 10, 9}
};

    public sbyte[] ca = [0, 1, 2, 3, 4, 5, 6, 7];
    public sbyte[] ea = [0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22];
    public CubieCube temps = null;

    public CubieCube()
    {
    }

    public CubieCube(int cperm, int twist, int eperm, int flip)
    {
        this.SetCPerm(cperm);
        this.SetTwist(twist);
        Util.SetNPerm(ea, eperm, 12, true);
        this.SetFlip(flip);
    }

    public CubieCube(CubieCube c)
    {
        Copy(c);
    }

    public void Copy(CubieCube c)
    {
        for (int i = 0; i < 8; i++)
        {
            this.ca[i] = c.ca[i];
        }
        for (int i = 0; i < 12; i++)
        {
            this.ea[i] = c.ea[i];
        }
    }

    public void InvCubieCube()
    {
        temps = temps ?? new CubieCube();
        for (sbyte edge = 0; edge < 12; edge++)
        {
            temps.ea[ea[edge] >> 1] = (sbyte)(edge << 1 | ea[edge] & 1);
        }
        for (sbyte corn = 0; corn < 8; corn++)
        {
            temps.ca[ca[corn] & 0x7] = (sbyte)(corn | (sbyte)(0x20 >> (ca[corn] >> 3) & 0x18));
        }
        Copy(temps);
    }

    /**
        * prod = a * b, Corner Only.
        */
    public static void CornMult(CubieCube a, CubieCube b, CubieCube prod)
    {
        for (int corn = 0; corn < 8; corn++)
        {
            int oriA = a.ca[b.ca[corn] & 7] >> 3;
            int oriB = b.ca[corn] >> 3;
            int ori = oriA + ((oriA < 3) ? oriB : 6 - oriB);
            ori = ori % 3 + ((oriA < 3) == (oriB < 3) ? 0 : 3);
            prod.ca[corn] = (sbyte)(a.ca[b.ca[corn] & 7] & 7 | ori << 3);
        }
    }

    /**
        * prod = a * b, Edge Only.
        */
    public static void EdgeMult(CubieCube a, CubieCube b, CubieCube prod)
    {
        for (int ed = 0; ed < 12; ed++)
        {
            prod.ea[ed] = (sbyte)(a.ea[b.ea[ed] >> 1] ^ (b.ea[ed] & 1));
        }
    }

    /**
        * b = S_idx^-1 * a * S_idx, Corner Only.
        */
    public static void CornConjugate(CubieCube a, int idx, CubieCube b)
    {
        CubieCube sinv = CubeSym[SymMultInv[0, idx]];
        CubieCube s = CubeSym[idx];
        for (int corn = 0; corn < 8; corn++)
        {
            int oriA = sinv.ca[a.ca[s.ca[corn] & 7] & 7] >> 3;
            int oriB = a.ca[s.ca[corn] & 7] >> 3;
            int ori = (oriA < 3) ? oriB : (3 - oriB) % 3;
            b.ca[corn] = (sbyte)(sinv.ca[a.ca[s.ca[corn] & 7] & 7] & 7 | ori << 3);
        }
    }

    /**
        * b = S_idx^-1 * a * S_idx, Edge Only.
        */
    public static void EdgeConjugate(CubieCube a, int idx, CubieCube b)
    {
        CubieCube sinv = CubeSym[SymMultInv[0, idx]];
        CubieCube s = CubeSym[idx];
        for (int ed = 0; ed < 12; ed++)
        {
            b.ea[ed] = (sbyte)(sinv.ea[a.ea[s.ea[ed] >> 1] >> 1] ^ (a.ea[s.ea[ed] >> 1] & 1) ^ (s.ea[ed] & 1));
        }
    }

    public static int GetPermSymInv(int idx, int sym, bool isCorner)
    {
        int idxi = PermInvEdgeSym[idx];
        if (isCorner)
        {
            idxi = ESym2CSym(idxi);
        }
        return idxi & 0xfff0 | SymMult[idxi & 0xf, sym];
    }

    public static int GetSkipMoves(long ssym)
    {
        int ret = 0;
        for (int i = 1; (ssym >>= 1) != 0; i++)
        {
            if ((ssym & 1) == 1)
            {
                ret |= firstMoveSym[i];
            }
        }
        return ret;
    }

    /**
        * this = S_urf^-1 * this * S_urf.
        */
    public void URFConjugate()
    {
        temps = temps ?? new CubieCube();
        CornMult(urf2, this, temps);
        CornMult(temps, urf1, this);
        EdgeMult(urf2, this, temps);
        EdgeMult(temps, urf1, this);
    }

    // ********************************************* Get and set coordinates *********************************************
    // XSym : Symmetry Coordnate of X. MUST be called after initialization of ClassIndexToRepresentantArrays.

    // ++++++++++++++++++++ Phase 1 Coordnates ++++++++++++++++++++
    // Flip : Orientation of 12 Edges. Raw[0, 2048) Sym[0, 336 * 8)
    // Twist : Orientation of 8 Corners. Raw[0, 2187) Sym[0, 324 * 8)
    // UDSlice : Positions of the 4 UDSlice edges, the order is ignored. [0, 495)

    public int GetFlip()
    {
        int idx = 0;
        for (int i = 0; i < 11; i++)
        {
            idx = idx << 1 | ea[i] & 1;
        }
        return idx;
    }

    public void SetFlip(int idx)
    {
        int parity = 0, val;
        for (int i = 10; i >= 0; i--, idx >>= 1)
        {
            parity ^= (val = idx & 1);
            ea[i] = (sbyte)(ea[i] & 0xfe | val);
        }
        ea[11] = (sbyte)(ea[11] & 0xfe | parity);
    }

    public int GetFlipSym()
    {
        return FlipRaw2Sym(GetFlip());
    }

    public static int FlipRaw2Sym(int raw)
    {
        return 0xfff & FlipR2S[raw + CoordCube.N_FLIP_HALF] << 4 | CoordCube.GetPruning(FlipR2S, raw);
    }

    public int GetTwist()
    {
        int idx = 0;
        for (int i = 0; i < 7; i++)
        {
            idx += (idx << 1) + (ca[i] >> 3);
        }
        return idx;
    }

    public void SetTwist(int idx)
    {
        int twst = 15, val;
        for (int i = 6; i >= 0; i--, idx /= 3)
        {
            twst -= (val = idx % 3);
            ca[i] = (sbyte)(ca[i] & 0x7 | val << 3);
        }
        ca[7] = (sbyte)(ca[7] & 0x7 | (twst % 3) << 3);
    }

    public int GetTwistSym()
    {
        int raw = GetTwist();
        return 0xfff & TwistR2S[raw + CoordCube.N_TWIST_HALF] << 4 | CoordCube.GetPruning(TwistR2S, raw);
    }

    public int GetUDSlice()
    {
        return 494 - Util.GetComb(ea, 8, true);
    }

    public void SetUDSlice(int idx)
    {
        Util.SetComb(ea, 494 - idx, 8, true);
    }

    // ++++++++++++++++++++ Phase 2 Coordnates ++++++++++++++++++++
    // EPerm : Permutations of 8 UD Edges. Raw[0, 40320) Sym[0, 2187 * 16)
    // Cperm : Permutations of 8 Corners. Raw[0, 40320) Sym[0, 2187 * 16)
    // MPerm : Permutations of 4 UDSlice Edges. [0, 24)

    public int GetCPerm()
    {
        return Util.GetNPerm(ca, 8, false);
    }

    public void SetCPerm(int idx)
    {
        Util.SetNPerm(ca, idx, 8, false);
    }

    public int GetCPermSym()
    {
        int k = ESym2CSym(CoordCube.GetPruning(EPermR2S, GetCPerm())) & 0xf;
        temps = temps ?? new CubieCube();
        CornConjugate(this, SymMultInv[0, k], temps);
        int idx = Array.BinarySearch(EPermS2R, (char)temps.GetCPerm());
        //assert idx >= 0;
        return idx << 4 | k;
    }

    public int GetEPerm()
    {
        return Util.GetNPerm(ea, 8, true);
    }

    public void SetEPerm(int idx)
    {
        Util.SetNPerm(ea, idx, 8, true);
    }

    public int GetEPermSym()
    {
        int raw = GetEPerm();
        int k = CoordCube.GetPruning(EPermR2S, raw);
        temps = temps ?? new CubieCube();
        EdgeConjugate(this, SymMultInv[0, k], temps);
        int idx = Array.BinarySearch(EPermS2R, (char)temps.GetEPerm());
        //assert idx >= 0;
        return idx << 4 | k;
    }

    public int GetMPerm()
    {
        return Util.GetNPerm(ea, 12, true) % 24;
    }

    public void SetMPerm(int idx)
    {
        Util.SetNPerm(ea, idx, 12, true);
    }

    public int GetCComb()
    {
        return Util.GetComb(ca, 0, false);
    }

    public void SetCComb(int idx)
    {
        Util.SetComb(ca, idx, 0, false);
    }

    /**
        * Check a cubiecube for solvability. Return the error code.
        * 0: Cube is solvable
        * -2: Not all 12 edges exist exactly once
        * -3: Flip error: One edge has to be flipped
        * -4: Not all corners exist exactly once
        * -5: Twist error: One corner has to be twisted
        * -6: Parity error: Two corners or two edges have to be exchanged
        */
    public int Verify()
    {
        int sum = 0;
        int edgeMask = 0;
        for (int e = 0; e < 12; e++)
        {
            edgeMask |= 1 << (ea[e] >> 1);
            sum ^= ea[e] & 1;
        }
        if (edgeMask != 0xfff)
        {
            return -2;// missing edges
        }
        if (sum != 0)
        {
            return -3;
        }
        int cornMask = 0;
        sum = 0;
        for (int c = 0; c < 8; c++)
        {
            cornMask |= 1 << (ca[c] & 7);
            sum += ca[c] >> 3;
        }
        if (cornMask != 0xff)
        {
            return -4;// missing corners
        }
        if (sum % 3 != 0)
        {
            return -5;// twisted corner
        }
        if ((Util.GetNParity(Util.GetNPerm(ea, 12, true), 12) ^ Util.GetNParity(GetCPerm(), 8)) != 0)
        {
            return -6;// parity error
        }
        return 0;// cube ok
    }

    public long SelfSymmetry()
    {
        CubieCube c = new(this);
        CubieCube d = new();
        long sym = 0L;
        for (int i = 0; i < 96; i++)
        {
            CornConjugate(c, SymMultInv[0, i % 16], d);
            if (System.Linq.Enumerable.SequenceEqual(d.ca, ca))
            {
                EdgeConjugate(c, SymMultInv[0, i % 16], d);
                if (System.Linq.Enumerable.SequenceEqual(d.ea, ea))
                {
                    sym |= 1L << Math.Min(i, 48);
                }
            }
            if (i % 16 == 15)
            {
                c.URFConjugate();
            }
            if (i % 48 == 47)
            {
                c.InvCubieCube();
            }
        }
        return sym;
    }

    // ********************************************* Initialization functions *********************************************

    public static void InitMove()
    {
        moveCube[0] = new CubieCube(15120, 0, 119750400, 0);
        moveCube[3] = new CubieCube(21021, 1494, 323403417, 0);
        moveCube[6] = new CubieCube(8064, 1236, 29441808, 550);
        moveCube[9] = new CubieCube(9, 0, 5880, 0);
        moveCube[12] = new CubieCube(1230, 412, 2949660, 0);
        moveCube[15] = new CubieCube(224, 137, 328552, 137);
        for (int a = 0; a < 18; a += 3)
        {
            for (int p = 0; p < 2; p++)
            {
                moveCube[a + p + 1] = new CubieCube();
                EdgeMult(moveCube[a + p], moveCube[a], moveCube[a + p + 1]);
                CornMult(moveCube[a + p], moveCube[a], moveCube[a + p + 1]);
            }
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (int i = 0; i < 8; i++)
        {
            sb.Append("|").Append(ca[i] & 7).Append(" ").Append(ca[i] >> 3);
        }
        sb.Append("\n");
        for (int i = 0; i < 12; i++)
        {
            sb.Append("|").Append(ea[i] >> 1).Append(" ").Append(ea[i] & 1);
        }
        return sb.ToString();
    }

    public static void InitSym()
    {
        CubieCube c = new();
        CubieCube d = new();
        CubieCube t;

        CubieCube f2 = new(28783, 0, 259268407, 0);
        CubieCube u4 = new(15138, 0, 119765538, 7);
        CubieCube lr2 = new(5167, 0, 83473207, 0);
        for (int i = 0; i < 8; i++)
        {
            lr2.ca[i] |= 3 << 3;
        }

        for (int i = 0; i < 16; i++)
        {
            CubeSym[i] = new CubieCube(c);
            CornMult(c, u4, d);
            EdgeMult(c, u4, d);
            t = d; d = c; c = t;
            if (i % 4 == 3)
            {
                CornMult(c, lr2, d);
                EdgeMult(c, lr2, d);
                t = d; d = c; c = t;
            }
            if (i % 8 == 7)
            {
                CornMult(c, f2, d);
                EdgeMult(c, f2, d);
                t = d; d = c; c = t;
            }
        }
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                CornMult(CubeSym[i], CubeSym[j], c);
                for (int k = 0; k < 16; k++)
                {
                    if (System.Linq.Enumerable.SequenceEqual(CubeSym[k].ca, c.ca))
                    {
                        SymMult[i, j] = k; // SymMult[i,j] = (k ^ i ^ j ^ (0x14ab4 >> j & i << 1 & 2)));
                        SymMultInv[k, j] = i; // i * j = k => k * j^-1 = i
                        break;
                    }
                }
            }
        }
        for (int j = 0; j < 18; j++)
        {
            for (int s = 0; s < 16; s++)
            {
                CornConjugate(moveCube[j], SymMultInv[0, s], c);
                for (int m = 0; m < 18; m++)
                {
                    if (System.Linq.Enumerable.SequenceEqual(moveCube[m].ca, c.ca))
                    {
                        SymMove[s, j] = m;
                        SymMoveUD[s, Util.std2ud[j]] = Util.std2ud[m];
                        break;
                    }
                }
                if (s % 2 == 0)
                {
                    Sym8Move[j << 3 | s >> 1] = SymMove[s, j];
                }
            }
        }

        for (int i = 0; i < 18; i++)
        {
            moveCubeSym[i] = moveCube[i].SelfSymmetry();
            int j = i;
            for (int s = 0; s < 48; s++)
            {
                if (SymMove[s % 16, j] < i)
                {
                    firstMoveSym[s] |= 1 << i;
                }
                if (s % 16 == 15)
                {
                    j = urfMove[2, j];
                }
            }
        }
    }

    public static int InitSym2Raw(int N_RAW, char[] Sym2Raw, sbyte[] Raw2Sym, char[] SymState, int coord)
    {
        int N_RAW_HALF = (N_RAW + 1) / 2;
        CubieCube c = new();
        CubieCube d = new();
        int count = 0, idx = 0;
        int sym_inc = coord >= 2 ? 1 : 2;
        bool isEdge = coord != 1;

        for (int i = 0; i < N_RAW; i++)
        {
            if (CoordCube.GetPruning(Raw2Sym, i) != 0)
            {
                continue;
            }
            switch (coord)
            {
                case 0: c.SetFlip(i); break;
                case 1: c.SetTwist(i); break;
                case 2: c.SetEPerm(i); break;
            }
            for (int s = 0; s < 16; s += sym_inc)
            {
                if (isEdge)
                {
                    EdgeConjugate(c, s, d);
                }
                else
                {
                    CornConjugate(c, s, d);
                }
                switch (coord)
                {
                    case 0:
                        idx = d.GetFlip();
                        break;
                    case 1:
                        idx = d.GetTwist();
                        break;
                    case 2:
                        idx = d.GetEPerm();
                        break;
                }
                if (coord == 0 && Search.USE_TWIST_FLIP_PRUN)
                {
                    FlipS2RF[count << 3 | s >> 1] = (char)idx;
                }
                if (idx == i)
                {
                    SymState[count] |= (char)(1 << (s / sym_inc));
                }
                int symIdx = (count << 4 | s) / sym_inc;
                if (CoordCube.GetPruning(Raw2Sym, idx) == 0)
                {
                    CoordCube.SetPruning(Raw2Sym, idx, symIdx & 0xf);
                    if (coord != 2)
                    {
                        Raw2Sym[idx + N_RAW_HALF] = (sbyte)(symIdx >> 4);
                    }
                }
            }
            Sym2Raw[count++] = (char)i;
        }
        return count;
    }

    public static void InitFlipSym2Raw()
    {
        InitSym2Raw(CoordCube.N_FLIP, FlipS2R, FlipR2S,
                    SymStateFlip = new char[CoordCube.N_FLIP_SYM], 0);
    }

    public static void InitTwistSym2Raw()
    {
        InitSym2Raw(CoordCube.N_TWIST, TwistS2R, TwistR2S,
                    SymStateTwist = new char[CoordCube.N_TWIST_SYM], 1);
    }

    public static void InitPermSym2Raw()
    {
        InitSym2Raw(CoordCube.N_PERM, EPermS2R, EPermR2S,
                    SymStatePerm = new char[CoordCube.N_PERM_SYM], 2);
        CubieCube cc = new();
        for (int i = 0; i < CoordCube.N_PERM_SYM; i++)
        {
            cc.SetEPerm(EPermS2R[i]);
            Perm2CombP[i] = (sbyte)(Util.GetComb(cc.ea, 0, true) + (Search.USE_COMBP_PRUN ? Util.GetNParity(EPermS2R[i], 8) * 70 : 0));
            cc.InvCubieCube();
            PermInvEdgeSym[i] = (char)cc.GetEPermSym();
        }
    }
}
