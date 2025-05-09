﻿using System.Text;
namespace CS.ThreePhase;

public static class Util
{

    public static int[,] Cnk = new int[25, 25];
    public static int[] FactI = new int[13];

    static Util()
    {
        for (int i = 0; i < 25; i++)
        {
            Cnk[i, i] = 1;
            Cnk[i, 0] = 1;
        }
        for (int i = 1; i < 25; i++)
        {
            for (int j = 1; j <= i; j++)
            {
                Cnk[i, j] = Cnk[i - 1, j] + Cnk[i - 1, j - 1];
            }
        }
        FactI[0] = 1;
        for (int i = 0; i < 12; i++)
        {
            FactI[i + 1] = FactI[i] * (i + 1);
        }
    }

    public static int[] Tomove(string s)
    {
        int[] arr = new int[s.Length];
        int j = 0;
        int axis = -1;
        for (int i = 0, Length = s.Length; i < Length; i++)
        {
            switch (s[i])
            {
                case 'U': axis = 0; break;
                case 'R': axis = 3; break;
                case 'F': axis = 6; break;
                case 'D': axis = 9; break;
                case 'L': axis = 12; break;
                case 'B': axis = 15; break;
                case 'u': axis = 18; break;
                case 'r': axis = 21; break;
                case 'f': axis = 24; break;
                case 'd': axis = 27; break;
                case 'l': axis = 30; break;
                case 'b': axis = 33; break;
                case ' ':
                    if (axis != -1)
                    {
                        arr[j++] = axis;
                    }
                    axis = -1;
                    break;
                case '2': axis++; break;
                case '\'': axis += 2; break;
                case 'w': axis += 18; break;
                default: continue;
            }

        }
        if (axis != -1) arr[j++] = axis;
        int[] ret = new int[j];
        while (--j >= 0)
        {
            ret[j] = arr[j];
        }
        return ret;
    }

    public static string Tostr(int[] moves)
    {
        StringBuilder s = new();
        foreach (int m in moves)
        {
            s.Append(Moves.move2str[m]).Append(' ');
        }
        return s.ToString();
    }

    public static void Swap(int[] arr, int a, int b, int c, int d, int key)
    {
        int temp;
        switch (key)
        {
            case 0:
                temp = arr[d];
                arr[d] = arr[c];
                arr[c] = arr[b];
                arr[b] = arr[a];
                arr[a] = temp;
                return;
            case 1:
                temp = arr[a];
                arr[a] = arr[c];
                arr[c] = temp;
                temp = arr[b];
                arr[b] = arr[d];
                arr[d] = temp;
                return;
            case 2:
                temp = arr[a];
                arr[a] = arr[b];
                arr[b] = arr[c];
                arr[c] = arr[d];
                arr[d] = temp;
                return;
        }
    }

    public static void Swap(sbyte[] arr, int a, int b, int c, int d, int key)
    {
        sbyte temp;
        switch (key)
        {
            case 0:
                temp = arr[d];
                arr[d] = arr[c];
                arr[c] = arr[b];
                arr[b] = arr[a];
                arr[a] = temp;
                return;
            case 1:
                temp = arr[a];
                arr[a] = arr[c];
                arr[c] = temp;
                temp = arr[b];
                arr[b] = arr[d];
                arr[d] = temp;
                return;
            case 2:
                temp = arr[a];
                arr[a] = arr[b];
                arr[b] = arr[c];
                arr[c] = arr[d];
                arr[d] = temp;
                return;
        }
    }

    public static void Set8Perm(sbyte[] arr, int idx)
    {
        int val = 0x76543210;
        for (int i = 0; i < 7; i++)
        {
            int p = FactI[7 - i];
            int v = idx / p;
            idx -= v * p;
            v <<= 2;
            arr[i] = (sbyte)((val >> v) & 0xf);
            int m = (1 << v) - 1;
            val = (val & m) + ((val >> 4) & ~m);
        }
        arr[7] = (sbyte)val;
    }

    public static int Parity(sbyte[] arr)
    {
        int parity = 0;
        for (int i = 0, len = arr.Length; i < len; i++)
        {
            for (int j = i; j < len; j++)
            {
                if (arr[i] > arr[j])
                {
                    parity ^= 1;
                }
            }
        }
        return parity;
    }
}
