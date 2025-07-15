using System.Diagnostics;
using static Cube.Move;
using static Cube.Tools;

namespace Cube;
public class Edge
{
    public CycleConfig[] state;

    public Edge() => state = [.. Enumerable.Range(0, 12).Select(i => new CycleConfig{ perm = i, ori = 0 })];
    public CycleConfig this[int perm, int ori] => new() { perm = state[perm].perm, ori = state[perm].ori ^ ori };
    public CycleConfig this[CycleConfig s] => new() { perm = state[s.perm].perm, ori = state[s.perm].ori ^ s.ori };
    public static Edge Random()
    {
        int[] perm = RandomPermutation(12), ori = RandomOrientation(12, 2);
        return new Edge { state = [.. Enumerable.Range(0, 12).Select(i => new CycleConfig { perm = perm[i], ori = ori[i] })] };
    }
    public Edge Copy() => new() { state = [.. state] };
    public void Solve(){
        for (int i = 0; i < 12; i++){
            state[i].perm = i;
            state[i].ori = 0;
        }
    }
    public bool IsSolved(){
        for (int i = 0; i < 12; i++){
            if (state[i].perm != i || state[i].ori != 0)
                return false;
        }
        return true;
    }
    public int GetParity(){
        return Tools.GetParity([.. state.Select(s => s.perm)]);
    }
    
    public void Cycle4(int p0, int p1, int p2, int p3, int o)
    {
        int t = state[p0].perm;
        state[p0].perm = state[p3].perm;
        state[p3].perm = state[p2].perm;
        state[p2].perm = state[p1].perm;
        state[p1].perm = t;

        t = state[p0].ori;
        state[p0].ori = state[p3].ori ^ o;
        state[p3].ori = state[p2].ori ^ o;
        state[p2].ori = state[p1].ori ^ o;
        state[p1].ori = t ^ o;
    }
    public void Swap(int p0, int p1, int o = 0)
    {
        (state[p1].perm, state[p0].perm) = (state[p0].perm, state[p1].perm);
        int t = state[p0].ori;
        state[p0].ori = state[p1].ori ^ o;
        state[p1].ori = t ^ o;
    }
    public void Turn(Move m)
    {
        switch (m)
        {
            case U: Cycle4(0, 1, 2, 3, 0); break;
            case U2: Swap(0, 2); Swap(1, 3); break;
            case U_: Cycle4(0, 3, 2, 1, 0); break;
            case D: Cycle4(4, 7, 6, 5, 0); break;
            case D2: Swap(4, 6); Swap(7, 5); break;
            case D_: Cycle4(4, 5, 6, 7, 0); break;
            case R: Cycle4(3, 11, 7, 8, 0); break;
            case R2: Swap(3, 7); Swap(11, 8); break;
            case R_: Cycle4(3, 8, 7, 11, 0); break;
            case L: Cycle4(1, 9, 5, 10, 0); break;
            case L2: Swap(1, 5); Swap(9, 10); break;
            case L_: Cycle4(1, 10, 5, 9, 0); break;
            case F: Cycle4(0, 8, 4, 9, 1); break;
            case F2: Swap(0, 4); Swap(8, 9); break;
            case F_: Cycle4(0, 9, 4, 8, 1); break;
            case B: Cycle4(2, 10, 6, 11, 1); break;
            case B2: Swap(2, 6); Swap(10, 11); break;
            case B_: Cycle4(2, 11, 6, 10, 1); break;
            case E: Cycle4(8, 11, 10, 9, 1); break;
            case E2: Swap(8, 10); Swap(11, 9); break;
            case E_: Cycle4(8, 9, 10, 11, 1); break;
            case M: Cycle4(0, 4, 6, 2, 1); break;
            case M2: Swap(0, 6); Swap(4, 2); break;
            case M_: Cycle4(0, 2, 6, 4, 1); break;
            case S: Cycle4(1, 3, 7, 5, 1); break;
            case S2: Swap(1, 7); Swap(5, 3); break;
            case S_: Cycle4(1, 5, 7, 3, 1); break;
            case x: Turn(R); Turn(L_); Turn(M_); break;
            case x2: Turn(R2); Turn(L2); Turn(M2); break;
            case x_: Turn(R_); Turn(L); Turn(M); break;
            case y: Turn(U); Turn(D_); Turn(E_); break;
            case y2: Turn(U2); Turn(D2); Turn(E2); break;
            case y_: Turn(U_); Turn(D); Turn(E); break;
            case z: Turn(F); Turn(B_); Turn(S); break;
            case z2: Turn(F2); Turn(B2); Turn(S2); break;
            case z_: Turn(F_); Turn(B); Turn(S_); break;
            default: break;
        }
    }

    public const string Code = "ABCDEFGHIJKLMNOPQRSTWXYZ";
    public void Cycle(string code, int Buffer=0){
        foreach (char c in code)
        {
            int idx = Code.IndexOf(c);
            if (idx >= 0 && idx / 2 != Buffer){
                Swap(Buffer, idx / 2, idx % 2);
            }
        }
    }
    public string ReadCode(int Buffer=0, bool flipSign=true, bool preserveOri=true){
        System.Text.StringBuilder sb = new();
        int remainBlocks = 0xfff;
        remainBlocks &= ~(1 << Buffer);
        // Remove solved blocks
        for (int i = 0; i < 12; i++)
            if (state[i].perm == i && state[i].ori == 0)
                remainBlocks &= ~(1 << i);
        CycleConfig head = (Buffer, 0);
        while(true){
            var next = this[head];
            // Append cycle middle to code
            while (next.perm != head.perm){
                sb.Append(Code[next.perm * 2 + next.ori]);
                remainBlocks &= ~(1 << next.perm);
                next = this[next];
            }
            // Append cycle end to code
            if (next.perm != Buffer){
                sb.Append(flipSign && state[next.perm].perm == next.perm ? '+' : Code[next.perm * 2 + next.ori]);
            }
            if(remainBlocks == 0)
                break;
            // Find the next unsolved block
            head.perm = 0;
            head.ori = preserveOri ? next.ori : 0;
            while(((remainBlocks >> head.perm) & 1) == 0)
                ++head.perm;
            remainBlocks &= ~(1 << head.perm);
            // Append cycle head to code
            sb.Append(Code[head.perm * 2 + head.ori]);
        }
        return sb.ToString();
    }
}
