using System;
using System.Diagnostics;
using System.Threading;
using Luxi;

class Program
{
    static void Main(string[] args)
    {
        var cc = new Cube3Class{
            parityLimit = x => true,
            edgeLimit = x => x.CodeLength is >=10 and < 12 && x.FlipCount == 0,
            cornerLimit = x => x.CodeLength is >=6 and < 8 && x.TwistCount == 0,
        };
        cc.Init();
        Console.WriteLine($"Probability={cc.probability}");
    
        for (int i = 0; i < 10; i++)
        {
            var cube = cc.GetCube3();
            System.Console.Write($"{i+1}. ");
            Console.WriteLine(cube.GetScramble());
        }

        // var cc = new Cube4Class{
        //     cornerLimit = x => x.CodeLength == 6,
        //     wingLimit = x => x.CodeLength == 20,
        //     scrambleXCenter = false
        // };
        // cc.Init();
        // Console.WriteLine($"Probability={cc.probability}");

        // var cube = cc.GetCube4();
        // Console.WriteLine(cube.GetScramble());
    }
}
