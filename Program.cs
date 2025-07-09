
using Cube;

class Program
{
    static void Main(string[] args)
    {
        // var baseAlg = new Alg("Rw2B2Rw'U2Rw'U2B2Rw'B2RwB2Rw'B2Rw2B2U2");
        // foreach(var alg in baseAlg.Shifts().Skip(1)){
        //     var cube = new Cube4();
        //     cube.Turn(alg.GetInv());
        //     string code = cube.ReadCode();
        //     if (code.Length > 2) continue;
        //     Console.WriteLine($"{code}={alg}");
        //     Console.WriteLine($"{code}={alg.GetInv()}");
        // }

        // var baseAlg = new Alg("F R U R' U' R' F' R U' R' D' R U2 R' D R2 U' R'");
        // foreach(var alg in baseAlg.Shifts()){
        //     var cube = new Cube3();
        //     cube.Turn(alg.GetInv());
        //     string code = cube.ReadCode();
        //     if (code.Length > 4) continue;
        //     Console.WriteLine($"{code}={alg}");
        //     // Inverse
        //     cube = new Cube3();
        //     cube.Turn(alg);
        //     code = cube.ReadCode();
        //     Console.WriteLine($"{code}={alg.GetInv()}");
        // }

        // Dictionary<int,Int128> stat = [];
        // foreach (var w in WingCC.all)
        // {
        //     int key = w.OtherCycles.Count(x => x == 2);
        //     stat[key] = stat.GetValueOrDefault(key, 0) + w.Count;
        // }
        // foreach (var i in stat.Keys.OrderBy(x => x))
        // {
        //     double prob = (double)stat[i] / (double)WingCC.Sum;
        //     if(prob > 0.001)
        //         Console.WriteLine($"#{i}: {prob:F4}");
        // }

        // double[] es = new double[33], cs = new double[21];
        // foreach (var e in EdgeCC.AllList){
        //     int key = e.Algx2;
        //     es[key] += (double)e.Count / (double)EdgeCC.Sum;
        // }
        // foreach (var c in CornerCC.AllList){
        //     int key = c.Algx2;
        //     cs[key] += (double)c.Count / (double)CornerCC.Sum;
        // }
        
        // double[] total = new double[(es.Length + cs.Length)>>1];
        // for (int eIdx = 0; eIdx < es.Length; eIdx++)
        //     for (int cIdx = 0; cIdx < cs.Length; cIdx++)
        //         if (eIdx % 2 == cIdx % 2)
        //             total[(eIdx + cIdx) >> 1] += es[eIdx] * cs[cIdx] * 2.0;

        // double sum = 0.0, xSum = 0.0, x2Sum = 0.0;
        // for (int i = 0; i < total.Length; i++){
        //     sum += total[i];
        //     xSum += i * total[i];
        //     x2Sum += i * i * total[i];
        //     if (total[i] > 0.001)
        //         Console.WriteLine($"#{i}: {total[i]:F4}");
        // }
        // Console.WriteLine($"Total: {sum:F4} Mean: {xSum:F4} StdDev: {Math.Sqrt(x2Sum - xSum * xSum):F4}");
    }
}
