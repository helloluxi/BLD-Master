using static Cube.Tools;
namespace Cube;

public class WingCC
{
    public readonly int FirstCycle;
    public readonly int[] OtherCycles;
    public const int _Perm = 24;
    public readonly int CodeLength, Algx2, Cycles, Parity, Float1, Float2, Float3, Float4, Float5;
    public readonly Int128 Count;
    public WingCC(int FirstCycle, int[] OtherCycles)
    {
        this.FirstCycle = FirstCycle;
        this.OtherCycles = OtherCycles;
        CodeLength = OtherCycles.Sum(x => x > 1 ? x + 1 : 0) + FirstCycle - 1;
        Parity = CodeLength & 1;
        Cycles = OtherCycles.Count(x => x != 1);
        Float1 = OtherCycles.Count(x => x == 1);
        Float2 = OtherCycles.Count(x => x == 2);
        Float3 = OtherCycles.Count(x => x == 3);
        Float4 = OtherCycles.Count(x => x == 4);
        Float5 = OtherCycles.Count(x => x == 5);
        Count = FactI128[_Perm - 1];
        foreach (var i in OtherCycles)
            Count /= i;
        foreach (var i in OtherCycles.GroupBy(x => x))
            Count /= FactI128[i.Count()];
        Algx2 = CodeLength - Float3 * 2;
    }
    public Wing GetInstance(int Buffer=0)
    {
        int[] instance = new int[_Perm], order = Wing.Random().state;
        int head, remain = FirstCycle, current, i = 0;
        current = head = Buffer;
        order[Array.IndexOf(order, head)] = order[0];
        order[0] = head;
        while ((--remain) > 0)
        {
            i++;
            instance[current] = order[i];
            current = order[i];
        }
        instance[current] = head;
        foreach (var cycle in OtherCycles)
        {
            remain = cycle;
            current = head = order[++i];
            while ((--remain) > 0)
            {
                i++;
                instance[current] = order[i];
                current = order[i];
            }
            instance[current] = head;
        }
        return new Wing { state = instance };
    }



    #region 
    public static readonly List<WingCC> all;
    public static readonly Int128 Sum = Int128.Parse("620448401733239439360000");
    static WingCC()
    {
        all = [];
        // if (File.Exists("Cache/w.txt")){
        //     all.AddRange(File.ReadAllLines("Cache/w.txt").Select(x => x.Split(',')).Select(x => new WingCC(int.Parse(x[0]), x.Skip(1).Select(int.Parse).ToArray())));
        // }
        // else{
            foreach (int[] s in GeneratePerm(24))
                all.Add(new WingCC(24 - s.Sum(), s.Clone() as int[]));
            // Directory.CreateDirectory("Cache");
            // File.WriteAllLines("Cache/w.txt",
            //     all.Select(x => string.Join(",", Enumerable.Concat(
            //         [x.FirstCycle],
            //         x.OtherCycles
            //     ))));
        // }
    }
    #endregion
}
