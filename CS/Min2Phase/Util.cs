
namespace CS.Min2Phase
{
    public static class Util
    {
        /*  //Edges
            public const sbyte UR = 0;
            public const sbyte UF = 1;
            public const sbyte UL = 2;
            public const sbyte UB = 3;
            public const sbyte DR = 4;
            public const sbyte DF = 5;
            public const sbyte DL = 6;
            public const sbyte DB = 7;
            public const sbyte FR = 8;
            public const sbyte FL = 9;
            public const sbyte BL = 10;
            public const sbyte BR = 11;

            //Corners
            public const sbyte URF = 0;
            public const sbyte UFL = 1;
            public const sbyte ULB = 2;
            public const sbyte UBR = 3;
            public const sbyte DFR = 4;
            public const sbyte DLF = 5;
            public const sbyte DBL = 6;
            public const sbyte DRB = 7;
        */
        //Moves
        public const sbyte Ux1 = 0;
        public const sbyte Ux2 = 1;
        public const sbyte Ux3 = 2;
        public const sbyte Rx1 = 3;
        public const sbyte Rx2 = 4;
        public const sbyte Rx3 = 5;
        public const sbyte Fx1 = 6;
        public const sbyte Fx2 = 7;
        public const sbyte Fx3 = 8;
        public const sbyte Dx1 = 9;
        public const sbyte Dx2 = 10;
        public const sbyte Dx3 = 11;
        public const sbyte Lx1 = 12;
        public const sbyte Lx2 = 13;
        public const sbyte Lx3 = 14;
        public const sbyte Bx1 = 15;
        public const sbyte Bx2 = 16;
        public const sbyte Bx3 = 17;

        //Facelets
        public const sbyte U1 = 0;
        public const sbyte U2 = 1;
        public const sbyte U3 = 2;
        public const sbyte U4 = 3;
        public const sbyte U5 = 4;
        public const sbyte U6 = 5;
        public const sbyte U7 = 6;
        public const sbyte U8 = 7;
        public const sbyte U9 = 8;
        public const sbyte R1 = 9;
        public const sbyte R2 = 10;
        public const sbyte R3 = 11;
        public const sbyte R4 = 12;
        public const sbyte R5 = 13;
        public const sbyte R6 = 14;
        public const sbyte R7 = 15;
        public const sbyte R8 = 16;
        public const sbyte R9 = 17;
        public const sbyte F1 = 18;
        public const sbyte F2 = 19;
        public const sbyte F3 = 20;
        public const sbyte F4 = 21;
        public const sbyte F5 = 22;
        public const sbyte F6 = 23;
        public const sbyte F7 = 24;
        public const sbyte F8 = 25;
        public const sbyte F9 = 26;
        public const sbyte D1 = 27;
        public const sbyte D2 = 28;
        public const sbyte D3 = 29;
        public const sbyte D4 = 30;
        public const sbyte D5 = 31;
        public const sbyte D6 = 32;
        public const sbyte D7 = 33;
        public const sbyte D8 = 34;
        public const sbyte D9 = 35;
        public const sbyte L1 = 36;
        public const sbyte L2 = 37;
        public const sbyte L3 = 38;
        public const sbyte L4 = 39;
        public const sbyte L5 = 40;
        public const sbyte L6 = 41;
        public const sbyte L7 = 42;
        public const sbyte L8 = 43;
        public const sbyte L9 = 44;
        public const sbyte B1 = 45;
        public const sbyte B2 = 46;
        public const sbyte B3 = 47;
        public const sbyte B4 = 48;
        public const sbyte B5 = 49;
        public const sbyte B6 = 50;
        public const sbyte B7 = 51;
        public const sbyte B8 = 52;
        public const sbyte B9 = 53;

        //Colors
        public const sbyte U = 0;
        public const sbyte R = 1;
        public const sbyte F = 2;
        public const sbyte D = 3;
        public const sbyte L = 4;
        public const sbyte B = 5;

        public static readonly sbyte[,] cornerFacelet = {
        { U9, R1, F3 }, { U7, F1, L3 }, { U1, L1, B3 }, { U3, B1, R3 },
        { D3, F9, R7 }, { D1, L9, F7 }, { D7, B9, L7 }, { D9, R9, B7 }
    };
        public static readonly sbyte[,] edgeFacelet = {
        { U6, R2 }, { U8, F2 }, { U4, L2 }, { U2, B2 }, { D6, R8 }, { D2, F8 },
        { D4, L8 }, { D8, B8 }, { F6, R4 }, { F4, L6 }, { B6, L4 }, { B4, R6 }
    };

        public static int[,] Cnk = new int[13, 13];
        public static string[] move2str = {
        "U ", "U2", "U'", "R ", "R2", "R'", "F ", "F2", "F'",
        "D ", "D2", "D'", "L ", "L2", "L'", "B ", "B2", "B'"
    };
        public static int[] ud2std = { Ux1, Ux2, Ux3, Rx2, Fx2, Dx1, Dx2, Dx3, Lx2, Bx2, Rx1, Rx3, Fx1, Fx3, Lx1, Lx3, Bx1, Bx3 };
        public static int[] std2ud = new int[18];
        public static int[] ckmv2bit = new int[11];

        public static void ToCubieCube(sbyte[] f, CubieCube ccRet)
        {
            sbyte ori;
            for (int i = 0; i < 8; i++)
                ccRet.ca[i] = 0;// invalidate corners
            for (int i = 0; i < 12; i++)
                ccRet.ea[i] = 0;// and edges
            sbyte col1, col2;
            for (sbyte i = 0; i < 8; i++)
            {
                // get the colors of the cubie at corner i, starting with U/D
                for (ori = 0; ori < 3; ori++)
                    if (f[cornerFacelet[i, ori]] == U || f[cornerFacelet[i, ori]] == D)
                        break;
                col1 = f[cornerFacelet[i, (ori + 1) % 3]];
                col2 = f[cornerFacelet[i, (ori + 2) % 3]];

                for (sbyte j = 0; j < 8; j++)
                {
                    if (col1 == cornerFacelet[j, 1] / 9 && col2 == cornerFacelet[j, 2] / 9)
                    {
                        // in cornerposition i we have cornercubie j
                        ccRet.ca[i] = (sbyte)((sbyte)(ori % 3 << 3) | j);
                        break;
                    }
                }
            }
            for (sbyte i = 0; i < 12; i++)
            {
                for (sbyte j = 0; j < 12; j++)
                {
                    if (f[edgeFacelet[i, 0]] == edgeFacelet[j, 0] / 9
                            && f[edgeFacelet[i, 1]] == edgeFacelet[j, 1] / 9)
                    {
                        ccRet.ea[i] = (sbyte)(j << 1);
                        break;
                    }
                    if (f[edgeFacelet[i, 0]] == edgeFacelet[j, 1] / 9
                            && f[edgeFacelet[i, 1]] == edgeFacelet[j, 0] / 9)
                    {
                        ccRet.ea[i] = (sbyte)(j << 1 | 1);
                        break;
                    }
                }
            }
        }

        public static string ToFaceCube(CubieCube cc)
        {
            char[] f = new char[54];
            char[] ts = { 'U', 'R', 'F', 'D', 'L', 'B' };
            for (int i = 0; i < 54; i++)
            {
                f[i] = ts[i / 9];
            }
            for (sbyte c = 0; c < 8; c++)
            {
                int j = cc.ca[c] & 0x7;// cornercubie with index j is at
                                       // cornerposition with index c
                int ori = cc.ca[c] >> 3;// Orientation of this cubie
                for (sbyte n = 0; n < 3; n++)
                    f[cornerFacelet[c, (n + ori) % 3]] = ts[cornerFacelet[j, n] / 9];
            }
            for (sbyte e = 0; e < 12; e++)
            {
                int j = cc.ea[e] >> 1;// edgecubie with index j is at edgeposition
                                      // with index e
                int ori = cc.ea[e] & 1;// Orientation of this cubie
                for (sbyte n = 0; n < 2; n++)
                    f[edgeFacelet[e, (n + ori) % 2]] = ts[edgeFacelet[j, n] / 9];
            }
            return new string(f);
        }

        public static int GetNParity(int idx, int n)
        {
            int p = 0;
            for (int i = n - 2; i >= 0; i--)
            {
                p ^= idx % (n - i);
                idx /= (n - i);
            }
            return p & 1;
        }

        public static sbyte SetVal(int val0, int val, bool isEdge)
        {
            return (sbyte)(isEdge ? (val << 1 | val0 & 1) : (val | val0 & 0xf8));
        }

        public static int GetVal(int val0, bool isEdge)
        {
            return isEdge ? val0 >> 1 : val0 & 7;
        }

        public static void SetNPerm(sbyte[] arr, int idx, int n, bool isEdge)
        {
            unchecked
            {
                long val = (long)0xFEDCBA9876543210;
                long extract = 0;
                for (int p = 2; p <= n; p++)
                {
                    extract = extract << 4 | (uint)idx % p;
                    idx /= p;
                }
                for (int i = 0; i < n - 1; i++)
                {
                    int v = ((int)extract & 0xf) << 2;
                    extract >>= 4;
                    arr[i] = SetVal(arr[i], (int)(val >> v & 0xf), isEdge);
                    long m = (1L << v) - 1;
                    val = val & m | val >> 4 & ~m;
                }
                arr[n - 1] = SetVal(arr[n - 1], (int)(val & 0xf), isEdge);
            }

        }

        public static int GetNPerm(sbyte[] arr, int n, bool isEdge)
        {
            int idx = 0;
            unchecked
            {
                long val = (long)0xFEDCBA9876543210;
                for (int i = 0; i < n - 1; i++)
                {
                    int v = GetVal(arr[i], isEdge) << 2;
                    idx = (n - i) * idx + (int)(val >> v & 0xf);
                    val -= 0x1111111111111110 << v;
                }
            }
            return idx;
        }

        public static int GetComb(sbyte[] arr, int mask, bool isEdge)
        {
            int end = arr.Length - 1;
            int idxC = 0, r = 4;
            for (int i = end; i >= 0; i--)
            {
                int perm = GetVal(arr[i], isEdge);
                if ((perm & 0xc) == mask)
                {
                    idxC += Cnk[i, r--];
                }
            }
            return idxC;
        }

        public static void SetComb(sbyte[] arr, int idxC, int mask, bool isEdge)
        {
            int end = arr.Length - 1;
            int r = 4, fill = end;
            for (int i = end; i >= 0; i--)
            {
                if (idxC >= Cnk[i, r])
                {
                    idxC -= Cnk[i, r--];
                    arr[i] = SetVal(arr[i], r | mask, isEdge);
                }
                else
                {
                    if ((fill & 0xc) == mask)
                    {
                        fill -= 4;
                    }
                    arr[i] = SetVal(arr[i], fill--, isEdge);
                }
            }
        }

        static Util()
        {
            for (int i = 0; i < 18; i++)
            {
                std2ud[ud2std[i]] = i;
            }
            for (int i = 0; i < 10; i++)
            {
                int ix = ud2std[i] / 3;
                ckmv2bit[i] = 0;
                for (int j = 0; j < 10; j++)
                {
                    int jx = ud2std[j] / 3;
                    ckmv2bit[i] |= ((ix == jx) || ((ix % 3 == jx % 3) && (ix >= jx)) ? 1 : 0) << j;
                }
            }
            ckmv2bit[10] = 0;
            for (int i = 0; i < 13; i++)
            {
                Cnk[i, 0] = Cnk[i, i] = 1;
                for (int j = 1; j < i; j++)
                {
                    Cnk[i, j] = Cnk[i - 1, j - 1] + Cnk[i - 1, j];
                }
            }
        }
    }
}
