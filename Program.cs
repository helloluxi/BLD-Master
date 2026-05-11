using Cube;

int size = 3;
List<string> positional = [];
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "-s" && i + 1 < args.Length)
    {
        if (!int.TryParse(args[++i], out size) || size < 3 || size > 5)
        {
            Console.Error.WriteLine("error: -s expects 3, 4, or 5");
            return 1;
        }
    }
    else positional.Add(args[i]);
}

if (positional.Count == 0 || positional[0] != "alg2code")
{
    Console.Error.WriteLine("Usage: bld [-s 3|4|5] alg2code <alg_string>");
    Console.Error.WriteLine("       bld [-s 3|4|5] alg2code            (one alg per line from stdin, blank line to finish)");
    return 1;
}

if (positional.Count >= 2)
{
    PrintCode(string.Join(' ', positional.Skip(1)), size);
}
else
{
    List<string> algs = [];
    string? line;
    while ((line = Console.In.ReadLine()) != null)
    {
        if (string.IsNullOrWhiteSpace(line)) break;
        algs.Add(line);
    }
    foreach (var a in algs) PrintCode(a, size);
}
return 0;

static void PrintCode(string algStr, int size)
{
    var alg = new Alg(algStr);
    string code;
    switch (size)
    {
        case 3:
            {
                var cube = new Cube3();
                cube.Turn(alg.GetInv());
                code = cube.ReadCode();
                break;
            }
        case 4:
            {
                var cube = new Cube4();
                cube.Turn(alg.GetInv());
                code = cube.ReadCode();
                break;
            }
        default:
            Console.Error.WriteLine($"error: -s {size} not implemented yet");
            Environment.Exit(1);
            return;
    }
    Console.WriteLine($"{code} = {alg}");
}
