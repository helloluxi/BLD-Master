namespace Cube;

public static class Tools
{
    public static readonly Random rd = new();
    public static readonly int[] Pow3 = new int[9];
    public static readonly int[] FactI = new int[13];
    public static readonly long[] FactI64 = new long[21];
    public static readonly Int128[] FactI128 = new Int128[25];
    static Tools()
    {
        FactI[0] = 1;
        for (int i = 1; i < FactI.Length; i++)
            FactI[i] = FactI[i - 1] * i;
        FactI64[0] = 1;
        for (int i = 1; i < FactI64.Length; i++)
            FactI64[i] = FactI64[i - 1] * i;
        FactI128[0] = 1;
        for (int i = 1; i < FactI128.Length; i++)
            FactI128[i] = FactI128[i - 1] * i;
        Pow3[0] = 1;
        for (int i = 1; i < Pow3.Length; i++)
        {
            Pow3[i] = Pow3[i - 1] * 3;
        }
    }
    public static int GetNParity(int idx, int n)
    {
        int p = 0;
        for (int i = n - 2; i >= 0; i--)
        {
            p ^= idx % (n - i);
            idx /= n - i;
        }
        return p & 1;
    }
    public static int[] RandomPermutation(int n)
    {
        int[] perm = [.. Enumerable.Range(0, n)];
        for (int i = 0; i < n - 1; i++)
        {
            int j = i + rd.Next(n - i);
            (perm[i], perm[j]) = (perm[j], perm[i]);
        }
        return perm;
    }
    public static int[] RandomOrientation(int n, int numOri)
    {
        int[] ori = new int[n];
        int sumOri = 0;
        for (int i = 0; i < n - 1; i++)
            sumOri += ori[i] = rd.Next(numOri);
        ori[n - 1] = (numOri - sumOri % numOri) % numOri;
        return ori;
    }
    public static int GetParity(int[] arr)
    {
        int parity = 0;
        for (int i = 0; i < arr.Length - 1; i++)
            for (int j = i + 1; j < arr.Length; j++)
                if (arr[i] > arr[j])
                    parity ^= 1;
        return parity;
    }
    public static IEnumerable<int[]> GeneratePerm(int perm)
    {
        for (int size = 0; size < perm; size++)
        {
            int[] sizes = [.. Enumerable.Range(0, size).Select(x => 1)];
            yield return sizes;
        OUT:
            for (int head = 0; head < sizes.Length; head++)
            {
                sizes[head]++;
                Array.Fill(sizes, sizes[head], 0, head);
                if (sizes.Sum() <= perm - 1)
                {
                    yield return sizes;
                    goto OUT;
                }
            }
        }
    }
    public static IEnumerable<CycleConfig[]> GenerateOri(int index, int[] ps, int[] os, List<int> indexes, int ori, int perm)
    {
        if (index == ps.Length)
        {
            var OtherCycles = ps.Select((size, i) => (CycleConfig)(size, os[i])).ToArray();
            int FirstCyclePerm = perm - ps.Sum();
            int FirstCycleOri = (ori * perm - os.Sum()) % ori;
            yield return [(FirstCyclePerm, FirstCycleOri), .. OtherCycles];
        }
        else
        {
            int j = indexes.IndexOf(index);
            int firstOri = j >= 0 ? ori - 1 : os[index - 1];
            for (int i = firstOri; i >= 0; i--)
            {
                os[index] = i;
                foreach (var item in GenerateOri(index + 1, ps, os, indexes, ori, perm))
                    yield return item;
            }
        }
    }
    public static IEnumerable<CycleConfig[]> GenerateCycleConfigs(int perm, int ori)
    {
        List<int> indexes = [];
        foreach (var ps in GeneratePerm(perm))
        {
            int p = 0;
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i] != p)
                {
                    p = ps[i];
                    indexes.Add(i);
                }
            }
            foreach (var item in GenerateOri(0, ps, new int[ps.Length], indexes, ori, perm))
                yield return item;
            indexes.Clear();
        }
    }
}
