namespace CS;

public static class Global
{
    private static Min2Phase.Search _search3;
    private static ThreePhase.Search _search4;
    private static Cube555.Search _search5;
    public static Min2Phase.Search Search3 => _search3 ??= new Min2Phase.Search();
    public static ThreePhase.Search Search4 => _search4 ??= new ThreePhase.Search();
    public static Cube555.Search Search5 => _search5 ??= new Cube555.Search();
    public static string GetScramble3(string cube) => Search3.Solution(cube, 21, 100000000, 0, 2);
    public static string GetScramble4(string cube) => Search4.Solution(cube);
    public static string GetScramble5(string cube) => Search5.SolveReduction(cube, 2);
}