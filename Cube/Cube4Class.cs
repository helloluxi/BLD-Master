using static Cube.Tools;

namespace Cube;
public class Cube4Class
{
    public bool scrambleXCenter;
    public Predicate<CornerCC> cornerConstranit;
    public Predicate<WingCC> wingConstraint;
    
    private readonly double[] CornerCDF = new double[CornerCC.EvenList.Count + CornerCC.OddList.Count],
        WingCDF = new double[WingCC.AllList.Count];
    public long CornerCount, XCenterCount;
    public Int128 WingCount;
    public double CornerProbability, WingProbability, XCenterProbability;
    public double probability => CornerProbability * WingProbability * XCenterProbability;

    public Cube4Class(){
        scrambleXCenter = true;
        cornerConstranit = x => true;
        wingConstraint = x => true;
    }
    public void Init()
    {
        int index = 0;
        CornerCount = 0;
        foreach (var x in CornerCC.AllList)
            CornerCDF[index++] = (double)(CornerCount += cornerConstranit(x) ? x.Count : 0) / (double)CornerCC.Sum;
        CornerProbability = CornerCDF[^1];

        WingCount = 0;
        for (index = 0; index < WingCDF.Length; ++index)
        {
            if (wingConstraint(WingCC.AllList[index]))
            {
                WingCDF[index] = (double)(WingCount += WingCC.AllList[index].Count) / (double)WingCC.Sum;
            }
        }
        WingProbability = WingCDF[^1];

        XCenterCount = scrambleXCenter ? 1 : XCenterCC.Sum;
        XCenterProbability = (double)XCenterCount / XCenterCC.Sum;

        if (CornerCount == 0 || WingCount == 0)
            throw new Exception("No cube4 can be generated.");
    }
    public Cube4 GetCube4()
    {
        double a = rd.NextDouble() * CornerProbability, b = rd.NextDouble() * WingProbability;
        return new Cube4{
            corner = CornerCC.AllList[Array.FindIndex(CornerCDF, x => x > a)].Realize(),
            wing = WingCC.AllList[Array.FindIndex(WingCDF, x => x > b)].GetInstance(),
            xcenter = XCenterCC.GetInstance(scrambleXCenter)
        };
    }
}
