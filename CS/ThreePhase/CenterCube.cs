using System;
using static CS.ThreePhase.Util;

namespace CS.ThreePhase
{
    public class CenterCube
    {

        public sbyte[] ct = new sbyte[24];

        public CenterCube()
        {
            for (int i = 0; i < 24; i++)
            {
                ct[i] = (sbyte)(FullCube.centerFacelet[i] / 16);
            }
        }

        public CenterCube(CenterCube c)
        {
            Copy(c);
        }

        public CenterCube(Random r) : this()
        {
            for (int i = 0; i < 23; i++)
            {
                int t = i + r.Next(24 - i);
                if (ct[t] != ct[i])
                {
                    sbyte m = ct[i];
                    ct[i] = ct[t];
                    ct[t] = m;
                }
            }
        }

        public CenterCube(int[] moveseq) : this()
        {
            for (int m = 0; m < moveseq.Length; m++)
            {
                Move(m);
            }
        }

        public void Copy(CenterCube c)
        {
            for (int i = 0; i < 24; i++)
            {
                this.ct[i] = c.ct[i];
            }
        }

        /*void print()
        {
            for (int i = 0; i < 24; i++)
            {
                //System.out.print(ct[i]);
                //System.out.print('\t');
            }
            //System.out.println();
        }*/

        public static int[] center333Map = { 0, 4, 2, 1, 5, 3 };

        public void Fill333Facelet(char[] facelet)
        {
            int firstIdx = 4, inc = 9;
            for (int i = 0; i < 6; i++)
            {
                int idx = center333Map[i] << 2;
                if (ct[idx] != ct[idx + 1] || ct[idx + 1] != ct[idx + 2] || ct[idx + 2] != ct[idx + 3])
                {
                    throw new Exception("Runtime Exception : Unsolved Center");
                }
                facelet[firstIdx + i * inc] = "URFDLB"[ct[idx]];
            }
        }

        public void Move(int m)
        {
            int key = m % 3;
            m /= 3;
            switch (m)
            {
                case 0: //U
                    Swap(ct, 0, 1, 2, 3, key);
                    break;
                case 1: //R
                    Swap(ct, 16, 17, 18, 19, key);
                    break;
                case 2: //F
                    Swap(ct, 8, 9, 10, 11, key);
                    break;
                case 3: //D
                    Swap(ct, 4, 5, 6, 7, key);
                    break;
                case 4: //L
                    Swap(ct, 20, 21, 22, 23, key);
                    break;
                case 5: //B
                    Swap(ct, 12, 13, 14, 15, key);
                    break;
                case 6: //u
                    Swap(ct, 0, 1, 2, 3, key);
                    Swap(ct, 8, 20, 12, 16, key);
                    Swap(ct, 9, 21, 13, 17, key);
                    break;
                case 7: //r
                    Swap(ct, 16, 17, 18, 19, key);
                    Swap(ct, 1, 15, 5, 9, key);
                    Swap(ct, 2, 12, 6, 10, key);
                    break;
                case 8: //f
                    Swap(ct, 8, 9, 10, 11, key);
                    Swap(ct, 2, 19, 4, 21, key);
                    Swap(ct, 3, 16, 5, 22, key);
                    break;
                case 9: //d
                    Swap(ct, 4, 5, 6, 7, key);
                    Swap(ct, 10, 18, 14, 22, key);
                    Swap(ct, 11, 19, 15, 23, key);
                    break;
                case 10://l
                    Swap(ct, 20, 21, 22, 23, key);
                    Swap(ct, 0, 8, 4, 14, key);
                    Swap(ct, 3, 11, 7, 13, key);
                    break;
                case 11://b
                    Swap(ct, 12, 13, 14, 15, key);
                    Swap(ct, 1, 20, 7, 18, key);
                    Swap(ct, 0, 23, 6, 17, key);
                    break;
            }
        }
    }
}