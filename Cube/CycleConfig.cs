namespace Cube;
public struct CycleConfig {
    public int perm, ori;
    public static implicit operator CycleConfig((int, int) state) => new() { perm=state.Item1, ori=state.Item2 };
    public override readonly string ToString() => $"{perm},{ori}";
}
