using System;
using System.Collections.Generic;

namespace Luxi
{
    public static class Tools
    {
        public static readonly Random rd = new Random();
        public static int FactI(int x) => x > 0 ? x * FactI(x - 1) : 1;
        public static long FactI64(int x) => x > 0 ? x * FactI64(x - 1) : 1;
        public static Int128 FactI128(int x) => x > 0 ? x * FactI128(x - 1) : 1;
        public static int Pow(int x, int y){
            int r = 1;
            while (y > 0){
                if ((y & 1) == 1)
                    r *= x;
                x *= x;
                y >>= 1;
            }
            return r;
        }
        public static long PowL(int x, int y){
            long r = 1;
            while (y > 0){
                if ((y & 1) == 1)
                    r *= x;
                x *= x;
                y >>= 1;
            }
            return r;
        }
        public static bool GetNParity(int idx, int n)
        {
            int p = 0;
            for (int i = n - 2; i >= 0; i--)
            {
                p ^= idx % (n - i);
                idx /= n - i;
            }
            return (p & 1) == 1;
        }
        public static void SetNPerm(int idx, int n, out int[] arr)
        {
            arr = new int[n];
            arr[n - 1] = 0;
            for (int i = n - 2; i >= 0; i--)
            {
                arr[i] = idx % (n - i);
                idx /= n - i;
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j] >= arr[i])
                        arr[j]++;
                }
            }
        }
        public static int GetNPerm(int[] arr, int n)
        {
            int idx = 0;
            for (int i = 0; i < n; i++)
            {
                idx *= n - i;
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j] < arr[i])
                    {
                        idx++;
                    }
                }
            }
            return idx;
        }
        public static void SetNFlip(int val, int n, out int[] arr)
        {
            arr = new int[n];
            int color = 0;
            for (int i = 0; i < n - 1; i++, val >>= 1)
                color += arr[i] = val & 1;
            arr[n - 1] = color & 1;
        }
        public static void SetNTwist(int val, int n, out int[] arr)
        {
            arr = new int[n];
            int color = 0;
            for (int i = 0; i < n - 1; i++, val /= 3)
                color += arr[i] = val % 3;
            arr[n - 1] = (24 - color) % 3;
        }
        public static IEnumerable<int[]> SizeGenerater(int Perm)
        {
            for (int size = 0, i, j; size < Perm; ++size)
            {
                int[] sizes = Enumerable.Range(0, size).Select(x => 1).ToArray();
                yield return sizes;
            OUT:
                for (i = 0; i < sizes.Length; i++)
                {
                    sizes[i]++;
                    for (j = 0; j < i; j++)
                        sizes[j] = sizes[i];
                    if (sizes.Sum() <= Perm - 1)
                    {
                        yield return sizes;
                        goto OUT;
                    }
                }
            }
        }
    }
}
