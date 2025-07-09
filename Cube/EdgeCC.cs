using static Cube.Tools;

namespace Cube;
public class EdgeCC
{
    public readonly State FirstCycle;
    public readonly State[] OtherCycles;
    private const int _Perm = 12, _Ori = 2;
    public const long Sum = 980995276800;
    public readonly int Algx2, Cycles, Parity, Float1, Bad1, Float2, Bad2, Float3, Bad3, Float4, Bad4, Float5, Bad5;
    public readonly long Count;

    public EdgeCC(int FirstCycleLength, State[] OtherCycles)
    {
        this.FirstCycle = (FirstCycleLength, (_Perm * _Ori - OtherCycles.Sum(x => x.ori)) % _Ori);
        this.OtherCycles = OtherCycles;
        int baseAlgs = OtherCycles.Sum(x => x.perm > 1 ? x.perm + 1 : 0) + FirstCycleLength - 1;
        int twistAlgs = (OtherCycles.Count(x => x.perm == 1 && x.ori != 0) + 3) >> 2;
        Parity = baseAlgs & 1;
        Cycles = OtherCycles.Count(x => x.perm != 1);
        Float1 = OtherCycles.Count(x => x.perm == 1 && x.ori == 0);
        Bad1 = OtherCycles.Count(x => x.perm == 1 && x.ori > 0);
        Float2 = OtherCycles.Count(x => x.perm == 2 && x.ori == 0);
        Bad2 = OtherCycles.Count(x => x.perm == 2 && x.ori > 0);
        Float3 = OtherCycles.Count(x => x.perm == 3 && x.ori == 0);
        Bad3 = OtherCycles.Count(x => x.perm == 3 && x.ori > 0);
        Float4 = OtherCycles.Count(x => x.perm == 4 && x.ori == 0);
        Bad4 = OtherCycles.Count(x => x.perm == 4 && x.ori > 0);
        Float5 = OtherCycles.Count(x => x.perm == 5 && x.ori == 0);
        Bad5 = OtherCycles.Count(x => x.perm == 5 && x.ori > 0);
        Count = FactI64[_Perm - 1];
        foreach (var i in OtherCycles)
            Count /= i.perm;
        Count *= 1L << (_Perm - 1 - OtherCycles.Length);
        foreach (var i in OtherCycles.GroupBy(x => x))
            Count /= FactI64[i.Count()];
        Algx2 = baseAlgs + (twistAlgs - Float3) * 2;
    }
    public Edge GetInstance(int Buffer=0)
    {
        State[] instance = new State[_Perm];
        int head, remain = FirstCycle.perm, current, i = 0, o = 0;
        int[] perm = RandomPermutation(_Perm), ori = RandomOrientation(_Perm, _Ori);
        current = head = Buffer >> 1;
        perm[Array.IndexOf(perm, head)] = perm[0];
        perm[0] = head;
        while ((--remain) > 0)
        {
            i++;
            instance[current].perm = perm[i];
            instance[current].ori = ori[i];
            current = perm[i];
            o += ori[i];
        }
        instance[current].perm = head;
        instance[current].ori = (FirstCycle.ori + 24 - o) % _Ori;
        foreach (var cycle in OtherCycles)
        {
            o = 0;
            remain = cycle.perm;
            current = head = perm[++i];
            while ((--remain) > 0)
            {
                i++;
                instance[current].perm = perm[i];
                instance[current].ori = ori[i];
                current = perm[i];
                o += ori[i];
            }
            instance[current].perm = head;
            instance[current].ori = (cycle.ori + 24 - o) % _Ori;
        }
        return new Edge { state = instance };
    }

#region
    public static readonly List<EdgeCC> OddList, EvenList;
    public static IEnumerable<EdgeCC> AllList => OddList.Concat(EvenList);
    private static void GenerateOri(int index, int[] sizes, int[] colors, List<int> indexes)
    {
        if (index == sizes.Length)
        {
            State[] OtherCycles = new State[sizes.Length];
            for (int i = 0; i < index; i++)
                OtherCycles[i] = (sizes[i], colors[i]);
            var t = new EdgeCC(12 - sizes.Sum(), OtherCycles);
            (t.Parity == 1 ? OddList : EvenList).Add(t);
        }
        else
        {
            int j = indexes.IndexOf(index);
            for (int i = j >= 0 ? 1 : colors[index - 1]; i >= 0; i--)
            {
                colors[index] = i;
                GenerateOri(index + 1, sizes, colors, indexes);
            }
        }
    }
    static EdgeCC()
    {
        OddList = [];
        EvenList = [];
        List<int> temp = [];
        foreach (var s in GeneratePerm(12))
        {
            int p = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != p)
                {
                    p = s[i];
                    temp.Add(i);
                }
            }
            GenerateOri(0, s, new int[s.Length], temp);
            temp.Clear();
        }
    }
#endregion

}
