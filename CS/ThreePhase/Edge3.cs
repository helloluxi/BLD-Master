
namespace CS.ThreePhase;

/*
                    13	1	
                4			17 
                16			5 
                    0	12	
    4	16			0	12			5	17			1	13	
9			20	20			11	11			22	22			9 
21			8	8			23	23			10	10			21 
    19	7			15	3			18	6			14	2	
                    15	3	
                7			18 
                19			6 
                    2	14	
    */
public class Edge3
{
    public static bool IS_64BIT_PLATFORM = true;

    public const int N_SYM = 1538;
    public const int N_RAW = 20160;
    public const int N_EPRUN = N_SYM * N_RAW;
    public const int MAX_DEPTH = 10;

    public static readonly int[] prunValues = [1, 4, 16, 55, 324, 1922, 12275, 77640, 485359, 2778197, 11742425, 27492416, 31002941, 31006080];

    public static int[] eprun = new int[N_EPRUN / 16];

    public static int[] sym2raw = new int[N_SYM];
    public static char[] symstate = new char[N_SYM];
    public static int[] raw2sym = new int[11880];

    public static int[] syminv = [0, 1, 6, 3, 4, 5, 2, 7];

    public int[] edge = new int[12];
    public int[] edgeo = new int[12];
    public int[] temp;
    public bool isStd = true;

    public static int[,] mvrot = new int[20 * 8, 12];
    public static int[,] mvroto = new int[20 * 8, 12];

    public static int[] factX = [1, 1, 2 / 2, 6 / 2, 24 / 2, 120 / 2, 720 / 2, 5040 / 2, 40320 / 2, 362880 / 2, 3628800 / 2, 39916800 / 2, 479001600 / 2];

    public static int done = 0;

    public static double InitStatus()
    {
        return done * 1.0 / prunValues[MAX_DEPTH - 1];
    }

    public static void InitMvrot()
    {
        Edge3 e = new();
        for (int m = 0; m < 20; m++)
        {
            for (int r = 0; r < 8; r++)
            {
                e.Set(0);
                e.Move(m);
                e.Rotate(r);
                for (int i = 0; i < 12; i++)
                {
                    mvrot[m << 3 | r, i] = e.edge[i];
                }
                e.Std();
                for (int i = 0; i < 12; i++)
                {
                    mvroto[m << 3 | r, i] = e.temp[i];
                }
            }
        }
    }

    public static void InitRaw2Sym()
    {
        Edge3 e = new();
        sbyte[] occ = new sbyte[11880 / 8];
        int count = 0;
        for (int i = 0; i < 11880; i++)
        {
            if ((occ[i >> 3] & (1 << (i & 7))) == 0) //>
            {
                e.Set(i * factX[8]);
                for (int j = 0; j < 8; j++)
                {
                    int idx = e.Get(4);
                    if (idx == i)
                    {
                        symstate[count] |= (char)(1 << j);
                    }
                    occ[idx >> 3] |= (sbyte)(1 << (idx & 7));
                    raw2sym[idx] = count << 3 | syminv[j];
                    e.Rot(0);
                    if (j % 2 == 1)
                    {
                        e.Rot(1);
                        e.Rot(2);
                    }
                }
                sym2raw[count++] = i;
            }
        }
        //assert count == 1538;
    }

    public static void SetPruning(int[] table, int index, int value)
    {
        table[index >> 4] ^= (0x3 ^ value) << ((index & 0xf) << 1);
    }

    public static int GetPruning(int[] table, int index)
    {
        return (table[index >> 4] >> ((index & 0xf) << 1)) & 0x3;
    }

    public static int Getprun(int edge, int prun)
    {
        int depm3 = GetPruning(eprun, edge);
        if (depm3 == 0x3)
        {
            return MAX_DEPTH;
        }
        return (depm3 - prun + 16) % 3 + prun - 1;
    }

    public static int Getprun(int edge)
    {
        Edge3 e = new();
        int depth = 0;
        int depm3 = GetPruning(eprun, edge);
        if (depm3 == 0x3)
        {
            return MAX_DEPTH;
        }
        while (edge != 0)
        {
            if (depm3 == 0)
            {
                depm3 = 2;
            }
            else
            {
                depm3--;
            }

            int symcord1 = edge / N_RAW;
            int cord1 = sym2raw[symcord1];
            int cord2 = edge % N_RAW;
            e.Set(cord1 * N_RAW + cord2);

            for (int m = 0; m < 17; m++)
            {
                int cord1x = Getmvrot(e.edge, m << 3, 4);
                int symcord1x = raw2sym[cord1x];
                int symx = symcord1x & 0x7;
                symcord1x >>= 3;
                int cord2x = Getmvrot(e.edge, m << 3 | symx, 10) % N_RAW;
                int idx = symcord1x * N_RAW + cord2x;
                if (GetPruning(eprun, idx) == depm3)
                {
                    depth++;
                    edge = idx;
                    break;
                }
            }
        }
        return depth;
    }

    public static void CreatePrun()
    {
        Edge3 e = new();
        Edge3 f = new();
        Edge3 g = new();

        for (int i = 0; i < eprun.Length; i++)
            eprun[i] = -1;
        int depth = 0;
        done = 1;
        SetPruning(eprun, 0, 0);

        while (done != N_EPRUN)
        {
            bool inv = depth > 9;
            int depm3 = depth % 3;
            int dep1m3 = (depth + 1) % 3;
            int find = inv ? 0x3 : depm3;
            int chk = inv ? depm3 : 0x3;

            if (depth >= MAX_DEPTH - 1)
            {
                break;
            }

            for (int i_ = 0; i_ < N_EPRUN; i_ += 16)
            {
                int val = eprun[i_ >> 4];
                if (!inv && val == -1)
                {
                    continue;
                }
                for (int i = i_, end = i_ + 16; i < end; i++, val >>= 2)
                {
                    if ((val & 0x3) != find)
                    {
                        continue;
                    }
                    int symcord1 = i / N_RAW;
                    int cord1 = sym2raw[symcord1];
                    int cord2 = i % N_RAW;
                    e.Set(cord1 * N_RAW + cord2);

                    for (int m = 0; m < 17; m++)
                    {
                        int cord1x = Getmvrot(e.edge, m << 3, 4);
                        int symcord1x = raw2sym[cord1x];
                        int symx = symcord1x & 0x7;
                        symcord1x >>= 3;
                        int cord2x = Getmvrot(e.edge, m << 3 | symx, 10) % N_RAW;
                        int idx = symcord1x * N_RAW + cord2x;
                        if (GetPruning(eprun, idx) != chk)
                        {
                            continue;
                        }
                        SetPruning(eprun, inv ? i : idx, dep1m3);
                        done++;
                        // if ((done & 0x3ffff) == 0) {
                        // 	System.out.print(string.format("%d\r", done));
                        // }
                        if (inv)
                        {
                            break;
                        }
                        char symState = symstate[symcord1x];
                        if (symState == 1)
                        {
                            continue;
                        }
                        f.Set(e);
                        f.Move(m);
                        f.Rotate(symx);
                        for (int j = 1; (symState >>= 1) != 0; j++)
                        {
                            if ((symState & 1) != 1)
                            {
                                continue;
                            }
                            g.Set(f);
                            g.Rotate(j);
                            int idxx = symcord1x * N_RAW + g.Get(10) % N_RAW;
                            if (GetPruning(eprun, idxx) == chk)
                            {
                                SetPruning(eprun, idxx, dep1m3);
                                done++;
                                // if ((done & 0x3ffff) == 0) {
                                // 	System.out.print(string.format("%d\r", done));
                                // }
                            }
                        }
                    }
                }
            }
            depth++;
            //System.out.println(depth + "\t" + done);
        }
    }

    public static int[] FullEdgeMap = [0, 2, 4, 6, 1, 3, 7, 5, 8, 9, 10, 11];

    public int Getsym()
    {
        int cord1x = Get(4);
        int symcord1x = raw2sym[cord1x];
        int symx = symcord1x & 0x7;
        symcord1x >>= 3;
        Rotate(symx);
        int cord2x = Get(10) % N_RAW;
        return symcord1x * N_RAW + cord2x;
    }

    public int Set(EdgeCube c)
    {
        if (temp == null)
        {
            temp = new int[12];
        }
        for (int i = 0; i < 12; i++)
        {
            temp[i] = i;
            edge[i] = c.ep[FullEdgeMap[i] + 12] % 12;
        }
        int parity = 1; //because of FullEdgeMap
        for (int i = 0; i < 12; i++)
        {
            while (edge[i] != i)
            {
                int t = edge[i];
                edge[i] = edge[t];
                edge[t] = t;
                int s = temp[i];
                temp[i] = temp[t];
                temp[t] = s;
                parity ^= 1;
            }
        }
        for (int i = 0; i < 12; i++)
        {
            edge[i] = temp[c.ep[FullEdgeMap[i]] % 12];
        }
        return parity;
    }

    public void Set(Edge3 e)
    {
        for (int i = 0; i < 12; i++)
        {
            edge[i] = e.edge[i];
            edgeo[i] = e.edgeo[i];
        }
        isStd = e.isStd;
    }

    public static int Getmvrot(int[] ep, int mrIdx, int end)
    {
        int idx = 0;

        if (IS_64BIT_PLATFORM)
        {
            long val = 0xba9876543210L;
            for (int i = 0; i < end; i++)
            {
                int v = mvroto[mrIdx, ep[mvrot[mrIdx, i]]] << 2;
                idx *= 12 - i;
                idx += (int)((val >> v) & 0xf);
                val -= 0x111111111110L << v;
            }
        }
        else
        {   //long is not as fast as expected
            int vall = 0x76543210;
            int valh = 0xba98;
            for (int i = 0; i < end; i++)
            {
                int v = mvroto[mrIdx, ep[mvrot[mrIdx, i]]] << 2;
                idx *= 12 - i;
                if (v >= 32)
                {
                    idx += (valh >> (v - 32)) & 0xf;
                    valh -= 0x1110 << (v - 32);
                }
                else
                {
                    idx += (vall >> v) & 0xf;
                    valh -= 0x1111;
                    vall -= 0x11111110 << v;
                }
            }
        }
        return idx;

    }

    public void Std()
    {
        if (temp == null)
        {
            temp = new int[12];
        }
        for (int i = 0; i < 12; i++)
        {
            temp[edgeo[i]] = i;
        }

        for (int i = 0; i < 12; i++)
        {
            edge[i] = temp[edge[i]];
            edgeo[i] = i;
        }
        isStd = true;
    }

    public int Get(int end)
    {
        if (!isStd)
        {
            Std();
        }
        int idx = 0;
        long val = 0xba9876543210L;
        for (int i = 0; i < end; i++)
        {
            int v = edge[i] << 2;
            idx *= 12 - i;
            idx += (int)((val >> v) & 0xf);
            val -= 0x111111111110L << v;
        }
        return idx;
    }

    public void Set(int idx)
    {
        if (IS_64BIT_PLATFORM)
        {
            long val = 0xba9876543210L;
            int parity = 0;
            for (int i = 0; i < 11; i++)
            {
                int p = factX[11 - i];
                int v = idx / p;
                idx %= p;
                parity ^= v;
                v <<= 2;
                edge[i] = (int)((val >> v) & 0xf);
                long m = (1L << v) - 1;
                val = (val & m) + ((val >> 4) & ~m);
            }
            if ((parity & 1) == 0)
            {
                edge[11] = (int)val;
            }
            else
            {
                edge[11] = edge[10];
                edge[10] = (int)val;
            }
        }
        else
        {
            int vall = 0x76543210;
            int valh = 0xba98;
            int parity = 0;
            for (int i = 0; i < 11; i++)
            {
                int p = factX[11 - i];
                int v = idx / p;
                idx %= p;
                parity ^= v;
                v <<= 2;
                if (v >= 32)
                {
                    v -= 32;
                    edge[i] = valh >> v & 0xf;
                    int m = (1 << v) - 1;
                    valh = (valh & m) + ((valh >> 4) & ~m);
                }
                else
                {
                    edge[i] = vall >> v & 0xf;
                    int m = (1 << v) - 1;
                    vall = (vall & m) + ((vall >> 4) & ~m) + (valh << 28);//>
                    valh >>= 4;
                }
            }
            if ((parity & 1) == 0)
            {
                edge[11] = vall;
            }
            else
            {
                edge[11] = edge[10];
                edge[10] = vall;
            }
        }
        for (int i = 0; i < 12; i++)
        {
            edgeo[i] = i;
        }
        isStd = true;
    }

    public void Move(int i)
    {
        isStd = false;
        switch (i)
        {
            case 0:     //U
                Circle(edge, 0, 4, 1, 5);
                Circle(edgeo, 0, 4, 1, 5);
                break;
            case 1:     //U2
                Swap(edge, 0, 4, 1, 5);
                Swap(edgeo, 0, 4, 1, 5);
                break;
            case 2:     //U'
                Circle(edge, 0, 5, 1, 4);
                Circle(edgeo, 0, 5, 1, 4);
                break;
            case 3:     //R2
                Swap(edge, 5, 10, 6, 11);
                Swap(edgeo, 5, 10, 6, 11);
                break;
            case 4:     //F
                Circle(edge, 0, 11, 3, 8);
                Circle(edgeo, 0, 11, 3, 8);
                break;
            case 5:     //F2
                Swap(edge, 0, 11, 3, 8);
                Swap(edgeo, 0, 11, 3, 8);
                break;
            case 6:     //F'
                Circle(edge, 0, 8, 3, 11);
                Circle(edgeo, 0, 8, 3, 11);
                break;
            case 7:     //D
                Circle(edge, 2, 7, 3, 6);
                Circle(edgeo, 2, 7, 3, 6);
                break;
            case 8:     //D2
                Swap(edge, 2, 7, 3, 6);
                Swap(edgeo, 2, 7, 3, 6);
                break;
            case 9:     //D'
                Circle(edge, 2, 6, 3, 7);
                Circle(edgeo, 2, 6, 3, 7);
                break;
            case 10:    //L2
                Swap(edge, 4, 8, 7, 9);
                Swap(edgeo, 4, 8, 7, 9);
                break;
            case 11:    //B
                Circle(edge, 1, 9, 2, 10);
                Circle(edgeo, 1, 9, 2, 10);
                break;
            case 12:    //B2
                Swap(edge, 1, 9, 2, 10);
                Swap(edgeo, 1, 9, 2, 10);
                break;
            case 13:    //B'
                Circle(edge, 1, 10, 2, 9);
                Circle(edgeo, 1, 10, 2, 9);
                break;
            case 14:    //u2
                Swap(edge, 0, 4, 1, 5);
                Swap(edgeo, 0, 4, 1, 5);
                Swap(edge, 9, 11);
                Swap(edgeo, 8, 10);
                break;
            case 15:    //r2
                Swap(edge, 5, 10, 6, 11);
                Swap(edgeo, 5, 10, 6, 11);
                Swap(edge, 1, 3);
                Swap(edgeo, 0, 2);
                break;
            case 16:    //f2
                Swap(edge, 0, 11, 3, 8);
                Swap(edgeo, 0, 11, 3, 8);
                Swap(edge, 5, 7);
                Swap(edgeo, 4, 6);
                break;
            case 17:    //d2
                Swap(edge, 2, 7, 3, 6);
                Swap(edgeo, 2, 7, 3, 6);
                Swap(edge, 8, 10);
                Swap(edgeo, 9, 11);
                break;
            case 18:    //l2
                Swap(edge, 4, 8, 7, 9);
                Swap(edgeo, 4, 8, 7, 9);
                Swap(edge, 0, 2);
                Swap(edgeo, 1, 3);
                break;
            case 19:    //b2
                Swap(edge, 1, 9, 2, 10);
                Swap(edgeo, 1, 9, 2, 10);
                Swap(edge, 4, 6);
                Swap(edgeo, 5, 7);
                break;
        }
    }

    public void Rot(int r)
    {
        isStd = false;
        switch (r)
        {
            case 0:
                Move(14);
                Move(17);
                break;
            case 1:
                Circlex(11, 5, 10, 6);//r
                Circlex(5, 10, 6, 11);
                Circlex(1, 2, 3, 0);
                Circlex(4, 9, 7, 8);//l'
                Circlex(8, 4, 9, 7);
                Circlex(0, 1, 2, 3);
                break;
            case 2:
                Swapx(4, 5); Swapx(5, 4);
                Swapx(11, 8); Swapx(8, 11);
                Swapx(7, 6); Swapx(6, 7);
                Swapx(9, 10); Swapx(10, 9);
                Swapx(1, 1); Swapx(0, 0);
                Swapx(3, 3); Swapx(2, 2);
                break;
        }
    }

    public void Rotate(int r)
    {
        while (r >= 2)
        {
            r -= 2;
            Rot(1);
            Rot(2);
        }
        if (r != 0)
        {
            Rot(0);
        }
    }


    public void Circle(int[] arr, int a, int b, int c, int d)
    {
        int temp = arr[d];
        arr[d] = arr[c];
        arr[c] = arr[b];
        arr[b] = arr[a];
        arr[a] = temp;
    }

    public void Swap(int[] arr, int a, int b, int c, int d)
    {
        int temp = arr[a];
        arr[a] = arr[c];
        arr[c] = temp;
        temp = arr[b];
        arr[b] = arr[d];
        arr[d] = temp;
    }

    public void Swap(int[] arr, int x, int y)
    {
        int temp = arr[x];
        arr[x] = arr[y];
        arr[y] = temp;
    }

    public void Swapx(int x, int y)
    {
        int temp = edge[x];
        edge[x] = edgeo[y];
        edgeo[y] = temp;
    }

    public void Circlex(int a, int b, int c, int d)
    {
        int temp = edgeo[d];
        edgeo[d] = edge[c];
        edge[c] = edgeo[b];
        edgeo[b] = edge[a];
        edge[a] = temp;
    }
}
