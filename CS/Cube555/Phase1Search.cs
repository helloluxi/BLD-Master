using System;
using static CS.Cube555.Util;
namespace CS.Cube555;

/*
           0  0  1
           3     1
           3  2  2

20 20 21   8  8  9    16 16 17   12 12  13
23    21   11    9    19    17   15     13
23 22 22   11 10 10   19 18 18   15 14  14

           4  4  5
           7     5
           7  6  6
*/

public class Phase1Search : PhaseSearch {

	public static new int[] VALID_MOVES = [
	    Ux1, Ux2, Ux3, Rx1, Rx2, Rx3, Fx1, Fx2, Fx3, Dx1, Dx2, Dx3, Lx1, Lx2, Lx3, Bx1, Bx2, Bx3,
	    ux1, ux2, ux3, rx1, rx2, rx3, fx1, fx2, fx3, dx1, dx2, dx3, lx1, lx2, lx3, bx1, bx2, bx3
	];

	public static PruningTable TCenterPrun;
	public static PruningTable XCenterPrun;
	public static PruningTable CenterSymPrun;
 
	public static PruningTable TCenterSymPrun;
	public static PruningTable XCenterSymPrun;
	public static int[] TCenterSym2RawF;
	public static int[] TCenterRaw2Sym;
	public static int[, ] TCenterSymMove;
	public static int[] XCenterSym2Raw;
	public static int[] XCenterRaw2Sym;
	public static int[, ] XCenterSymMove;

	public static int[, ] SymMove;

	public static void init() {
		initCenter();
	}

	public static void initCenter() {
		Phase1Center center = new();
		int symCnt = 0;
		TCenterSym2RawF = new int[46935 * 16];
		TCenterRaw2Sym = new int[735471];
		int[] TCenterSelfSym = new int[46935];
		for(int i = 0; i < TCenterRaw2Sym.Length; i++) {
			if (TCenterRaw2Sym[i] != 0) {
				continue;
			}
			center.setTCenter(i);
			for(int sym = 0; sym < 16; sym++) {
				int idx = center.getTCenter();
				TCenterRaw2Sym[idx] = symCnt << 4 | sym;
				TCenterSym2RawF[symCnt << 4 | sym] = idx;
				if (idx == i) {
					TCenterSelfSym[symCnt] |= 1 << sym;
				}
				center.doConj(0);
				if ((sym & 3) == 3) {
					center.doConj(1);
				}
				if ((sym & 7) == 7) {
					center.doConj(2);
				}
			}
			symCnt++;
		}
		TCenterSymMove = new int[symCnt, VALID_MOVES.Length];
		for(int i = 0; i < symCnt; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				center.setTCenter(TCenterSym2RawF[i << 4]);
				center.doMove(m);
				TCenterSymMove[i, m] = TCenterRaw2Sym[center.getTCenter()];
			}
		}

		symCnt = 0;
		XCenterSym2Raw = new int[46371];
		XCenterRaw2Sym = new int[735471];
		int[] XCenterSelfSym = new int[46371];
		for(int i = 0; i < XCenterRaw2Sym.Length; i++) {
			if (XCenterRaw2Sym[i] != 0) {
				continue;
			}
			center.setXCenter(i);
			for(int sym = 0; sym < 16; sym++) {
				int idx = center.getXCenter();
				XCenterRaw2Sym[idx] = symCnt << 4 | sym;
				if (idx == i) {
					XCenterSelfSym[symCnt] |= 1 << sym;
				}
				center.doConj(0);
				if ((sym & 3) == 3) {
					center.doConj(1);
				}
				if ((sym & 7) == 7) {
					center.doConj(2);
				}
			}
			XCenterSym2Raw[symCnt] = i;
			symCnt++;
		}
		XCenterSymMove = new int[symCnt, VALID_MOVES.Length];
		for(int i = 0; i < symCnt; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				center.setXCenter(XCenterSym2Raw[i]);
				center.doMove(m);
				XCenterSymMove[i, m] = XCenterRaw2Sym[center.getXCenter()];
			}
		}

		SymCoord XCenterSymCoord = new TableSymCoord(XCenterSymMove, XCenterSelfSym, 16);

		TCenterSymPrun = new PruningTable(
		    new TableSymCoord(TCenterSymMove, TCenterSelfSym, 16),
		    null, "Phase1TCenterSym");

		XCenterSymPrun = new PruningTable(
		    XCenterSymCoord,
		    null, "Phase1XCenterSym");

		SymMove = CubieCube.getSymMove(VALID_MOVES, 16);

		var rawCoord = new RawCoord() {
			N_IDX = 735471
		};
		rawCoord.set = (idx) => {
			rawCoord.idx = TCenterRaw2Sym[idx];
		};
		rawCoord.getMoved = (move) => {
			int ret = TCenterSymMove[rawCoord.idx >> 4, SymMove[rawCoord.idx & 0xf, move]];
			ret = ret & ~0xf | CubieCube.SymMult[ret & 0xf, rawCoord.idx & 0xf];
			return TCenterSym2RawF[ret];
		};
		rawCoord.getConj = (idx, conj) => {
			idx = TCenterRaw2Sym[idx];
			idx = idx & ~0xf | CubieCube.SymMultInv[idx & 0xf, conj];
			return TCenterSym2RawF[idx];
		};

		CenterSymPrun = new PruningTable(XCenterSymCoord, rawCoord, null, 7, 1 << 26, "Phase1CenterSym");
	}

	public class Phase1Node : Node {
		public int tCenter;
		public int xCenter;
		public override int getPrun() {
			return Math.Max(
			           Math.Max(
			               TCenterSymPrun.getPrun(tCenter >> 4),
			               XCenterSymPrun.getPrun(xCenter >> 4)),
			           CenterSymPrun.getPrun(xCenter >> 4, TCenterSym2RawF[tCenter & ~0xf | CubieCube.SymMultInv[tCenter & 0xf, xCenter & 0xf]]));
		}
		public override int doMovePrun(Node node0, int move, int maxl) {
			Phase1Node node = (Phase1Node) node0;

			tCenter = TCenterSymMove[node.tCenter >> 4, SymMove[node.tCenter & 0xf, move]];
			tCenter = tCenter & ~0xf | CubieCube.SymMult[tCenter & 0xf, node.tCenter & 0xf];

			xCenter = XCenterSymMove[node.xCenter >> 4, SymMove[node.xCenter & 0xf, move]];
			xCenter = xCenter & ~0xf | CubieCube.SymMult[xCenter & 0xf, node.xCenter & 0xf];

			return getPrun();
		}
	}

	public Phase1Search() {
		base.VALID_MOVES = VALID_MOVES;
		base.MIN_BACK_DEPTH = 5;
		for(int i = 0; i < searchNode.Length; i++) {
			searchNode[i] = new Phase1Node();
		}
	}

	public override Node[] initFrom(CubieCube cc) {
		SymMove ??= CubieCube.getSymMove(VALID_MOVES, 16);
		Phase1Center ct = new();
		for(int i = 0; i < 24; i++) {
			ct.xCenter[i] = cc.xCenter[i] == 1 || cc.xCenter[i] == 4 ? 0 : -1;
			ct.tCenter[i] = cc.tCenter[i] == 1 || cc.tCenter[i] == 4 ? 0 : -1;
		}
		Phase1Node node = new();
		node.xCenter = XCenterRaw2Sym[ct.getXCenter()];
		node.tCenter = TCenterRaw2Sym[ct.getTCenter()];
		return [node];
	}
}