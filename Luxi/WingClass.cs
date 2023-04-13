using System;
using System.Collections.Generic;
using static Luxi.Tools;

namespace Luxi
{
    public class WingCC
    {
        public readonly int FirstCycle;
        public readonly int[] OtherCycles;
        public const int Perm = 24;
        public readonly int CodeLength, OtherCycleAmount, Parity;
        public readonly Int128 Count;
        public WingCC(int FirstCycle, int[] OtherCycles)
        {
            this.FirstCycle = FirstCycle;
            this.OtherCycles = OtherCycles;
            CodeLength = OtherCycles.Sum(x => x > 1 ? x + 1 : 0) + FirstCycle - 1;
            Parity = CodeLength & 1;
            OtherCycleAmount = OtherCycles.Count(x => x != 1);
            Count = FactI128(Perm - 1);
            foreach (var i in OtherCycles)
                Count /= i;
            foreach (var i in OtherCycles.GroupBy(x => x))
                Count /= FactI128(i.Count());
        }
        public Wing GetInstance(int Buffer=0)
        {
            int[] instance = new int[Perm], order = Wing.Random().state;
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
        public static readonly List<WingCC> list;
        public static readonly Int128 Sum = Int128.Parse("620448401733239439360000");
        static WingCC()
        {
            list = new List<WingCC>();
            foreach (int[] s in SizeGenerater(24))
                list.Add(new WingCC(24 - s.Sum(), s.Clone() as int[]));
        }
        #endregion
    }
}