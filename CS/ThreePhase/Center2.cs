using static CS.ThreePhase.Util;
using static CS.ThreePhase.Moves;

namespace CS.ThreePhase
{
    /*
			    0	1
			    3	2

    4	5		8	9		0	1		12	13
    7	6		11	10		3	2		15	14

			    4	5
			    7	6
    */
    public class Center2
    {

        public int[] rl = new int[8];
        public int[] ct = new int[16];
        public int parity = 0;

        public static int[,] rlmv = new int[70, 28];
        public static char[,] ctmv = new char[6435, 28];
        public static int[,] rlrot = new int[70, 16];
        public static char[,] ctrot = new char[6435, 16];
        public static sbyte[] ctprun = new sbyte[6435 * 35 * 2];

        private static readonly int[] pmv = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1,
                        0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0};

        public static void Init()
        {
            Center2 c = new Center2();

            for (int i = 0; i < 35 * 2; i++)
            {
                for (int m = 0; m < 28; m++)
                {
                    c.Setrl(i);
                    c.Move(move2std[m]);
                    rlmv[i, m] = c.Getrl();
                }
            }

            for (int i = 0; i < 70; i++)
            {
                c.Setrl(i);
                for (int j = 0; j < 16; j++)
                {
                    rlrot[i, j] = c.Getrl();
                    c.Rot(0);
                    if (j % 2 == 1) c.Rot(1);
                    if (j % 8 == 7) c.Rot(2);
                }
            }

            for (int i = 0; i < 6435; i++)
            {
                c.Setct(i);
                for (int j = 0; j < 16; j++)
                {
                    ctrot[i, j] = (char)c.Getct();
                    c.Rot(0);
                    if (j % 2 == 1) c.Rot(1);
                    if (j % 8 == 7) c.Rot(2);
                }
            }

            for (int i = 0; i < 6435; i++)
            {
                for (int m = 0; m < 28; m++)
                {
                    c.Setct(i);
                    c.Move(move2std[m]);
                    ctmv[i, m] = (char)c.Getct();
                }
            }
            for (int i = 0; i < ctprun.Length; i++)
                ctprun[i] = -1;

            ctprun[0] = ctprun[18] = ctprun[28] = ctprun[46] = ctprun[54] = ctprun[56] = 0;
            int depth = 0;
            int done = 6;
            while (done != 6435 * 35 * 2)
            {
                for (int i = 0; i < 6435 * 35 * 2; i++)
                {
                    if (ctprun[i] != depth)
                    {
                        continue;
                    }
                    int ct = i / 70;
                    int rl = i % 70;
                    for (int m = 0; m < 23; m++)
                    {
                        int ctx = ctmv[ct, m];
                        int rlx = rlmv[rl, m];
                        int idx = ctx * 70 + rlx;
                        if (ctprun[idx] == -1)
                        {
                            ctprun[idx] = (sbyte)(depth + 1);
                            done++;
                        }
                    }
                }
                depth++;
                //			//System.out.println(string.format("%2d%10d", depth, done));
            }
        }

        public void Set(CenterCube c, int edgeParity)
        {
            for (int i = 0; i < 16; i++)
            {
                ct[i] = c.ct[i] % 3;
            }
            for (int i = 0; i < 8; i++)
            {
                rl[i] = c.ct[i + 16];
            }
            parity = edgeParity;
        }

        public int Getrl()
        {
            int idx = 0;
            int r = 4;
            for (int i = 6; i >= 0; i--)
            {
                if (rl[i] != rl[7])
                {
                    idx += Cnk[i, r--];
                }
            }
            return idx * 2 + parity;
        }

        public void Setrl(int idx)
        {
            parity = idx & 1;
            idx >>= 1; //>
            int r = 4;
            rl[7] = 0;
            for (int i = 6; i >= 0; i--)
            {
                if (idx >= Cnk[i, r])
                {
                    idx -= Cnk[i, r--];
                    rl[i] = 1;
                }
                else
                {
                    rl[i] = 0;
                }
            }
        }

        public int Getct()
        {
            int idx = 0;
            int r = 8;
            for (int i = 14; i >= 0; i--)
            {
                if (ct[i] != ct[15])
                {
                    idx += Cnk[i, r--];
                }
            }
            return idx;
        }

        public void Setct(int idx)
        {
            int r = 8;
            ct[15] = 0;
            for (int i = 14; i >= 0; i--)
            {
                if (idx >= Cnk[i, r])
                {
                    idx -= Cnk[i, r--];
                    ct[i] = 1;
                }
                else
                {
                    ct[i] = 0;
                }
            }
        }

        public void Rot(int r)
        {
            switch (r)
            {
                case 0:
                    Move(ux2);
                    Move(dx2);
                    break;
                case 1:
                    Move(rx1);
                    Move(lx3);
                    break;
                case 2:
                    Swap(ct, 0, 3, 1, 2, 1);
                    Swap(ct, 8, 11, 9, 10, 1);
                    Swap(ct, 4, 7, 5, 6, 1);
                    Swap(ct, 12, 15, 13, 14, 1);
                    Swap(rl, 0, 3, 5, 6, 1);
                    Swap(rl, 1, 2, 4, 7, 1);
                    break;
            }
        }


        public void Move(int m)
        {
            parity ^= pmv[m];
            int key = m % 3;
            m /= 3;
            switch (m)
            {
                case 0:     //U
                    Swap(ct, 0, 1, 2, 3, key);
                    break;
                case 1:     //R
                    Swap(rl, 0, 1, 2, 3, key);
                    break;
                case 2:     //F
                    Swap(ct, 8, 9, 10, 11, key);
                    break;
                case 3:     //D
                    Swap(ct, 4, 5, 6, 7, key);
                    break;
                case 4:     //L
                    Swap(rl, 4, 5, 6, 7, key);
                    break;
                case 5:     //B
                    Swap(ct, 12, 13, 14, 15, key);
                    break;
                case 6:     //u
                    Swap(ct, 0, 1, 2, 3, key);
                    Swap(rl, 0, 5, 4, 1, key);
                    Swap(ct, 8, 9, 12, 13, key);
                    break;
                case 7:     //r
                    Swap(rl, 0, 1, 2, 3, key);
                    Swap(ct, 1, 15, 5, 9, key);
                    Swap(ct, 2, 12, 6, 10, key);
                    break;
                case 8:     //f
                    Swap(ct, 8, 9, 10, 11, key);
                    Swap(rl, 0, 3, 6, 5, key);
                    Swap(ct, 3, 2, 5, 4, key);
                    break;
                case 9:     //d
                    Swap(ct, 4, 5, 6, 7, key);
                    Swap(rl, 3, 2, 7, 6, key);
                    Swap(ct, 11, 10, 15, 14, key);
                    break;
                case 10:    //l
                    Swap(rl, 4, 5, 6, 7, key);
                    Swap(ct, 0, 8, 4, 14, key);
                    Swap(ct, 3, 11, 7, 13, key);
                    break;
                case 11:    //b		
                    Swap(ct, 12, 13, 14, 15, key);
                    Swap(rl, 1, 4, 7, 2, key);
                    Swap(ct, 1, 0, 7, 6, key);
                    break;
            }
        }
    }
}
