using System;
using System.Numerics;
using static Luxi.Tools;

namespace Luxi
{
    class Cube3Class
    {
        public Predicate<bool> parityLimit;
        public Predicate<EdgeCC> edgeLimit;
        public Predicate<CornerCC> cornerLimit;
        private readonly double[] evenEdgeCDF = new double[EdgeCC.evenList.Count], oddEdgeCDF = new double[EdgeCC.oddList.Count],
            evenCornerCDF = new double[CornerCC.evenList.Count], oddCornerCDF = new double[CornerCC.oddList.Count];
        public double evenEdgeProbability, oddEdgeProbability, evenCornerProbability, oddCornerProbability;
        public long evenEdgeCount, oddEdgeCount, evenCornerCount, oddCornerCount;
        public double meanParity, probability;
        
        public void Init()
        {
            int index = 0;
            evenEdgeCount = 0;
            foreach (var x in EdgeCC.evenList)
                evenEdgeCDF[index++] = (evenEdgeCount += parityLimit(false) && edgeLimit(x) ? x.Count : 0) * 2.0 / EdgeCC.Sum;
            evenEdgeProbability = evenEdgeCDF[evenEdgeCDF.Length - 1];

            oddEdgeCount = index = 0;
            foreach (var x in EdgeCC.oddList)
                oddEdgeCDF[index++] = (oddEdgeCount += parityLimit(true) && edgeLimit(x) ? x.Count : 0) * 2.0 / EdgeCC.Sum;
            oddEdgeProbability = oddEdgeCDF[oddEdgeCDF.Length - 1];

            evenCornerCount = index = 0;
            foreach (var x in CornerCC.evenList)
                evenCornerCDF[index++] = (evenCornerCount += parityLimit(false) && cornerLimit(x) ? x.Count : 0) * 2.0 / CornerCC.Sum;
            evenCornerProbability = evenCornerCDF[evenCornerCDF.Length - 1];

            oddCornerCount = index = 0;
            foreach (var x in CornerCC.oddList)
                oddCornerCDF[index++] = (oddCornerCount += parityLimit(true) && cornerLimit(x) ? x.Count : 0) * 2.0 / CornerCC.Sum;
            oddCornerProbability = oddCornerCDF[oddCornerCDF.Length - 1];

            meanParity = oddEdgeProbability * oddCornerProbability / (evenEdgeProbability * evenCornerProbability + oddEdgeProbability * oddCornerProbability);
            probability = (evenEdgeProbability * evenCornerProbability + oddEdgeProbability * oddCornerProbability) * 2.0;

            if((evenEdgeCount == 0 || evenCornerCount == 0) && (oddEdgeCount == 0 || oddCornerCount == 0))
                throw new Exception("No cube3 can be generated.");
        }
        public Cube3 GetCube3()
        {
            if (rd.NextDouble() < meanParity)
            {
                double a = rd.NextDouble() * oddEdgeProbability, b = rd.NextDouble() * oddCornerProbability;
                return new Cube3{
                    edge = EdgeCC.oddList[Array.FindIndex(oddEdgeCDF, x => x > a)].GetInstance(),
                    corner = CornerCC.oddList[Array.FindIndex(oddCornerCDF, x => x > b)].GetInstance()
                };
            }
            else
            {
                double a = rd.NextDouble() * evenEdgeProbability, b = rd.NextDouble() * evenCornerProbability;
                return new Cube3{
                    edge = EdgeCC.evenList[Array.FindIndex(evenEdgeCDF, x => x > a)].GetInstance(),
                    corner = CornerCC.evenList[Array.FindIndex(evenCornerCDF, x => x > b)].GetInstance()
                };
            }
        }
    }
}