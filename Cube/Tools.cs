namespace Cube;
public static class Tools
{
    public static readonly Random rd = new();
    public static readonly int[] Pow3 = new int[9];
    public static readonly int[] FactI = new int[13];
    public static readonly long[] FactI64 = new long[21];
    public static readonly Int128[] FactI128 = new Int128[25];
    static Tools(){
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
        for (int i = 1; i < Pow3.Length; i++){
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
    public static IEnumerable<int[]> GeneratePerm(int Perm)
    {
        for (int size = 0, i, j; size < Perm; ++size)
        {
            int[] sizes = [.. Enumerable.Range(0, size).Select(x => 1)];
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
