using static Cube.Tools;

namespace Cube;
class Cube3Class
{
    public Predicate<int> parityConstraint;
    public Predicate<EdgeCC> edgeConstraint;
    public Predicate<CornerCC> cornerConstraint;
    private readonly double[] evenEdgeCDF = new double[EdgeCC.EvenList.Count], oddEdgeCDF = new double[EdgeCC.OddList.Count],
        evenCornerCDF = new double[CornerCC.EvenList.Count], oddCornerCDF = new double[CornerCC.OddList.Count];
    public double evenEdgeProbability, oddEdgeProbability, evenCornerProbability, oddCornerProbability;
    public long evenEdgeCount, oddEdgeCount, evenCornerCount, oddCornerCount;
    public double meanParity, probability;
    
    public Cube3Class(){
        parityConstraint = x => true;
        edgeConstraint = x => true;
        cornerConstraint = x => true;
    }
    public void Init()
    {
        int index = 0;
        evenEdgeCount = 0;
        foreach (var x in EdgeCC.EvenList)
            evenEdgeCDF[index++] = (evenEdgeCount += parityConstraint(0) && edgeConstraint(x) ? x.Count : 0) * 2.0 / EdgeCC.Sum;
        evenEdgeProbability = evenEdgeCDF[^1];

        oddEdgeCount = index = 0;
        foreach (var x in EdgeCC.OddList)
            oddEdgeCDF[index++] = (oddEdgeCount += parityConstraint(1) && edgeConstraint(x) ? x.Count : 0) * 2.0 / EdgeCC.Sum;
        oddEdgeProbability = oddEdgeCDF[^1];

        evenCornerCount = index = 0;
        foreach (var x in CornerCC.EvenList)
            evenCornerCDF[index++] = (evenCornerCount += parityConstraint(0) && cornerConstraint(x) ? x.Count : 0) * 2.0 / CornerCC.Sum;
        evenCornerProbability = evenCornerCDF[^1];

        oddCornerCount = index = 0;
        foreach (var x in CornerCC.OddList)
            oddCornerCDF[index++] = (oddCornerCount += parityConstraint(1) && cornerConstraint(x) ? x.Count : 0) * 2.0 / CornerCC.Sum;
        oddCornerProbability = oddCornerCDF[^1];

        meanParity = oddEdgeProbability * oddCornerProbability / (evenEdgeProbability * evenCornerProbability + oddEdgeProbability * oddCornerProbability);
        probability = (evenEdgeProbability * evenCornerProbability + oddEdgeProbability * oddCornerProbability) * 0.5;

        if((evenEdgeCount == 0 || evenCornerCount == 0) && (oddEdgeCount == 0 || oddCornerCount == 0))
            throw new Exception("No cube3 can be generated.");
    }
    public Cube3 GetCube3()
    {
        if (rd.NextDouble() < meanParity)
        {
            double a = rd.NextDouble() * oddEdgeProbability, b = rd.NextDouble() * oddCornerProbability;
            return new Cube3{
                edge = EdgeCC.OddList[Array.FindIndex(oddEdgeCDF, x => x > a)].GetInstance(),
                corner = CornerCC.OddList[Array.FindIndex(oddCornerCDF, x => x > b)].GetInstance()
            };
        }
        else
        {
            double a = rd.NextDouble() * evenEdgeProbability, b = rd.NextDouble() * evenCornerProbability;
            return new Cube3{
                edge = EdgeCC.EvenList[Array.FindIndex(evenEdgeCDF, x => x > a)].GetInstance(),
                corner = CornerCC.EvenList[Array.FindIndex(evenCornerCDF, x => x > b)].GetInstance()
            };
        }
    }
}
