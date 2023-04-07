using System;
using System.Text;
using System.Runtime.CompilerServices;

/**
 * <summary>
 * <para>Copyright (C) 2015  Shuang Chen</para>
 * <para>
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * </para>
 * <para>
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * </para>
 * <para>
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * </para>
 * </summary>
*/

/**
 * <summary><para>Rubik's Cube Solver.</para>
 * <para>A much faster and smaller implemention of Two-Phase Algorithm.</para>
 * <para>Symmetry is used to reduce memory used.</para>
 * <para>Total Memory used is about 1MB.</para></summary>
 * <author>Shuang Chen</author>
 */

namespace CS.Min2Phase
{
    public class Search
    {
        public const bool USE_TWIST_FLIP_PRUN = true;

        /**
         * If this variable is set, only a few entries of the pruning table will be initialized.
         * Hence, the initialization time will be decreased by about 50%, however, the speed
         * of the solver is affected.
         * 0: without partial initialization
         * 1: enable partial initialization, and the initialization will continue during solving
         * 2: enable partial initialization, and the initialization will not continue
         */
        public const int PARTIAL_INIT_LEVEL = 0;

        //Options for research purpose.
        public const int MAX_PRE_MOVES = 20;
        public const bool TRY_INVERSE = true;
        public const bool TRY_THREE_AXES = true;

        public const bool USE_COMBP_PRUN = USE_TWIST_FLIP_PRUN;
        public const bool USE_CONJ_PRUN = USE_TWIST_FLIP_PRUN;
        protected static int MIN_P1LENGTH_PRE = 7;
        protected static int MAX_DEPTH2 = 13;

        protected int[] move = new int[31];
        protected int[] moveSol = new int[31];
        protected readonly CoordCube[] nodeUD = new CoordCube[21];
        protected readonly CoordCube[] nodeRL = new CoordCube[21];
        protected readonly CoordCube[] nodeFB = new CoordCube[21];

        protected long selfSym;
        protected int conjMask;
        protected int urfIdx;
        protected int length1;
        protected int depth1;
        protected int maxDep2;
        protected int sol;
        protected string solution;
        protected long probe;
        protected long probeMax;
        protected long probeMin;
        protected int verbose;
        protected int valid1;
        protected bool allowShorter = false;
        protected readonly CubieCube cc = new CubieCube();
        protected readonly CubieCube[] urfCubieCube = new CubieCube[6];
        protected readonly CoordCube[] urfCoordCube = new CoordCube[6];
        protected readonly CubieCube[] phase1Cubie = new CubieCube[21];

        public readonly CubieCube[] preMoveCubes = new CubieCube[MAX_PRE_MOVES + 1];
        public readonly int[] preMoves = new int[MAX_PRE_MOVES];
        public int preMoveLen = 0;
        public int maxPreMoves = 0;
        public CubieCube phase2Cubie;

        protected bool isRec = false;
        public int Length => sol;
        public long NumberOfProbes => probe;

        /**
         * <summary>
         *      Verbose_Mask determines if a " . " separates the phase1
         * and phase2 parts of the solver string like in F' R B R L2 F .
         * U2 U D for example.</summary>
         */
        public const int USE_SEPARATOR = 0x1;

        /**
         * <summary>
         *      Verbose_Mask determines if the solution will be inversed
         * to a scramble/state generator.
         * </summary>
         */
        public const int INVERSE_SOLUTION = 0x2;

        /**
         * <summary>
         *      Verbose_Mask determines if a tag such as "(21f)" will be
         * appended to the solution.    
         * </summary>
         * 
         */
        public const int APPEND_LENGTH = 0x4;

        /**
         * <summary>
         *      Verbose_Mask determines if guaranteeing the solution to be optimal.
         * </summary>
         */
        public const int OPTIMAL_SOLUTION = 0x8;


        public Search()
        {
            for (int i = 0; i < 21; i++)
            {
                nodeUD[i] = new CoordCube();
                nodeRL[i] = new CoordCube();
                nodeFB[i] = new CoordCube();
                phase1Cubie[i] = new CubieCube();
            }
            for (int i = 0; i < 6; i++)
            {
                urfCubieCube[i] = new CubieCube();
                urfCoordCube[i] = new CoordCube();
            }
            for (int i = 0; i < MAX_PRE_MOVES; i++)
            {
                preMoveCubes[i + 1] = new CubieCube();
            }
        }

        /**
         * <summary>Computes the solver string for a given cube.</summary> 
         *
         * <param name="facelets" >
         * <para>is the cube definition string format.</para>
         * <para>The names of the facelet positions of the cube:</para>
         * <c>
         * <para>             |************|</para>
         * <para>             |*U1**U2**U3*|</para>
         * <para>             |************|</para>
         * <para>             |*U4**U5**U6*|</para>
         * <para>             |************|</para>
         * <para>             |*U7**U8**U9*|</para>
         * <para>             |************|</para>
         * <para> ************|************|************|************|</para>
         * <para> *L1**L2**L3*|*F1**F2**F3*|*R1**R2**R3*|*B1**B2**B3*|</para>
         * <para> ************|************|************|************|</para>
         * <para> *L4**L5**L6*|*F4**F5**F6*|*R4**R5**R6*|*B4**B5**B6*|</para>
         * <para> ************|************|************|************|</para>
         * <para> *L7**L8**L9*|*F7**F8**F9*|*R7**R8**R9*|*B7**B8**B9*|</para>
         * <para> ************|************|************|************|</para>
         * <para>             |************|</para>
         * <para>             |*D1**D2**D3*|</para>
         * <para>             |************|</para>
         * <para>             |*D4**D5**D6*|</para>
         * <para>             |************|</para>
         * <para>             |*D7**D8**D9*|</para>
         * <para>             |************|</para>
         * </c>
         * <para>A cube definition string "UBL..." means for example: In position U1 we have the U-color, in position U2 we have the
         * B-color, in position U3 we have the L color etc. For example, the "super flip" state is represented as </para>
         * <para><c>UBULURUFURURFRBRDRFUFLFRFDFDFDLDRDBDLULBLFLDLBUBRBLBDB</c></para>
         * <para>and the state generated by "F U' F2 D' B U R' F' L D' R' U' L U B' D2 R' F U2 D2" can be represented as </para>
         * <para><c>FBLLURRFBUUFBRFDDFUULLFRDDLRFBLDRFBLUUBFLBDDBUURRBLDDR</c></para>
         * <para>You can also use {@link cs.min2phase.Tools#fromScramble(java.lang.string s)} to convert the scramble string to the
         * cube definition string.</para>
         * </param>
         *
         * <param name="maxDepth">
         *      defines the maximal allowed maneuver length. For random cubes, a maxDepth of 21 usually will return a
         *      solution in less than 0.02 seconds on average. With a maxDepth of 20 it takes about 0.1 seconds on average to find a
         *      solution, but it may take much longer for specific cubes.
         * </param>
         *
         * 
         * <param name="probeMax">
         *      defines the maximum number of the probes of phase 2. If it does not return with a solution, it returns with
         *      an error code.
         * </param>
         *
         * <param name="probeMin">
         *      defines the minimum number of the probes of phase 2. So, if a solution is found within given probes, the
         *      computing will continue to find shorter solution(s). Btw, if probeMin > probeMax, probeMin will be set to probeMax.
         * </param>
         * 
         * <param name="verbose">
         *      determins the format of the solution(s). see USE_SEPARATOR, INVERSE_SOLUTION, APPEND_LENGTH, OPTIMAL_SOLUTION
         *</param>
         * 
         * <returns>
         * <para>The solution string or an error code:</para>
         * <para>Error 1: There is not exactly one facelet of each colour</para>
         * <para>Error 2: Not all 12 edges exist exactly once</para>
         * <para>Error 3: Flip error: One edge has to be flipped</para>
         * <para>Error 4: Not all corners exist exactly once</para>
         * <para>Error 5: Twist error: One corner has to be twisted</para>
         * <para>Error 6: Parity error: Two corners or two edges have to be exchanged</para>
         * <para>Error 7: No solution exists for the given maxDepth</para>
         * <para>Error 8: Probe limit exceeded, no solution within given probMax</para>
         *</returns>
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string Solution(string facelets, int maxDepth, long probeMax, long probeMin, int verbose)
        {
            int check = Verify(facelets);
            if (check != 0)
            {
                return "Error " + Math.Abs(check);
            }
            this.sol = maxDepth + 1;
            this.probe = 0;
            this.probeMax = probeMax;
            this.probeMin = Math.Min(probeMin, probeMax);
            this.verbose = verbose;
            this.solution = null;
            this.isRec = false;

            CoordCube.Init();
            InitSearch();

            return (verbose & OPTIMAL_SOLUTION) == 0 ? SSearch() : Searchopt();
        }

        protected void InitSearch()
        {
            conjMask = (TRY_INVERSE ? 0 : 0x38) | (TRY_THREE_AXES ? 0 : 0x36);
            selfSym = cc.SelfSymmetry();
            conjMask |= (selfSym >> 16 & 0xffff) != 0 ? 0x12 : 0;
            conjMask |= (selfSym >> 32 & 0xffff) != 0 ? 0x24 : 0;
            conjMask |= (selfSym >> 48 & 0xffff) != 0 ? 0x38 : 0;
            selfSym &= 0xffffffffffffL;
            maxPreMoves = conjMask > 7 ? 0 : MAX_PRE_MOVES;

            for (int i = 0; i < 6; i++)
            {
                urfCubieCube[i].Copy(cc);
                urfCoordCube[i].SetWithPrun(urfCubieCube[i], 20);
                cc.URFConjugate();
                if (i % 3 == 2)
                {
                    cc.InvCubieCube();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string Next(long probeMax, long probeMin, int verbose)
        {
            this.probe = 0;
            this.probeMax = probeMax;
            this.probeMin = Math.Min(probeMin, probeMax);
            this.solution = null;
            this.isRec = (this.verbose & OPTIMAL_SOLUTION) == (verbose & OPTIMAL_SOLUTION);
            this.verbose = verbose;
            return (verbose & OPTIMAL_SOLUTION) == 0 ? SSearch() : Searchopt();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        static Search()
        {
            CubieCube.InitMove();
            CubieCube.InitSym();
        }

        public int Verify(string facelets)
        {
            int count = 0x000000;
            sbyte[] f = new sbyte[54];
            try
            {
                string center = new string(
                    new char[] {
                    facelets[Util.U5],
                    facelets[Util.R5],
                    facelets[Util.F5],
                    facelets[Util.D5],
                    facelets[Util.L5],
                    facelets[Util.B5]
                    }
                );
                for (int i = 0; i < 54; i++)
                {
                    int j = center.IndexOf(facelets[i]);
                    if (j == -1) return -1;
                    f[i] = (sbyte)j;
                    count += 1 << (f[i] << 2);
                }
            }
            catch (Exception)
            {
                return -1;
            }
            if (count != 0x999999)
            {
                return -1;
            }
            Util.ToCubieCube(f, cc);
            return cc.Verify();
        }

        protected int Phase1PreMoves(int maxl, int lm, CubieCube cc, int ssym)
        {
            preMoveLen = maxPreMoves - maxl;
            if (isRec ? depth1 == length1 - preMoveLen
                    : (preMoveLen == 0 || (0x36FB7 >> lm & 1) == 0))
            {
                depth1 = length1 - preMoveLen;
                phase1Cubie[0] = cc;
                allowShorter = depth1 == MIN_P1LENGTH_PRE && preMoveLen != 0;

                if (nodeUD[depth1 + 1].SetWithPrun(cc, depth1)
                        && Phase1(nodeUD[depth1 + 1], ssym, depth1, -1) == 0)
                {
                    return 0;
                }
            }

            if (maxl == 0 || preMoveLen + MIN_P1LENGTH_PRE >= length1)
            {
                return 1;
            }

            int skipMoves = CubieCube.GetSkipMoves(ssym);
            if (maxl == 1 || preMoveLen + 1 + MIN_P1LENGTH_PRE >= length1)
            { //last pre move
                skipMoves |= 0x36FB7; // 11 0110 1111 1011 0111
            }

            lm = lm / 3 * 3;
            for (int m = 0; m < 18; m++)
            {
                if (m == lm || m == lm - 9 || m == lm + 9)
                {
                    m += 2;
                    continue;
                }
                if (isRec && m != preMoves[maxPreMoves - maxl] || (skipMoves & 1 << m) != 0)
                {
                    continue;
                }
                CubieCube.CornMult(CubieCube.moveCube[m], cc, preMoveCubes[maxl]);
                CubieCube.EdgeMult(CubieCube.moveCube[m], cc, preMoveCubes[maxl]);
                preMoves[maxPreMoves - maxl] = m;
                int ret = Phase1PreMoves(maxl - 1, m, preMoveCubes[maxl], ssym & (int)CubieCube.moveCubeSym[m]);
                if (ret == 0)
                {
                    return 0;
                }
            }
            return 1;
        }

        protected string SSearch()
        {
            for (length1 = isRec ? length1 : 0; length1 < sol; length1++)
            {
                maxDep2 = Math.Min(MAX_DEPTH2, sol - length1);
                for (urfIdx = isRec ? urfIdx : 0; urfIdx < 6; urfIdx++)
                {
                    if ((conjMask & 1 << urfIdx) != 0)
                    {
                        continue;
                    }
                    if (Phase1PreMoves(maxPreMoves, -30, urfCubieCube[urfIdx], (int)(selfSym & 0xffff)) == 0)
                    {
                        return solution ?? "Error 8";
                    }
                }
            }
            return solution ?? "Error 7";
        }

        protected int InitPhase2Pre()
        {
            isRec = false;
            if (probe >= (solution == null ? probeMax : probeMin))
            {
                return 0;
            }
            ++probe;

            for (int i = valid1; i < depth1; i++)
            {
                CubieCube.CornMult(phase1Cubie[i], CubieCube.moveCube[move[i]], phase1Cubie[i + 1]);
                CubieCube.EdgeMult(phase1Cubie[i], CubieCube.moveCube[move[i]], phase1Cubie[i + 1]);
            }
            valid1 = depth1;
            phase2Cubie = phase1Cubie[depth1];

            int ret = InitPhase2();
            if (ret == 0 || preMoveLen == 0 || ret == 2)
            {
                return ret;
            }

            int m = preMoves[preMoveLen - 1] / 3 * 3 + 1;
            phase2Cubie = new CubieCube();
            CubieCube.CornMult(CubieCube.moveCube[m], phase1Cubie[depth1], phase2Cubie);
            CubieCube.EdgeMult(CubieCube.moveCube[m], phase1Cubie[depth1], phase2Cubie);

            preMoves[preMoveLen - 1] += 2 - preMoves[preMoveLen - 1] % 3 * 2;
            ret = InitPhase2();
            preMoves[preMoveLen - 1] += 2 - preMoves[preMoveLen - 1] % 3 * 2;
            return ret;
        }

        protected int InitPhase2()
        {
            int p2corn = phase2Cubie.GetCPermSym();
            int p2csym = p2corn & 0xf;
            p2corn >>= 4;
            int p2edge = phase2Cubie.GetEPermSym();
            int p2esym = p2edge & 0xf;
            p2edge >>= 4;
            int p2mid = phase2Cubie.GetMPerm();

            int prun = Math.Max(
                           CoordCube.GetPruning(CoordCube.EPermCCombPPrun,
                                                p2edge * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[p2corn] & 0xff, CubieCube.SymMultInv[p2esym, p2csym]]),
                           CoordCube.GetPruning(CoordCube.MCPermPrun,
                                                p2corn * CoordCube.N_MPERM + CoordCube.MPermConj[p2mid, p2csym]));

            if (prun >= maxDep2)
            {
                return prun > maxDep2 ? 2 : 1;
            }

            int depth2;
            for (depth2 = maxDep2 - 1; depth2 >= prun; depth2--)
            {
                int ret = Phase2(p2edge, p2esym, p2corn, p2csym, p2mid, depth2, depth1, 10);
                if (ret < 0)
                {
                    break;
                }
                depth2 -= ret;
                sol = 0;
                for (int i = 0; i < depth1 + depth2; i++)
                {
                    AppendSolMove(move[i]);
                }
                for (int i = preMoveLen - 1; i >= 0; i--)
                {
                    AppendSolMove(preMoves[i]);
                }
                solution = SolutionTostring();
            }

            if (depth2 != maxDep2 - 1)
            { //At least one solution has been found.
                maxDep2 = Math.Min(MAX_DEPTH2, sol - length1);
                return probe >= probeMin ? 0 : 1;
            }
            else
            {
                return 1;
            }
        }

        /**
         * <returns>
         * <para>0: Found or Probe limit exceeded</para>
         * <para>1: Try Next Power</para>
         * <para>2: Try Next Axis</para>
         * </returns>
         */
        protected int Phase1(CoordCube node, int ssym, int maxl, int lm)
        {
            if (node.prun == 0 && maxl < 5)
            {
                if (allowShorter || maxl == 0)
                {
                    depth1 -= maxl;
                    int ret = InitPhase2Pre();
                    depth1 += maxl;
                    return ret;
                }
                else
                {
                    return 1;
                }
            }

            int skipMoves = CubieCube.GetSkipMoves(ssym);

            for (int axis = 0; axis < 18; axis += 3)
            {
                if (axis == lm || axis == lm - 9)
                {
                    continue;
                }
                for (int power = 0; power < 3; power++)
                {
                    int m = axis + power;

                    if (isRec && m != move[depth1 - maxl]
                            || skipMoves != 0 && (skipMoves & 1 << m) != 0)
                    {
                        continue;
                    }

                    int prun = nodeUD[maxl].DoMovePrun(node, m, true);
                    if (prun > maxl)
                    {
                        break;
                    }
                    else if (prun == maxl)
                    {
                        continue;
                    }

                    if (USE_CONJ_PRUN)
                    {
                        prun = nodeUD[maxl].DoMovePrunConj(node, m);
                        if (prun > maxl)
                        {
                            break;
                        }
                        else if (prun == maxl)
                        {
                            continue;
                        }
                    }

                    move[depth1 - maxl] = m;
                    valid1 = Math.Min(valid1, depth1 - maxl);
                    int ret = Phase1(nodeUD[maxl], ssym & (int)CubieCube.moveCubeSym[m], maxl - 1, axis);
                    if (ret == 0)
                    {
                        return 0;
                    }
                    else if (ret == 2)
                    {
                        break;
                    }
                }
            }
            return 1;
        }

        protected string Searchopt()
        {
            int maxprun1 = 0;
            int maxprun2 = 0;
            for (int i = 0; i < 6; i++)
            {
                urfCoordCube[i].CalcPruning(false);
                if (i < 3)
                {
                    maxprun1 = Math.Max(maxprun1, urfCoordCube[i].prun);
                }
                else
                {
                    maxprun2 = Math.Max(maxprun2, urfCoordCube[i].prun);
                }
            }
            urfIdx = maxprun2 > maxprun1 ? 3 : 0;
            phase1Cubie[0] = urfCubieCube[urfIdx];
            for (length1 = isRec ? length1 : 0; length1 < sol; length1++)
            {
                CoordCube ud = urfCoordCube[0 + urfIdx];
                CoordCube rl = urfCoordCube[1 + urfIdx];
                CoordCube fb = urfCoordCube[2 + urfIdx];

                if (ud.prun <= length1 && rl.prun <= length1 && fb.prun <= length1
                        && Phase1opt(ud, rl, fb, selfSym, length1, -1) == 0)
                {
                    return solution ?? "Error 8";
                }
            }
            return solution ?? "Error 7";
        }

        /** <returns>
         * <para>0: Found or Probe limit exceeded</para>
         * <para>1: Try Next Power</para>
         * <para>2: Try Next Axis</para>
         * </returns>
         */
        protected int Phase1opt(CoordCube ud, CoordCube rl, CoordCube fb, long ssym, int maxl, int lm)
        {
            if (ud.prun == 0 && rl.prun == 0 && fb.prun == 0 && maxl < 5)
            {
                maxDep2 = maxl + 1;
                depth1 = length1 - maxl;
                return InitPhase2Pre() == 0 ? 0 : 1;
            }

            int skipMoves = CubieCube.GetSkipMoves(ssym);

            for (int axis = 0; axis < 18; axis += 3)
            {
                if (axis == lm || axis == lm - 9)
                {
                    continue;
                }
                for (int power = 0; power < 3; power++)
                {
                    int m = axis + power;

                    if (isRec && m != move[length1 - maxl]
                            || skipMoves != 0 && (skipMoves & 1 << m) != 0)
                    {
                        continue;
                    }

                    // UD Axis
                    int prun_ud = Math.Max(nodeUD[maxl].DoMovePrun(ud, m, false),
                                           USE_CONJ_PRUN ? nodeUD[maxl].DoMovePrunConj(ud, m) : 0);
                    if (prun_ud > maxl)
                    {
                        break;
                    }
                    else if (prun_ud == maxl)
                    {
                        continue;
                    }

                    // RL Axis
                    m = CubieCube.urfMove[2, m];

                    int prun_rl = Math.Max(nodeRL[maxl].DoMovePrun(rl, m, false),
                                           USE_CONJ_PRUN ? nodeRL[maxl].DoMovePrunConj(rl, m) : 0);
                    if (prun_rl > maxl)
                    {
                        break;
                    }
                    else if (prun_rl == maxl)
                    {
                        continue;
                    }

                    // FB Axis
                    m = CubieCube.urfMove[2, m];

                    int prun_fb = Math.Max(nodeFB[maxl].DoMovePrun(fb, m, false),
                                           USE_CONJ_PRUN ? nodeFB[maxl].DoMovePrunConj(fb, m) : 0);
                    if (prun_ud == prun_rl && prun_rl == prun_fb && prun_fb != 0)
                    {
                        prun_fb++;
                    }

                    if (prun_fb > maxl)
                    {
                        break;
                    }
                    else if (prun_fb == maxl)
                    {
                        continue;
                    }

                    m = CubieCube.urfMove[2, m];

                    move[length1 - maxl] = m;
                    valid1 = Math.Min(valid1, length1 - maxl);
                    int ret = Phase1opt(nodeUD[maxl], nodeRL[maxl], nodeFB[maxl], ssym & CubieCube.moveCubeSym[m], maxl - 1, axis);
                    if (ret == 0)
                    {
                        return 0;
                    }
                }
            }
            return 1;
        }

        public void AppendSolMove(int curMove)
        {
            if (sol == 0)
            {
                moveSol[sol++] = curMove;
                return;
            }
            int axisCur = curMove / 3;
            int axisLast = moveSol[sol - 1] / 3;
            if (axisCur == axisLast)
            {
                int pow = (curMove % 3 + moveSol[sol - 1] % 3 + 1) % 4;
                if (pow == 3)
                {
                    sol--;
                }
                else
                {
                    moveSol[sol - 1] = axisCur * 3 + pow;
                }
                return;
            }
            if (sol > 1
                    && axisCur % 3 == axisLast % 3
                    && axisCur == moveSol[sol - 2] / 3)
            {
                int pow = (curMove % 3 + moveSol[sol - 2] % 3 + 1) % 4;
                if (pow == 3)
                {
                    moveSol[sol - 2] = moveSol[sol - 1];
                    sol--;
                }
                else
                {
                    moveSol[sol - 2] = axisCur * 3 + pow;
                }
                return;
            }
            moveSol[sol++] = curMove;
        }

        ///<returns>
        ///<para>-1: no solution found</para>
        ///<para>X: solution with X moves shorter than expectation.
        ///Hence, the length of the solution is  depth - X</para>
        ///</returns>
        protected int Phase2(int edge, int esym, int corn, int csym, int mid, int maxl, int depth, int lm)
        {
            if (edge == 0 && corn == 0 && mid == 0)
            {
                return maxl;
            }
            int moveMask = Util.ckmv2bit[lm];
            for (int m = 0; m < 10; m++)
            {
                if ((moveMask >> m & 1) != 0)
                {
                    m += 0x42 >> m & 3;
                    continue;
                }
                int midx = CoordCube.MPermMove[mid, m];
                int cornx = CoordCube.CPermMove[corn, CubieCube.SymMoveUD[csym, m]];
                int csymx = CubieCube.SymMult[cornx & 0xf, csym];
                cornx >>= 4;
                int edgex = CoordCube.EPermMove[edge, CubieCube.SymMoveUD[esym, m]];
                int esymx = CubieCube.SymMult[edgex & 0xf, esym];
                edgex >>= 4;
                int edgei = CubieCube.GetPermSymInv(edgex, esymx, false);
                int corni = CubieCube.GetPermSymInv(cornx, csymx, true);

                int prun = CoordCube.GetPruning(CoordCube.EPermCCombPPrun,
                                                (edgei >> 4) * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[corni >> 4] & 0xff, CubieCube.SymMultInv[edgei & 0xf, corni & 0xf]]);
                if (prun > maxl + 1)
                {
                    break;
                }
                else if (prun >= maxl)
                {
                    m += 0x42 >> m & 3 & (maxl - prun);
                    continue;
                }
                prun = Math.Max(
                           CoordCube.GetPruning(CoordCube.MCPermPrun,
                                                cornx * CoordCube.N_MPERM + CoordCube.MPermConj[midx, csymx]),
                           CoordCube.GetPruning(CoordCube.EPermCCombPPrun,
                                                edgex * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[cornx] & 0xff, CubieCube.SymMultInv[esymx, csymx]]));
                if (prun >= maxl)
                {
                    m += 0x42 >> m & 3 & (maxl - prun);
                    continue;
                }
                int ret = Phase2(edgex, esymx, cornx, csymx, midx, maxl - 1, depth + 1, m);
                if (ret >= 0)
                {
                    move[depth] = Util.ud2std[m];
                    return ret;
                }
            }
            return -1;
        }

        protected string SolutionTostring()
        {
            StringBuilder sb = new StringBuilder();
            int urf = (verbose & INVERSE_SOLUTION) != 0 ? (urfIdx + 3) % 6 : urfIdx;
            if (urf < 3)
            {
                for (int s = 0; s < sol; s++)
                {
                    if ((verbose & USE_SEPARATOR) != 0 && s == depth1)
                    {
                        sb.Append(".  ");
                    }
                    sb.Append(Util.move2str[CubieCube.urfMove[urf, moveSol[s]]]).Append(" ");
                }
            }
            else
            {
                for (int s = sol - 1; s >= 0; s--)
                {
                    sb.Append(Util.move2str[CubieCube.urfMove[urf, moveSol[s]]]).Append(" ");
                    if ((verbose & USE_SEPARATOR) != 0 && s == depth1)
                    {
                        sb.Append(".  ");
                    }
                }
            }
            if ((verbose & APPEND_LENGTH) != 0)
            {
                sb.Append($"({sol}f)");
            }
            return sb.ToString();
        }
    }
}
