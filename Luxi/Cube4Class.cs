using System;
using System.Linq;
using static Luxi.Tools;

namespace Luxi
{
    public class Cube4Class
    {
        public bool scrambleXCenter;
        public Predicate<CornerCC> cornerLimit;
        public Predicate<WingCC> wingLimit;
        
        private readonly double[] CornerCDF = new double[CornerCC.evenList.Count + CornerCC.oddList.Count],
            WingCDF = new double[WingCC.list.Count];
        public long CornerCount, XCenterCount;
        public Int128 WingCount;
        public double CornerProbability, WingProbability, XCenterProbability;
        public double probability => CornerProbability * WingProbability * XCenterProbability;

        public void Init()
        {
            int index = 0;
            CornerCount = 0;
            foreach (var x in CornerCC.list)
                CornerCDF[index++] = (double)(CornerCount += cornerLimit(x) ? x.Count : 0) / (double)CornerCC.Sum;
            CornerProbability = CornerCDF[CornerCDF.Length - 1];

            WingCount = 0;
            for (index = 0; index < WingCDF.Length; ++index)
            {
                if (wingLimit(WingCC.list[index]))
                {
                    WingCDF[index] = (double)(WingCount += WingCC.list[index].Count) / (double)WingCC.Sum;
                }
            }
            WingProbability = WingCDF[WingCDF.Length - 1];

            XCenterCount = scrambleXCenter ? 1 : XCenterCC.Sum;
            XCenterProbability = (double)XCenterCount / XCenterCC.Sum;

            if (CornerCount == 0 || WingCount == 0)
                throw new Exception("No cube4 can be generated.");
        }
        public Cube4 GetCube4()
        {
            double a = rd.NextDouble() * CornerProbability, b = rd.NextDouble() * WingProbability;
            return new Cube4{
                corner = CornerCC.list[Array.FindIndex(CornerCDF, x => x > a)].GetInstance(),
                wing = WingCC.list[Array.FindIndex(WingCDF, x => x > b)].GetInstance(),
                xcenter = XCenterCC.GetInstance(scrambleXCenter)
            };
        }
    }
}