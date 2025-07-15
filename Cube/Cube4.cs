
using static Cube.Move;
using static Cube.Tools;

namespace Cube;
public class Cube4
{
    public Wing wing = new();
    public Corner corner = new();
    public XCenter xcenter = new();

    public Cube4()
    {
        corner = new Corner();
        wing = new Wing();
        xcenter = new XCenter();
    }

    public static Cube4 RandomCube4(bool corner=true, bool wing=true, bool xcenter=true)
    {
        Cube4 cube = new()
        {
            corner = corner ? Corner.Random() : new Corner(),
            wing = wing ? Wing.Random() : new Wing(),
            xcenter = xcenter ? XCenter.Random() : new XCenter()
        };
        // Random parity
        if (corner && rd.Next(2) == 0)
            cube.corner.Turn(U);
        return cube;
    }
    public void Turn(IEnumerable<Move> s)
    {
        foreach (Move m in s) Turn(m);
    }
    public void Turn(Move m)
    {
        switch (m)
        {
            case U: wing.Turn(U); corner.Turn(U); xcenter.Turn(U); break;
            case U2: wing.Turn(U2); corner.Turn(U2); xcenter.Turn(U2); break;
            case U_: wing.Turn(U_); corner.Turn(U_); xcenter.Turn(U_); break;
            case u: wing.Turn(u); xcenter.Turn(u); break;
            case u2: wing.Turn(u2); xcenter.Turn(u2); break;
            case u_: wing.Turn(u_); xcenter.Turn(u_); break;
            case Uw: Turn(U); Turn(u); break;
            case Uw2: Turn(U2); Turn(u2); break;
            case Uw_: Turn(U_); Turn(u_); break;
            case _3Uw: Turn(U); Turn(u); Turn(d_); break;
            case _3Uw2: Turn(U2); Turn(u2); Turn(d2); break;
            case _3Uw_: Turn(U_); Turn(u_); Turn(d); break;
            case D: wing.Turn(D); corner.Turn(D); xcenter.Turn(D); break;
            case D2: wing.Turn(D2); corner.Turn(D2); xcenter.Turn(D2); break;
            case D_: wing.Turn(D_); corner.Turn(D_); xcenter.Turn(D_); break;
            case d: wing.Turn(d); xcenter.Turn(d); break;
            case d2: wing.Turn(d2); xcenter.Turn(d2); break;
            case d_: wing.Turn(d_); xcenter.Turn(d_); break;
            case Dw: Turn(D); Turn(d); break;
            case Dw2: Turn(D2); Turn(d2); break;
            case Dw_: Turn(D_); Turn(d_); break;
            case _3Dw: Turn(D); Turn(d); Turn(u_); break;
            case _3Dw2: Turn(D2); Turn(d2); Turn(u2); break;
            case _3Dw_: Turn(D_); Turn(d_); Turn(u); break;
            case R: wing.Turn(R); corner.Turn(R); xcenter.Turn(R); break;
            case R2: wing.Turn(R2); corner.Turn(R2); xcenter.Turn(R2); break;
            case R_: wing.Turn(R_); corner.Turn(R_); xcenter.Turn(R_); break;
            case r: wing.Turn(r); xcenter.Turn(r); break;
            case r2: wing.Turn(r2); xcenter.Turn(r2); break;
            case r_: wing.Turn(r_); xcenter.Turn(r_); break;
            case Rw: Turn(R); Turn(r); break;
            case Rw2: Turn(R2); Turn(r2); break;
            case Rw_: Turn(R_); Turn(r_); break;
            case _3Rw: Turn(R); Turn(r); Turn(l_); break;
            case _3Rw2: Turn(R2); Turn(r2); Turn(l2); break;
            case _3Rw_: Turn(R_); Turn(r_); Turn(l); break;
            case L: wing.Turn(L); corner.Turn(L); xcenter.Turn(L); break;
            case L2: wing.Turn(L2); corner.Turn(L2); xcenter.Turn(L2); break;
            case L_: wing.Turn(L_); corner.Turn(L_); xcenter.Turn(L_); break;
            case l: wing.Turn(l); xcenter.Turn(l); break;
            case l2: wing.Turn(l2); xcenter.Turn(l2); break;
            case l_: wing.Turn(l_); xcenter.Turn(l_); break;
            case Lw: Turn(L); Turn(l); break;
            case Lw2: Turn(L2); Turn(l2); break;
            case Lw_: Turn(L_); Turn(l_); break;
            case _3Lw: Turn(L); Turn(l); Turn(r_); break;
            case _3Lw2: Turn(L2); Turn(l2); Turn(r2); break;
            case _3Lw_: Turn(L_); Turn(l_); Turn(r); break;
            case F: wing.Turn(F); corner.Turn(F); xcenter.Turn(F); break;
            case F2: wing.Turn(F2); corner.Turn(F2); xcenter.Turn(F2); break;
            case F_: wing.Turn(F_); corner.Turn(F_); xcenter.Turn(F_); break;
            case f: wing.Turn(f); xcenter.Turn(f); break;
            case f2: wing.Turn(f2); xcenter.Turn(f2); break;
            case f_: wing.Turn(f_); xcenter.Turn(f_); break;
            case Fw: Turn(F); Turn(f); break;
            case Fw2: Turn(F2); Turn(f2); break;
            case Fw_: Turn(F_); Turn(f_); break;
            case _3Fw: Turn(F); Turn(f); Turn(b_); break;
            case _3Fw2: Turn(F2); Turn(f2); Turn(b2); break;
            case _3Fw_: Turn(F_); Turn(f_); Turn(b); break;
            case B: wing.Turn(B); corner.Turn(B); xcenter.Turn(B); break;
            case B2: wing.Turn(B2); corner.Turn(B2); xcenter.Turn(B2); break;
            case B_: wing.Turn(B_); corner.Turn(B_); xcenter.Turn(B_); break;
            case b: wing.Turn(b); xcenter.Turn(b); break;
            case b2: wing.Turn(b2); xcenter.Turn(b2); break;
            case b_: wing.Turn(b_); xcenter.Turn(b_); break;
            case Bw: Turn(B); Turn(b); break;
            case Bw2: Turn(B2); Turn(b2); break;
            case Bw_: Turn(B_); Turn(b_); break;
            case _3Bw: Turn(B); Turn(b); Turn(f_); break;
            case _3Bw2: Turn(B2); Turn(b2); Turn(f2); break;
            case _3Bw_: Turn(B_); Turn(b_); Turn(f); break;
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
    public void Cycle(string wcCode){
        wing.Cycle(wcCode);
        corner.Cycle(wcCode);
    }
    public void Solve()
    {
        corner.Solve();
        wing.Solve();
        xcenter.Solve();
    }
    public bool IsSolved() => corner.IsSolved() && wing.IsSolved() && xcenter.IsSolved();
    public string GetScramble() => CS.Global.GetScramble4(ToString());
    
    private static readonly int[] WingIndex = [
        14, 34, 33, 13, 8, 66, 65, 4, 1, 82, 81, 2,
        7, 18, 17, 11, 49, 45, 46, 50, 56, 77, 78, 52,
        62, 93, 94, 61, 55, 29, 30, 59, 39, 20, 24, 43,
        40, 75, 71, 36, 87, 68, 72, 91, 88, 27, 23, 84 
    ], CornerIndex = [ 12, 32, 67, 0, 64, 83, 3, 80, 19, 15, 16, 35,
        48, 79, 44, 60, 95, 76, 63, 31, 92, 51, 47, 28 
    ], XCenterIndex = [ 10, 9, 5, 6, 21, 22, 26, 25, 37, 38, 42, 41,
        53, 54, 58, 57, 69, 70, 74, 73, 85, 86, 90, 89 ];
    public const string faces = "URFDLB";
    public override string ToString()
    {
        char[] state = new char[96];
        for (int i = 0; i < 8; i++)
        {
            CycleConfig s = corner[i, 0];
            state[CornerIndex[i * 3]] = faces[CornerIndex[s.perm * 3 + s.ori] / 16];
            state[CornerIndex[i * 3 + 1]] = faces[CornerIndex[s.perm * 3 + (s.ori + 1) % 3] / 16];
            state[CornerIndex[i * 3 + 2]] = faces[CornerIndex[s.perm * 3 + (s.ori + 2) % 3] / 16];
        }
        for (int i = 0; i < 24; i++)
        {
            state[WingIndex[i * 2]] = faces[WingIndex[wing[i] * 2] / 16];
            state[WingIndex[i * 2 + 1]] = faces[WingIndex[wing[i] * 2 + 1] / 16];
            state[XCenterIndex[i]] = faces[xcenter[i]];
        }
        return new string(state);
    }
    public string ReadCode() => wing.ReadCode() + corner.ReadCode() + (xcenter.IsSolved() ? "" : "!");
}
