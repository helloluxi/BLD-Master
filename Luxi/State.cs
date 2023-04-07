namespace Luxi{
    public struct State{
        public int perm, ori;
        public static implicit operator State((int, int) state) => new State{ perm=state.Item1, ori=state.Item2 };
    }
}