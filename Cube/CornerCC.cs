using static Cube.Tools;
namespace Cube;
public class CornerCC {
    public readonly CycleConfig FirstCycle;
    public readonly CycleConfig[] OtherCycles;
    private const int _Perm = 8, _Ori = 3;
    public const long Sum = 88179840;
    public readonly int Algx2, Breaks, Parity, Closed1, Open1, CwTwist, CcwTwist, Closed2, Open2, Closed3, Open3;
    public readonly int AlgFFx2, AlgFFPx2;
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
        Closed1  = OtherCycles.Count(x => x.perm == 1 && x.ori == 0);
        Open1    = OtherCycles.Count(x => x.perm == 1 && x.ori > 0);
        CwTwist  = OtherCycles.Count(x => x.perm == 1 && x.ori == 1);
        CcwTwist = OtherCycles.Count(x => x.perm == 1 && x.ori == 2);
        Closed2 = OtherCycles.Count(x => x.perm == 2 && x.ori == 0);
        Open2   = OtherCycles.Count(x => x.perm == 2 && x.ori > 0);
        Closed3 = OtherCycles.Count(x => x.perm == 3 && x.ori == 0);
        Open3   = OtherCycles.Count(x => x.perm == 3 && x.ori > 0);
        Count = FactI[_Perm - 1];
        foreach (var i in OtherCycles)
            Count /= i.perm;
        Count *= Pow3[_Perm - 1 - OtherCycles.Length];
        foreach (var i in OtherCycles.GroupBy(x => x))
            Count /= FactI[i.Count()];
        Algx2 = baseAlgs + (twistAlgs - Closed3) * 2;

        int p0 = FirstCycle.perm;
        int algFF = 0, r20 = 0, r21 = 0, r22 = 0, r11 = 0, r12 = 0;
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
                else if (o == 1) r21++;
                else r22++;
            }
            else if (p == 1)
            {
                if (o == 1) r11++;
                else if (o == 2) r12++;
            }
        }
        algFF += r20 >> 1 << 1; r20 %= 2;
        int mp = Math.Min(r21, r22); algFF += mp * 2; r21 -= mp; r22 -= mp;
        algFF += r21 >> 1 << 1; r12 += r21 >> 1; r21 %= 2;
        algFF += r22 >> 1 << 1; r12 += r22 >> 1; r22 %= 2;
        int mp1 = Math.Min(r20, r21); algFF += mp1 * 2; r20 -= mp1; r21 -= mp1; r11 += mp1;
        int mp2 = Math.Min(r20, r22); algFF += mp2 * 2; r20 -= mp2; r22 -= mp2; r12 += mp2;
        algFF += r11 / 3; r11 %= 3;
        algFF += r12 / 3; r12 %= 3;
        algFF += (r11 + r12 + 2) / 3;
        AlgFFx2 = 2 * algFF + 3 * (r20 + r21 + r22) + (p0 - 1);
        AlgFFPx2 = Parity == 1
            ? 2 * algFF + r20 + 3 * (r21 + r22) + (p0 - 1)
            : AlgFFx2;
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
