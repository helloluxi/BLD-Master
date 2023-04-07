using System;

namespace CS.Min2Phase
{
    public class CoordCube
    {
        public const int N_MOVES = 18;
        public const int N_MOVES2 = 10;

        public const int N_SLICE = 495;
        public const int N_TWIST = 2187;
        public const int N_TWIST_HALF = (N_TWIST + 1) / 2;
        public const int N_TWIST_SYM = 324;
        public const int N_FLIP = 2048;
        public const int N_FLIP_HALF = (N_FLIP + 1) / 2;
        public const int N_FLIP_SYM = 336;
        public const int N_PERM = 40320;
        public const int N_PERM_HALF = (N_PERM + 1) / 2;
        public const int N_PERM_SYM = 2768;
        public const int N_MPERM = 24;
        public const int N_COMB = Search.USE_COMBP_PRUN ? 140 : 70;
        public const int P2_PARITY_MOVE = Search.USE_COMBP_PRUN ? 0xA5 : 0;

        //XMove = Move Table
        //XPrun = Pruning Table
        //XConj = Conjugate Table

        //phase1
        public static char[,] UDSliceMove = new char[N_SLICE, N_MOVES];
        public static char[,] TwistMove = new char[N_TWIST_SYM, N_MOVES];
        public static char[,] FlipMove = new char[N_FLIP_SYM, N_MOVES];
        public static char[,] UDSliceConj = new char[N_SLICE, 8];
        public static int[] UDSliceTwistPrun = new int[N_SLICE * N_TWIST_SYM / 8 + 1];
        public static int[] UDSliceFlipPrun = new int[N_SLICE * N_FLIP_SYM / 8 + 1];
        public static int[] TwistFlipPrun = Search.USE_TWIST_FLIP_PRUN ? new int[N_FLIP * N_TWIST_SYM / 8 + 1] : null;

        //phase2
        public static char[,] CPermMove = new char[N_PERM_SYM, N_MOVES2];
        public static char[,] EPermMove = new char[N_PERM_SYM, N_MOVES2];
        public static char[,] MPermMove = new char[N_MPERM, N_MOVES2];
        public static char[,] MPermConj = new char[N_MPERM, 16];
        public static char[,] CCombPMove;// = new char[N_COMB,N_MOVES2];
        public static char[,] CCombPConj = new char[N_COMB, 16];
        public static int[] MCPermPrun = new int[N_MPERM * N_PERM_SYM / 8 + 1];
        public static int[] EPermCCombPPrun = new int[N_COMB * N_PERM_SYM / 8 + 1];

        /**
         *  0: not initialized, 1: partially initialized, 2: finished
         */
        public static int initLevel = 0;

        public static void Init()
        {
            if (initLevel == 2)
            {
                return;
            }
            if (initLevel == 0)
            {
                CubieCube.InitPermSym2Raw();
                InitCPermMove();
                InitEPermMove();
                InitMPermMoveConj();
                InitCombPMoveConj();

                CubieCube.InitFlipSym2Raw();
                CubieCube.InitTwistSym2Raw();
                InitFlipMove();
                InitTwistMove();
                InitUDSliceMoveConj();
            }
            if (InitPruning(initLevel == 0))
            {
                initLevel = 2;

                //clean up
                CubieCube.SymStateTwist = null;
                CubieCube.SymStateFlip = null;
                CubieCube.SymStatePerm = null;
                CCombPMove = null;
            }
            else
            {
                initLevel = 1;
            }
            // System.out.println("initLevel: " + initLevel);
        }

        public static bool InitPruning(bool isFirst)
        {
            bool initedPrun = true;
            initedPrun = (initedPrun || isFirst) && InitMCPermPrun();
            initedPrun = (initedPrun || isFirst) && InitPermCombPPrun();
            initedPrun = (initedPrun || isFirst) && InitSliceTwistPrun();
            initedPrun = (initedPrun || isFirst) && InitSliceFlipPrun();
            if (Search.USE_TWIST_FLIP_PRUN)
            {
                initedPrun = (initedPrun || isFirst) && InitTwistFlipPrun();
            }
            return initedPrun;
        }

        public static void SetPruning(int[] table, int index, int value)
        {
            table[index >> 3] ^= value << (index << 2); // index << 2 <=> (index & 7) << 2
        }

        public static int GetPruning(int[] table, int index)
        {
            return table[index >> 3] >> (index << 2) & 0xf; // index << 2 <=> (index & 7) << 2
        }

        public static void SetPruning(sbyte[] table, int index, int value)
        {
            table[index >> 1] ^= (sbyte)(value << ((index & 1) << 2));
        }

        public static int GetPruning(sbyte[] table, int index)
        {
            return table[index >> 1] >> ((index & 1) << 2) & 0xf;
        }

        public static void InitUDSliceMoveConj()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_SLICE; i++)
            {
                c.SetUDSlice(i);
                for (int j = 0; j < N_MOVES; j += 3)
                {
                    CubieCube.EdgeMult(c, CubieCube.moveCube[j], d);
                    UDSliceMove[i, j] = (char)d.GetUDSlice();
                }
                for (int j = 0; j < 16; j += 2)
                {
                    CubieCube.EdgeConjugate(c, CubieCube.SymMultInv[0, j], d);
                    UDSliceConj[i, j >> 1] = (char)d.GetUDSlice();
                }
            }
            for (int i = 0; i < N_SLICE; i++)
            {
                for (int j = 0; j < N_MOVES; j += 3)
                {
                    int udslice = UDSliceMove[i, j];
                    for (int k = 1; k < 3; k++)
                    {
                        udslice = UDSliceMove[udslice, j];
                        UDSliceMove[i, j + k] = (char)udslice;
                    }
                }
            }
        }

        public static void InitFlipMove()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_FLIP_SYM; i++)
            {
                c.SetFlip(CubieCube.FlipS2R[i]);
                for (int j = 0; j < N_MOVES; j++)
                {
                    CubieCube.EdgeMult(c, CubieCube.moveCube[j], d);
                    FlipMove[i, j] = (char)d.GetFlipSym();
                }
            }
        }

        public static void InitTwistMove()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_TWIST_SYM; i++)
            {
                c.SetTwist(CubieCube.TwistS2R[i]);
                for (int j = 0; j < N_MOVES; j++)
                {
                    CubieCube.CornMult(c, CubieCube.moveCube[j], d);
                    TwistMove[i, j] = (char)d.GetTwistSym();
                }
            }
        }

        public static void InitCPermMove()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_PERM_SYM; i++)
            {
                c.SetCPerm(CubieCube.EPermS2R[i]);
                for (int j = 0; j < N_MOVES2; j++)
                {
                    CubieCube.CornMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                    CPermMove[i, j] = (char)d.GetCPermSym();
                }
            }
        }

        public static void InitEPermMove()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_PERM_SYM; i++)
            {
                c.SetEPerm(CubieCube.EPermS2R[i]);
                for (int j = 0; j < N_MOVES2; j++)
                {
                    CubieCube.EdgeMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                    EPermMove[i, j] = (char)d.GetEPermSym();
                }
            }
        }

        public static void InitMPermMoveConj()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_MPERM; i++)
            {
                c.SetMPerm(i);
                for (int j = 0; j < N_MOVES2; j++)
                {
                    CubieCube.EdgeMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                    MPermMove[i, j] = (char)d.GetMPerm();
                }
                for (int j = 0; j < 16; j++)
                {
                    CubieCube.EdgeConjugate(c, CubieCube.SymMultInv[0, j], d);
                    MPermConj[i, j] = (char)d.GetMPerm();
                }
            }
        }

        public static void InitCombPMoveConj()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            CCombPMove = new char[N_COMB, N_MOVES2];
            for (int i = 0; i < N_COMB; i++)
            {
                c.SetCComb(i % 70);
                for (int j = 0; j < N_MOVES2; j++)
                {
                    CubieCube.CornMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                    CCombPMove[i, j] = (char)(d.GetCComb() + 70 * ((P2_PARITY_MOVE >> j & 1) ^ (i / 70)));
                }
                for (int j = 0; j < 16; j++)
                {
                    CubieCube.CornConjugate(c, CubieCube.SymMultInv[0, j], d);
                    CCombPConj[i, j] = (char)(d.GetCComb() + 70 * (i / 70));
                }
            }
        }

        public static bool HasZero(int val)
        {
            return ((val - 0x11111111) & ~val & 0x88888888) != 0;
        }

        //          |   4 bits  |   4 bits  |   4 bits  |  2 bits | 1b |  1b |   4 bits  |
        //PrunFlag: | MIN_DEPTH | MAX_DEPTH | INV_DEPTH | Padding | P2 | E2C | SYM_SHIFT |
        public static bool InitRawSymPrun(int[] PrunTable, char[,] RawMove, char[,] RawConj, char[,] SymMove, char[] SymState, int PrunFlag)
        {
            int SYM_SHIFT = PrunFlag & 0xf;
            int SYM_E2C_MAGIC = ((PrunFlag >> 4) & 1) == 1 ? CubieCube.SYM_E2C_MAGIC : 0x00000000;
            bool IS_PHASE2 = ((PrunFlag >> 5) & 1) == 1;
            int INV_DEPTH = PrunFlag >> 8 & 0xf;
            int MAX_DEPTH = PrunFlag >> 12 & 0xf;
            int MIN_DEPTH = PrunFlag >> 16 & 0xf;

            int SYM_MASK = (1 << SYM_SHIFT) - 1;
            bool ISTFP = RawMove == null;
            int N_RAW = ISTFP ? N_FLIP : RawMove.GetLength(0);
            int N_SIZE = N_RAW * SymMove.GetLength(0);
            int N_MOVES = IS_PHASE2 ? 10 : 18;
            int NEXT_AXIS_MAGIC = N_MOVES == 10 ? 0x42 : 0x92492;

            int depth = GetPruning(PrunTable, N_SIZE) - 1;
            int done = 0;

            // long tt = System.nanoTime();

            if (depth == -1)
            {
                for (int i = 0; i < N_SIZE / 8 + 1; i++)
                {
                    PrunTable[i] = 0x11111111;
                }
                SetPruning(PrunTable, 0, 0 ^ 1);
                depth = 0;
                done = 1;
            }

            int SEARCH_DEPTH = Search.PARTIAL_INIT_LEVEL > 0 ?
                                     Math.Min(Math.Max(depth + 1, MIN_DEPTH), MAX_DEPTH) : MAX_DEPTH;

            while (depth < SEARCH_DEPTH)
            {
                unchecked
                {
                    int mask = (int)((depth + 1) * 0x11111111 ^ 0xffffffff);
                    for (int i = 0; i < PrunTable.Length; i++)
                    {
                        int val_ = PrunTable[i] ^ mask;
                        val_ &= val_ >> 1;
                        PrunTable[i] += val_ & (val_ >> 2) & 0x11111111;
                    }
                }

                bool inv = depth > INV_DEPTH;
                int select = inv ? (depth + 2) : depth;
                int selArrMask = select * 0x11111111;
                int check = inv ? depth : (depth + 2);
                depth++;
                int xorVal = depth ^ (depth + 1);
                int val = 0;
                for (int i = 0; i < N_SIZE; i++, val >>= 4)
                {
                    if ((i & 7) == 0)
                    {
                        val = PrunTable[i >> 3];
                        if (!HasZero(val ^ selArrMask))
                        {
                            i += 7;
                            continue;
                        }
                    }
                    if ((val & 0xf) != select)
                    {
                        continue;
                    }
                    int raw = i % N_RAW;
                    int sym = i / N_RAW;
                    int flip = 0, fsym = 0;
                    if (ISTFP)
                    {
                        flip = CubieCube.FlipRaw2Sym(raw);
                        fsym = flip & 7;
                        flip >>= 3;
                    }

                    for (int m = 0; m < N_MOVES; m++)
                    {
                        int symx = SymMove[sym, m];
                        int rawx;
                        if (ISTFP)
                        {
                            rawx = CubieCube.FlipS2RF[
                                       FlipMove[flip, CubieCube.Sym8Move[m << 3 | fsym]] ^
                                       fsym ^ (symx & SYM_MASK)];
                        }
                        else
                        {
                            rawx = RawConj[RawMove[raw, m], symx & SYM_MASK];

                        }
                        symx >>= SYM_SHIFT;
                        int idx = symx * N_RAW + rawx;
                        int prun = GetPruning(PrunTable, idx);
                        if (prun != check)
                        {
                            if (prun < depth - 1)
                            {
                                m += NEXT_AXIS_MAGIC >> m & 3;
                            }
                            continue;
                        }
                        done++;
                        if (inv)
                        {
                            SetPruning(PrunTable, i, xorVal);
                            break;
                        }
                        SetPruning(PrunTable, idx, xorVal);
                        for (int j = 1, symState = SymState[symx]; (symState >>= 1) != 0; j++)
                        {
                            if ((symState & 1) != 1)
                            {
                                continue;
                            }
                            int idxx = symx * N_RAW;
                            if (ISTFP)
                            {
                                idxx += CubieCube.FlipS2RF[CubieCube.FlipRaw2Sym(rawx) ^ j];
                            }
                            else
                            {
                                idxx += RawConj[rawx, j ^ (SYM_E2C_MAGIC >> (j << 1) & 3)];
                            }
                            if (GetPruning(PrunTable, idxx) == check)
                            {
                                SetPruning(PrunTable, idxx, xorVal);
                                done++;
                            }
                        }
                    }
                }
                // System.out.println(string.format("%2d%10d%10f", depth, done, (System.nanoTime() - tt) / 1e6d));
            }

            return Search.PARTIAL_INIT_LEVEL > 1 || depth == MAX_DEPTH;
        }

        public static bool InitTwistFlipPrun()
        {
            return InitRawSymPrun(
                       TwistFlipPrun,
                       null, null,
                       TwistMove, CubieCube.SymStateTwist, 0x19603
                   );
        }

        public static bool InitSliceTwistPrun()
        {
            return InitRawSymPrun(
                       UDSliceTwistPrun,
                       UDSliceMove, UDSliceConj,
                       TwistMove, CubieCube.SymStateTwist, 0x69603
                   );
        }

        public static bool InitSliceFlipPrun()
        {
            return InitRawSymPrun(
                       UDSliceFlipPrun,
                       UDSliceMove, UDSliceConj,
                       FlipMove, CubieCube.SymStateFlip, 0x69603
                   );
        }

        public static bool InitMCPermPrun()
        {
            return InitRawSymPrun(
                       MCPermPrun,
                       MPermMove, MPermConj,
                       CPermMove, CubieCube.SymStatePerm, 0x8ea34
                   );
        }

        public static bool InitPermCombPPrun()
        {
            return InitRawSymPrun(
                       EPermCCombPPrun,
                       CCombPMove, CCombPConj,
                       EPermMove, CubieCube.SymStatePerm, 0x7d824
                   );
        }


        public int twist;
        public int tsym;
        public int flip;
        public int fsym;
        public int slice;
        public int prun;

        public int twistc;
        public int flipc;

        public CoordCube() { }

        public void Set(CoordCube node)
        {
            this.twist = node.twist;
            this.tsym = node.tsym;
            this.flip = node.flip;
            this.fsym = node.fsym;
            this.slice = node.slice;
            this.prun = node.prun;

            if (Search.USE_CONJ_PRUN)
            {
                this.twistc = node.twistc;
                this.flipc = node.flipc;
            }
        }

        public void CalcPruning(bool isPhase1)
        {
            prun = Math.Max(
                       Math.Max(
                           GetPruning(UDSliceTwistPrun,
                                      twist * N_SLICE + UDSliceConj[slice, tsym]),
                           GetPruning(UDSliceFlipPrun,
                                      flip * N_SLICE + UDSliceConj[slice, fsym])),
                       Math.Max(
                           Search.USE_CONJ_PRUN ? GetPruning(TwistFlipPrun,
                                   (twistc >> 3) << 11 | CubieCube.FlipS2RF[flipc ^ (twistc & 7)]) : 0,
                           Search.USE_TWIST_FLIP_PRUN ? GetPruning(TwistFlipPrun,
                                   twist << 11 | CubieCube.FlipS2RF[flip << 3 | (fsym ^ tsym)]) : 0));
        }

        public bool SetWithPrun(CubieCube cc, int depth)
        {
            twist = cc.GetTwistSym();
            flip = cc.GetFlipSym();
            tsym = twist & 7;
            twist = twist >> 3;

            prun = Search.USE_TWIST_FLIP_PRUN ? GetPruning(TwistFlipPrun,
                    twist << 11 | CubieCube.FlipS2RF[flip ^ tsym]) : 0;
            if (prun > depth)
            {
                return false;
            }

            fsym = flip & 7;
            flip = flip >> 3;

            slice = cc.GetUDSlice();
            prun = Math.Max(prun, Math.Max(
                                GetPruning(UDSliceTwistPrun,
                                           twist * N_SLICE + UDSliceConj[slice, tsym]),
                                GetPruning(UDSliceFlipPrun,
                                           flip * N_SLICE + UDSliceConj[slice, fsym])));
            if (prun > depth)
            {
                return false;
            }

            if (Search.USE_CONJ_PRUN)
            {
                CubieCube pc = new CubieCube();
                CubieCube.CornConjugate(cc, 1, pc);
                CubieCube.EdgeConjugate(cc, 1, pc);
                twistc = pc.GetTwistSym();
                flipc = pc.GetFlipSym();
                prun = Math.Max(prun,
                                GetPruning(TwistFlipPrun,
                                           (twistc >> 3) << 11 | CubieCube.FlipS2RF[flipc ^ (twistc & 7)]));
            }

            return prun <= depth;
        }

        /**
         * @return pruning value
         */
        public int DoMovePrun(CoordCube cc, int m, bool isPhase1)
        {
            slice = UDSliceMove[cc.slice, m];

            flip = FlipMove[cc.flip, CubieCube.Sym8Move[m << 3 | cc.fsym]];
            fsym = (flip & 7) ^ cc.fsym;
            flip >>= 3;

            twist = TwistMove[cc.twist, CubieCube.Sym8Move[m << 3 | cc.tsym]];
            tsym = (twist & 7) ^ cc.tsym;
            twist >>= 3;

            prun = Math.Max(
                       Math.Max(
                           GetPruning(UDSliceTwistPrun,
                                      twist * N_SLICE + UDSliceConj[slice, tsym]),
                           GetPruning(UDSliceFlipPrun,
                                      flip * N_SLICE + UDSliceConj[slice, fsym])),
                       Search.USE_TWIST_FLIP_PRUN ? GetPruning(TwistFlipPrun,
                               twist << 11 | CubieCube.FlipS2RF[flip << 3 | (fsym ^ tsym)]) : 0);
            return prun;
        }

        public int DoMovePrunConj(CoordCube cc, int m)
        {
            m = CubieCube.SymMove[3, m];
            flipc = FlipMove[cc.flipc >> 3, CubieCube.Sym8Move[m << 3 | cc.flipc & 7]] ^ (cc.flipc & 7);
            twistc = TwistMove[cc.twistc >> 3, CubieCube.Sym8Move[m << 3 | cc.twistc & 7]] ^ (cc.twistc & 7);
            return GetPruning(TwistFlipPrun,
                              (twistc >> 3) << 11 | CubieCube.FlipS2RF[flipc ^ (twistc & 7)]);
        }
    }
}
