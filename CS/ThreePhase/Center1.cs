using System;
using static CS.ThreePhase.Util;
using static CS.ThreePhase.Moves;

namespace CS.ThreePhase;

/*
            0	1
            3	2

20	21		8	9		16	17		12	13
23	22		11	10		19	18		15	14

            4	5
            7	6
*/
public sealed class Center1
{
    public static int[,] ckmv = new int[15582, 36];
    public static int[] sym2raw = new int[15582];
    public static sbyte[] csprun = new sbyte[15582];

    public static int[,] symmult = new int[48, 48];
    public static int[,] symmove = new int[48, 36];
    public static int[] syminv = new int[48];
    public static int[] finish = new int[48];

    public static int[] raw2sym;

    public static void InitSym2Raw()
    {
        Center1 c = new();
        int[] occ = new int[735471 / 32 + 1];
        int count = 0;
        for (int i = 0; i < 735471; i++)
        {
            if ((occ[i >> 5] & (1 << (i & 0x1f))) == 0) //>
            {
                c.Set(i);
                for (int j = 0; j < 48; j++)
                {
                    int idx = c.Get();
                    occ[idx >> 5] |= (1 << (idx & 0x1f)); //>
                    if (raw2sym != null)
                    {
                        raw2sym[idx] = count << 6 | syminv[j];
                    }
                    c.Rot(0);
                    if (j % 2 == 1) c.Rot(1);
                    if (j % 8 == 7) c.Rot(2);
                    if (j % 16 == 15) c.Rot(3);
                }
                sym2raw[count++] = i;
            }
        }
        //assert count == 15582;
    }

    public static void CreatePrun()
    {
        for (int i = 0; i < csprun.Length; i++)
            csprun[i] = -1;
        csprun[0] = 0;
        int depth = 0;
        int done = 1;

        while (done != 15582)
        {
            bool inv = depth > 4;
            int select = inv ? -1 : depth;
            int check = inv ? depth : -1;
            depth++;
            for (int i = 0; i < 15582; i++)
            {
                if (csprun[i] != select)
                {
                    continue;
                }
                for (int m = 0; m < 27; m++)
                {
                    int idx = ckmv[i, m] >> 6; //>
                    if (csprun[idx] != check)
                    {
                        continue;
                    }
                    ++done;
                    if (inv)
                    {
                        csprun[i] = (sbyte)depth;
                        break;
                    }
                    else
                    {
                        csprun[idx] = (sbyte)depth;
                    }
                }
            }
            //			//System.out.println(string.format("%2d%10d", depth, done));
        }

    }

    public static void CreateMoveTable()
    {
        //System.out.println("Create Phase1 Center Move Table...");
        Center1 c = new();
        Center1 d = new();
        for (int i = 0; i < 15582; i++)
        {
            d.Set(sym2raw[i]);
            for (int m = 0; m < 36; m++)
            {
                c.Set(d);
                c.Move(m);
                ckmv[i, m] = c.Getsym();
            }
        }
    }

    public sbyte[] ct = new sbyte[24];

    public Center1()
    {
        for (int i = 0; i < 8; i++)
        {
            ct[i] = 1;
        }
        for (int i = 8; i < 24; i++)
        {
            ct[i] = 0;
        }
    }

    public Center1(sbyte[] ct)
    {
        for (int i = 0; i < 24; i++)
        {
            this.ct[i] = ct[i];
        }
    }

    public Center1(CenterCube c, int urf)
    {
        for (int i = 0; i < 24; i++)
        {
            ct[i] = (sbyte)((c.ct[i] % 3 == urf) ? 1 : 0);
        }
    }

    public void Move(int m)
    {
        int key = m % 3;
        m /= 3;
        switch (m)
        {
            case 0: //U
                Swap(ct, 0, 1, 2, 3, key);
                break;
            case 1: //R
                Swap(ct, 16, 17, 18, 19, key);
                break;
            case 2: //F
                Swap(ct, 8, 9, 10, 11, key);
                break;
            case 3: //D
                Swap(ct, 4, 5, 6, 7, key);
                break;
            case 4: //L
                Swap(ct, 20, 21, 22, 23, key);
                break;
            case 5: //B
                Swap(ct, 12, 13, 14, 15, key);
                break;
            case 6: //u
                Swap(ct, 0, 1, 2, 3, key);
                Swap(ct, 8, 20, 12, 16, key);
                Swap(ct, 9, 21, 13, 17, key);
                break;
            case 7: //r
                Swap(ct, 16, 17, 18, 19, key);
                Swap(ct, 1, 15, 5, 9, key);
                Swap(ct, 2, 12, 6, 10, key);
                break;
            case 8: //f
                Swap(ct, 8, 9, 10, 11, key);
                Swap(ct, 2, 19, 4, 21, key);
                Swap(ct, 3, 16, 5, 22, key);
                break;
            case 9: //d
                Swap(ct, 4, 5, 6, 7, key);
                Swap(ct, 10, 18, 14, 22, key);
                Swap(ct, 11, 19, 15, 23, key);
                break;
            case 10://l
                Swap(ct, 20, 21, 22, 23, key);
                Swap(ct, 0, 8, 4, 14, key);
                Swap(ct, 3, 11, 7, 13, key);
                break;
            case 11://b
                Swap(ct, 12, 13, 14, 15, key);
                Swap(ct, 1, 20, 7, 18, key);
                Swap(ct, 0, 23, 6, 17, key);
                break;
        }
    }

    public void Set(int idx)
    {
        int r = 8;
        for (int i = 23; i >= 0; i--)
        {
            ct[i] = 0;
            if (idx >= Cnk[i, r])
            {
                idx -= Cnk[i, r--];
                ct[i] = 1;
            }
        }
    }

    public int Get()
    {
        int idx = 0;
        int r = 8;
        for (int i = 23; i >= 0; i--)
        {
            if (ct[i] == 1)
            {
                idx += Cnk[i, r--];
            }
        }
        return idx;
    }

    public int Getsym()
    {
        if (raw2sym != null)
        {
            return raw2sym[Get()];
        }
        for (int j = 0; j < 48; j++)
        {
            int cord = Raw2sym(Get());
            if (cord != -1)
                return cord * 64 + j;
            Rot(0);
            if (j % 2 == 1) Rot(1);
            if (j % 8 == 7) Rot(2);
            if (j % 16 == 15) Rot(3);
        }
        //System.out.print('e');
        return -1;
    }

    private static int Raw2sym(int n)
    {
        int m = Array.BinarySearch(sym2raw, n);
        return (m >= 0 ? m : -1);
    }

    public void Set(Center1 c)
    {
        for (int i = 0; i < 24; i++)
        {
            this.ct[i] = c.ct[i];
        }
    }

    public void Rot(int r)
    {
        switch (r)
        {
            case 0:
                Move(ux2);
                Move(dx2);
                break;
            case 1:
                Move(rx1);
                Move(lx3);
                break;
            case 2:
                Swap(ct, 0, 3, 1, 2, 1);
                Swap(ct, 8, 11, 9, 10, 1);
                Swap(ct, 4, 7, 5, 6, 1);
                Swap(ct, 12, 15, 13, 14, 1);
                Swap(ct, 16, 19, 21, 22, 1);
                Swap(ct, 17, 18, 20, 23, 1);
                break;
            case 3:
                Move(ux1);
                Move(dx3);
                Move(fx1);
                Move(bx3);
                break;
        }
    }
    /*
    0	I
    1	y2
    2	x
    3	xy2
    4	x2
    5	z2
    6	x'
    7	x'y2
    16	yz
    17	y'z'
    18	y2z
    19	z'
    20	y'z
    21	yz'
    22	z
    23	zy2
    32	y'x'
    33	yx
    34	y'
    35	y
    36	y'x
    37	yx'
    38	yz2
    39	y'z2
        */
    public static string[] rot2str = ["", "y2", "x", "x y2", "x2", "z2", "x'", "x' y2", "", "", "", "", "", "", "", "",
    "y z", "y' z'", "y2 z", "z'", "y' z", "y z'", "z", "z y2", "", "", "", "", "", "", "", "",
    "y' x'", "y x", "y'", "y", "y' x", "y x'", "y z2", "y' z2",  "", "", "", "", "", "", "", ""];


    public void Rotate(int r)
    {
        for (int j = 0; j < r; j++)
        {
            Rot(0);
            if (j % 2 == 1) Rot(1);
            if (j % 8 == 7) Rot(2);
            if (j % 16 == 15) Rot(3);
        }
    }

    public static int GetSolvedSym(CenterCube cube)
    {
        Center1 c = new(cube.ct);
        for (int j = 0; j < 48; j++)
        {
            bool check = true;
            for (int i = 0; i < 24; i++)
            {
                if (c.ct[i] != FullCube.centerFacelet[i] / 16)
                {
                    check = false;
                    break;
                }
            }
            if (check)
            {
                return j;
            }
            c.Rot(0);
            if (j % 2 == 1) c.Rot(1);
            if (j % 8 == 7) c.Rot(2);
            if (j % 16 == 15) c.Rot(3);
        }
        return -1;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override bool Equals(object obj)
    {
        if (obj is Center1 c)
        {
            for (int i = 0; i < 24; i++)
            {
                if (ct[i] != c.ct[i])
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public static void InitSym()
    {
        Center1 c = new();
        for (sbyte i = 0; i < 24; i++)
        {
            c.ct[i] = i;
        }
        Center1 d = new(c.ct);
        Center1 e = new(c.ct);
        Center1 f = new(c.ct);

        for (int i = 0; i < 48; i++)
        {
            for (int j = 0; j < 48; j++)
            {
                for (int k = 0; k < 48; k++)
                {
                    if (c.Equals(d))
                    {
                        symmult[i, j] = k;
                        if (k == 0)
                        {
                            syminv[i] = j;
                        }
                    }
                    d.Rot(0);
                    if (k % 2 == 1) d.Rot(1);
                    if (k % 8 == 7) d.Rot(2);
                    if (k % 16 == 15) d.Rot(3);
                }
                c.Rot(0);
                if (j % 2 == 1) c.Rot(1);
                if (j % 8 == 7) c.Rot(2);
                if (j % 16 == 15) c.Rot(3);
            }
            c.Rot(0);
            if (i % 2 == 1) c.Rot(1);
            if (i % 8 == 7) c.Rot(2);
            if (i % 16 == 15) c.Rot(3);
        }

        for (int i = 0; i < 48; i++)
        {
            c.Set(e);
            c.Rotate(syminv[i]);
            for (int j = 0; j < 36; j++)
            {
                d.Set(c);
                d.Move(j);
                d.Rotate(i);
                for (int k = 0; k < 36; k++)
                {
                    f.Set(e);
                    f.Move(k);
                    if (f.Equals(d))
                    {
                        symmove[i, j] = k;
                        break;
                    }
                }
            }
        }

        c.Set(0);
        for (int i = 0; i < 48; i++)
        {
            finish[syminv[i]] = c.Get();
            c.Rot(0);
            if (i % 2 == 1) c.Rot(1);
            if (i % 8 == 7) c.Rot(2);
            if (i % 16 == 15) c.Rot(3);
        }
    }
}
