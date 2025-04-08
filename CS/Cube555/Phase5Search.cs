using System;

using static CS.Cube555.Util;
namespace CS.Cube555;

public class Phase5Search : PhaseSearch {
	public static new int[] VALID_MOVES = [
	    Ux1, Ux2, Ux3, Rx2, Fx2, Dx1, Dx2, Dx3, Lx2, Bx2,
	    rx2, fx2, lx2, bx2
	];

	public static new ulong[] SKIP_MOVES = genSkipMoves(VALID_MOVES);

	public static int[, ] LEdgeMove;
	public static int[] LEdgeSym2Raw;
	public static int[] LEdgeSelfSym;
	public static int[] LEdgeRaw2Sym;
	public static int[, ] LEdgeSymMove;
	public static int[] LEdgeMirror;
	public static int[] UDCenterMirror;
	public static int[, ] CenterMove;
	public static int[, ] UDCenterConj;
	public static PruningTable CenterPrun;
	public static PruningTable LEdgeSymCenterPrun;

	public static void init() {
		initLEdgeSymMove();
		initCenterMove();
		initEdgeMove();
		initPrun();
	}

	public static void initEdgeMove() {
		LEdgeMove = new int[40320, VALID_MOVES.Length];
		Phase5Edge edge = new();
		for(int i = 0; i < 40320; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				edge.setLEdge(i);
				edge.doMove(m);
				LEdgeMove[i, m] = edge.getLEdge();
			}
		}
	}

	public static void initLEdgeSymMove() {
		Phase5Edge edge = new();

		LEdgeMirror = new int[40320];
		for(int i = 0; i < LEdgeMirror.Length; i++) {
			edge.setLEdge(i);
			edge.doConj(2);
			LEdgeMirror[i] = edge.getHEdge();
		}

		int symCnt = 0;
		LEdgeSym2Raw = new int[5288 * 8];
		LEdgeSelfSym = new int[5288];
		LEdgeRaw2Sym = new int[40320];
		for(int i = 0; i < LEdgeRaw2Sym.Length; i++) {
			if (LEdgeRaw2Sym[i] != 0) {
				continue;
			}
			edge.setLEdge(i);
			for(int sym = 0; sym < 8; sym++) {
				int idx = edge.getLEdge();
				LEdgeRaw2Sym[idx] = symCnt << 3 | sym;
				if (idx == i) {
					LEdgeSelfSym[symCnt] |= 1 << sym;
				}
				edge.doConj(0);
				if ((sym & 3) == 3) {
					edge.doConj(1);
				}
			}
			LEdgeSym2Raw[symCnt] = i;
			symCnt++;
		}
		LEdgeSymMove = new int[symCnt, VALID_MOVES.Length];
		for(int i = 0; i < symCnt; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				edge.setLEdge(LEdgeSym2Raw[i]);
				edge.doMove(m);
				LEdgeSymMove[i, m] = LEdgeRaw2Sym[edge.getLEdge()];
			}
		}
	}

	public static void initCenterMove() {
		int[, ] RFLBMove = new int[36, VALID_MOVES.Length];
		int[, ] TMove = new int[70, VALID_MOVES.Length];
		int[, ] XMove = new int[70, VALID_MOVES.Length];
		int[, ] TConj = new int[70, 8];
		int[, ] XConj = new int[70, 8];
		Phase5Center center = new();
		for(int i = 0; i < 70; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				center.setTCenter(i);
				center.setXCenter(i);
				center.doMove(m);
				TMove[i, m] = center.getTCenter();
				XMove[i, m] = center.getXCenter();
			}
			center.setTCenter(i);
			center.setXCenter(i);
			for(int sym = 0; sym < 8; sym++) {
				TConj[i, CubieCube.SymMultInv[0, sym]] = center.getTCenter();
				XConj[i, CubieCube.SymMultInv[0, sym]] = center.getXCenter();
				center.doConj(0);
				if ((sym & 3) == 3) {
					center.doConj(1);
				}
			}
		}
		for(int i = 0; i < 36; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				center.setRFLBCenter(i);
				center.doMove(m);
				RFLBMove[i, m] = center.getRFLBCenter();
			}
		}

		CenterMove = new int[70 * 70 * 36, VALID_MOVES.Length];
		for(int i = 0; i < 70 * 70 * 36; i++) {
			int tCenter = i % 70;
			int xCenter = i / 70 % 70;
			int rflbCenter = i / 70 / 70;
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				CenterMove[i, m] = (RFLBMove[rflbCenter, m] * 70 + XMove[xCenter, m]) * 70 + TMove[tCenter, m];
			}
		}

		UDCenterMirror = new int[4900];
		UDCenterConj = new int[70 * 70, 8];
		for(int i = 0; i < 4900; i++) {
			int tCenter = i % 70;
			int xCenter = i / 70 % 70;
			center.setTCenter(tCenter);
			center.setXCenter(xCenter);
			center.doConj(2);
			UDCenterMirror[i] = center.getXCenter() * 70 + center.getTCenter();
			for(int s = 0; s < 8; s++) {
				UDCenterConj[i, s] = XConj[xCenter, s] * 70 + TConj[tCenter, s];
			}
		}
	}

	public static void initPrun() {
		int[, ] UDCenterMove = new int[4900, VALID_MOVES.Length];
		for(int i = 0; i < 4900; i++) {
			for(int j = 0; j < VALID_MOVES.Length; j++) {
				UDCenterMove[i, j] = CenterMove[i, j] % 4900;
			}
		}
		CenterPrun = new PruningTable(CenterMove, null, "Phase5Center");
		LEdgeSymCenterPrun = new PruningTable(
		    new TableSymCoord(LEdgeSymMove, LEdgeSelfSym, 8),
		    new TableRawCoord(UDCenterMove, UDCenterConj),
		    null, "Phase5LEdgeSymCenter");
	}

	public class Phase5Node : Node {
		public int lEdge;
		public int hEdgem;
		public int center;
		public override int getPrun() {
			int lEdges = LEdgeRaw2Sym[lEdge];
			int hEdges = LEdgeRaw2Sym[hEdgem];
			return Math.Max(
			           CenterPrun.getPrun(center),
			           Math.Max(LEdgeSymCenterPrun.getPrun(lEdges >> 3, UDCenterConj[center % 4900, lEdges & 0x7]),
			                    LEdgeSymCenterPrun.getPrun(hEdges >> 3, UDCenterConj[UDCenterMirror[center % 4900], hEdges & 0x7]))
			       );
		}
		public override int doMovePrun(Node node0, int move, int maxl) {
			Phase5Node node = (Phase5Node) node0;
			center = CenterMove[node.center, move];
			lEdge = LEdgeMove[node.lEdge, move];
			hEdgem = LEdgeMove[node.hEdgem, SymMove[8, move]];
			return getPrun();
		}
	}

	public static int[,] SymMove;

	public Phase5Search() {
		base.VALID_MOVES = VALID_MOVES;
		for(int i = 0; i < searchNode.Length; i++) {
			searchNode[i] = new Phase5Node();
		}
	}

	public override Node[] initFrom(CubieCube cc) {
		SymMove ??= CubieCube.getSymMove(VALID_MOVES, 16);
		Phase5Edge edge = new();
		Phase5Center center = new();
		int mask = 0;
		for(int i = 0; i < 8; i++) {
			mask |= 1 << (cc.mEdge[i] >> 1);
		}
		for(int i = 0; i < 8; i++) {
			int e = cc.mEdge[i] >> 1;
			edge.mEdge[i] = BitCount(mask & ((1 << e) - 1));
			e = cc.wEdge[i] % 12;
			edge.lEdge[i] = BitCount(mask & ((1 << e) - 1));
			e = cc.wEdge[i + 12] % 12;
			edge.hEdge[i] = BitCount(mask & ((1 << e) - 1));
			center.xCenter[i] = cc.xCenter[i] == 0 ? 0 : -1;
			center.tCenter[i] = cc.tCenter[i] == 0 ? 0 : -1;
			center.rflbCenter[i] = cc.tCenter[9 + i * 2] == 1 || cc.tCenter[9 + i * 2] == 2 ? 0 : -1;
		}
		edge.isStd = false;
		Phase5Node node = new();
		node.lEdge = edge.getLEdge();
		node.hEdgem = LEdgeMirror[edge.getHEdge()];
		node.center = (center.getRFLBCenter() * 70 + center.getXCenter()) * 70 + center.getTCenter();
		return [node];
	}
}