using static Cube.Move;
using static Cube.Tools;
namespace Cube;
public class Wing
{
    public int[] state = new int[24];
    public Wing() => Solve();
    public int this[int index] => state[index];
    public void Solve(){
        for(int i = 0; i < 24; i++)
            state[i] = i;
    }
    public bool IsSolved()
    {
        for (int i = 0; i < 24; i++)
            if (state[i] != i)
                return false;
        return true;
    }
    public void Cycle(int p0, int p1, int p2, int p3)
    {
        int t = state[p0];
        state[p0] = state[p3];
        state[p3] = state[p2];
        state[p2] = state[p1];
        state[p1] = t;
    }
    public void Cycle(string bfCode){
        foreach (char c in bfCode)
        {
            int idx = Edge.Code.IndexOf(c);
            if (idx > 0){
                Swap(0, idx);
            }
        }
    }
    public void Swap(int p0, int p1) => (state[p1], state[p0]) = (state[p0], state[p1]);
    public static Wing Random()
    {
        int[] arr = new int[24];
        for (int i = 22; i >= 0; i--)
        {
            arr[i] = rd.Next(24 - i);
            for (int j = i + 1; j < 24; j++)
            {
                if (arr[j] >= arr[i])
                    arr[j]++;
            }
        }
        return new Wing { state = arr };
    }
    public void Turn(Move m)
    {
        switch (m)
        {
            case U: Cycle(0, 2, 4, 6); Cycle(1, 3, 5, 7); break;
            case U2: Swap(0, 4); Swap(2, 6); Swap(1, 5); Swap(3, 7); break;
            case U_: Cycle(0, 6, 4, 2); Cycle(1, 7, 5, 3); break;
            case u: Cycle(16, 19, 20, 23); break;
            case u2: Swap(16, 20); Swap(19, 23); break;
            case u_: Cycle(16, 23, 20, 19); break;
            case D: Cycle(8, 14, 12, 10); Cycle(9, 15, 13, 11); break;
            case D2: Swap(8, 12); Swap(10, 14); Swap(9, 13); Swap(15, 11); break;
            case D_: Cycle(8, 10, 12, 14); Cycle(9, 11, 13, 15); break;
            case d: Cycle(17, 22, 21, 18); break;
            case d2: Swap(17, 21); Swap(18, 22); break;
            case d_: Cycle(17, 18, 21, 22); break;
            case R: Cycle(6, 22, 14, 16); Cycle(7, 23, 15, 17); break;
            case R2: Swap(6, 14); Swap(22, 16); Swap(7, 15); Swap(23, 17); break;
            case R_: Cycle(6, 16, 14, 22); Cycle(7, 17, 15, 23); break;
            case r: Cycle(0, 5, 12, 9); break;
            case r2: Swap(0, 12); Swap(5, 9); break;
            case r_: Cycle(0, 9, 12, 5); break;
            case L: Cycle(2, 18, 10, 20); Cycle(3, 19, 11, 21); break;
            case L2: Swap(2, 10); Swap(18, 20); Swap(3, 11); Swap(19, 21); break;
            case L_: Cycle(2, 20, 10, 18); Cycle(3, 21, 11, 19); break;
            case l: Cycle(1, 8, 13, 4); break;
            case l2: Swap(1, 13); Swap(8, 4); break;
            case l_: Cycle(1, 4, 13, 8); break;
            case F: Cycle(0, 17, 8, 19); Cycle(1, 16, 9, 18); break;
            case F2: Swap(0, 8); Swap(17, 19); Swap(1, 9); Swap(16, 18); break;
            case F_: Cycle(0, 19, 8, 17); Cycle(1, 18, 9, 16); break;
            case f: Cycle(2, 7, 14, 11); break;
            case f2: Swap(2, 14); Swap(7, 11); break;
            case f_: Cycle(2, 11, 14, 7); break;
            case B: Cycle(4, 21, 12, 23); Cycle(5, 20, 13, 22); break;
            case B2: Swap(4, 12); Swap(21, 23); Swap(5, 13); Swap(20, 22); break;
            case B_: Cycle(4, 23, 12, 21); Cycle(5, 22, 13, 20); break;
            case b: Cycle(3, 10, 15, 6); break;
            case b2: Swap(3, 15); Swap(10, 6); break;
            case b_: Cycle(3, 6, 15, 10); break;
            case x: Turn(R); Turn(r); Turn(l_); Turn(L_); break;
            case x2: Turn(R2); Turn(r2); Turn(l2); Turn(L2); break;
            case x_: Turn(R_); Turn(r_); Turn(l); Turn(L); break;
            case y: Turn(U); Turn(u); Turn(d_); Turn(D_); break;
            case y2: Turn(U2); Turn(u2); Turn(d2); Turn(D2); break;
            case y_: Turn(U_); Turn(u_); Turn(d); Turn(D); break;
            case z: Turn(F); Turn(f); Turn(b_); Turn(B_); break;
            case z2: Turn(F2); Turn(f2); Turn(b2); Turn(B2); break;
            case z_: Turn(F_); Turn(f_); Turn(b); Turn(B); break;
            default: break;
        }
    }
    public string ReadCode(int Buffer=0){
        System.Text.StringBuilder sb = new();
        int remainBlocks = 0xffffff;
        remainBlocks &= ~(1 << Buffer);
        // Remove solved blocks
        for (int i = 0; i < 24; i++)
            if (state[i] == i)
                remainBlocks &= ~(1 << i);
        int head = Buffer;
        while(true){
            var next = this[head];
            // Append cycle middle to code
            while (next != head){
                sb.Append(Edge.Code[next]);
                remainBlocks &= ~(1 << next);
                next = this[next];
            }
            // Append cycle end to code
            if (next != Buffer){
                sb.Append(Edge.Code[next]);
            }
            if(remainBlocks == 0)
                break;
            // Find the next unsolved block
            head = 0;
            while(((remainBlocks >> head) & 1) == 0)
                ++head;
            remainBlocks &= ~(1 << head);
            // Append cycle head to code
            sb.Append(Edge.Code[head]);
        }
        return sb.ToString();
    }
}
