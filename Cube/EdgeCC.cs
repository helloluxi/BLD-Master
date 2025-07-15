using static Cube.Tools;

namespace Cube;
public class EdgeCC
{
    public readonly CycleConfig FirstCycle;
    public readonly CycleConfig[] OtherCycles;
    private const int _Perm = 12, _Ori = 2;
    public const long Sum = 980995276800;
    public readonly int Algx2, Breaks, Parity, Float1, Bad1, Float2, Bad2, Float3, Bad3, Float4, Bad4, Float5, Bad5;
    public readonly long Count;

    public EdgeCC(CycleConfig[] cycles)
    {
        FirstCycle = (cycles[0].perm, cycles[0].ori);
        OtherCycles = [.. cycles.Skip(1)];

        int baseAlgs = OtherCycles.Sum(x => x.perm > 1 ? x.perm + 1 : 0) + FirstCycle.perm - 1;
        int twistAlgs = (OtherCycles.Count(x => x.perm == 1 && x.ori != 0) + 3) >> 2;
        Parity = baseAlgs & 1;
        Breaks = OtherCycles.Count(x => x.perm != 1);
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
    public Edge Realize(int Buffer=0)
    {
        CycleConfig[] instance = new CycleConfig[_Perm];
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
    public static readonly List<EdgeCC> OddList = [], EvenList = [];
    public static IEnumerable<EdgeCC> AllList => OddList.Concat(EvenList);
    static EdgeCC()
    {
        foreach (var config in GenerateCycleConfigs(12, 2))
        {
            var cc = new EdgeCC(config);
            (cc.Parity == 0 ? EvenList : OddList).Add(cc);
        }
    }
#endregion

}
