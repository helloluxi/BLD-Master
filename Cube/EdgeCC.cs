using static Cube.Tools;

namespace Cube;
public class EdgeCC
{
    public readonly CycleConfig FirstCycle;
    public readonly CycleConfig[] OtherCycles;
    private const int _Perm = 12, _Ori = 2;
    public const long Sum = 980995276800;
    public readonly int Algx2, Breaks, Parity, Closed1, Open1, Closed2, Open2, Closed3, Open3;
    public readonly int AlgFFx2, AlgFFPx2;
    public readonly long Count;

    public EdgeCC(CycleConfig[] cycles)
    {
        FirstCycle = (cycles[0].perm, cycles[0].ori);
        OtherCycles = [.. cycles.Skip(1)];

        int baseAlgs = OtherCycles.Sum(x => x.perm > 1 ? x.perm + 1 : 0) + FirstCycle.perm - 1;
        int twistAlgs = (OtherCycles.Count(x => x.perm == 1 && x.ori != 0) + 3) >> 2;
        Parity = baseAlgs & 1;
        Breaks = OtherCycles.Count(x => x.perm != 1);
        Closed1 = OtherCycles.Count(x => x.perm == 1 && x.ori == 0);
        Open1 = OtherCycles.Count(x => x.perm == 1 && x.ori > 0);
        Closed2 = OtherCycles.Count(x => x.perm == 2 && x.ori == 0);
        Open2 = OtherCycles.Count(x => x.perm == 2 && x.ori > 0);
        Closed3 = OtherCycles.Count(x => x.perm == 3 && x.ori == 0);
        Open3 = OtherCycles.Count(x => x.perm == 3 && x.ori > 0);
        Count = FactI64[_Perm - 1];
        foreach (var i in OtherCycles)
            Count /= i.perm;
        Count *= 1L << (_Perm - 1 - OtherCycles.Length);
        foreach (var i in OtherCycles.GroupBy(x => x))
            Count /= FactI64[i.Count()];
        Algx2 = baseAlgs + (twistAlgs - Closed3) * 2;

        int p0 = FirstCycle.perm;
        int algFF = 0, r20 = 0, r21 = 0, r11 = 0;
        foreach (var cycle in OtherCycles)
        {
            int p = cycle.perm, o = cycle.ori;
            if (p >= 3)
            {
                algFF += (p - 1) / 2;
                p = p % 2 == 0 ? 2 : 1;
            }
            if (p == 2)
            {
                if (o == 0) r20++;
                else r21++;
            }
            else if (p == 1 && o == 1) r11++;
        }
        algFF += r20 >> 1 << 1; r20 %= 2;
        algFF += r21 >> 1 << 1; r21 %= 2;
        int mp = Math.Min(r20, r21); algFF += mp * 2; r20 -= mp; r21 -= mp; r11 += mp;
        algFF += (r11 + 3) / 4;
        AlgFFx2 = 2 * algFF + 3 * (r20 + r21) + (p0 - 1);
        AlgFFPx2 = Parity == 1
            ? 2 * algFF + r20 + 3 * r21 + (p0 - 1)
            : AlgFFx2;
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
