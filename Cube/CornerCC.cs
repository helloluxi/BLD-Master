using static Cube.Tools;
namespace Cube;
public class CornerCC {
    public readonly CycleConfig FirstCycle;
    public readonly CycleConfig[] OtherCycles;
    private const int _Perm = 8, _Ori = 3;
    public const long Sum = 88179840;
    public readonly int Algx2, Breaks, Parity, Float1, Bad1, Float2, Bad2, Float3, Bad3, Float4, Bad4, Float5, Bad5;
    public readonly int Count;
    public CornerCC(CycleConfig[] cycles)
    {
        FirstCycle = (cycles[0].perm, cycles[0].ori);
        OtherCycles = [.. cycles.Skip(1)];

        int baseAlgs = OtherCycles.Sum(x => x.perm > 1 ? x.perm + 1 : 0) + FirstCycle.perm - 1;
        Parity = baseAlgs & 1;
        int twistAlgs = 0;
        var twistOris = OtherCycles.Where(cycle => cycle.perm == 1 && cycle.ori != 0).Select(cycle => cycle.ori).ToList();
        for (int i = 0; i < twistOris.Count - 2; i++)
        {
            if (twistOris[i] == twistOris[i + 1] && twistOris[i] == twistOris[i + 2])
            {
                twistOris.RemoveRange(i, 3);
                twistAlgs++;
                i--;
            }
        }
        for (int i = 0; i < twistOris.Count - 1; i++)
        {
            if (twistOris[i] != twistOris[i + 1])
            {
                twistOris.RemoveRange(i, 2);
                twistAlgs++;
                i--;
            }
        }
        if (twistOris.Count > 0)
        {
            twistAlgs++;
        }
        Breaks = OtherCycles.Count(x => x.perm != 1);
        Float1 = OtherCycles.Count(x => x.perm == 1 && x.ori == 0);
        Bad1   = OtherCycles.Count(x => x.perm == 1 && x.ori > 0);
        Float2 = OtherCycles.Count(x => x.perm == 2 && x.ori == 0);
        Bad2   = OtherCycles.Count(x => x.perm == 2 && x.ori > 0);
        Float3 = OtherCycles.Count(x => x.perm == 3 && x.ori == 0);
        Bad3   = OtherCycles.Count(x => x.perm == 3 && x.ori > 0);
        Float4 = OtherCycles.Count(x => x.perm == 4 && x.ori == 0);
        Bad4   = OtherCycles.Count(x => x.perm == 4 && x.ori > 0);
        Float5 = OtherCycles.Count(x => x.perm == 5 && x.ori == 0);
        Bad5   = OtherCycles.Count(x => x.perm == 5 && x.ori > 0);
        Count = FactI[_Perm - 1];
        foreach (var i in OtherCycles)
            Count /= i.perm;
        Count *= Pow3[_Perm - 1 - OtherCycles.Length];
        foreach (var i in OtherCycles.GroupBy(x => x))
            Count /= FactI[i.Count()];
        Algx2 = baseAlgs + (twistAlgs - Float3) * 2;
    }
    public Corner Realize(int Buffer=0)
    {
        CycleConfig[] instance = new CycleConfig[_Perm];
        int head, remain = FirstCycle.perm, current, i = 0, o = 0;
        int[] perm = RandomPermutation(_Perm), ori = RandomOrientation(_Perm, _Ori);
        current = head = Buffer;
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
        return new Corner { state = instance };
    }
    

#region 
    public static readonly List<CornerCC> OddList = [], EvenList = [], AllList = [];
    static CornerCC()
    {
        foreach (var config in GenerateCycleConfigs(8, 3))
        {
            var cc = new CornerCC(config);
            (cc.Parity == 0 ? EvenList : OddList).Add(cc);
        }
        AllList = [.. OddList, .. EvenList];
    }
#endregion

}
