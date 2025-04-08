using System;
using System.Collections.Generic;
using static CS.Cube555.Util;
namespace CS.Cube555;

public class Phase3Search : PhaseSearch {

	public new static int[] VALID_MOVES = [
	    Ux1, Ux2, Ux3, Rx1, Rx2, Rx3, Fx1, Fx2, Fx3, Dx1, Dx2, Dx3, Lx1, Lx2, Lx3, Bx1, Bx2, Bx3,
	    ux2, rx2, fx2, dx2, lx2, bx2
	];

	public static int[, ] SymMove;

	public static new ulong[] SKIP_MOVES = genSkipMoves(VALID_MOVES);
	public static new int NEXT_AXIS = 0x12492;

	public static int[, ] CenterMove;
	public static int[, ] MEdgeMove;
	public static int[, ] MEdgeConj;
	static PruningTable CenterMEdgePrun;

	public static int[, ] WEdgeSymMove;
	public static int[] WEdgeSym2Raw;
	public static int[] WEdgeSelfSym;
	public static int[] WEdgeRaw2Sym;

	public static void init() {
		initWEdgeSymMove();
		initMEdgeMove();
		initCenterMove();
		initPrun();
	}

	public static void initWEdgeSymMove() {
		Phase3Edge edge = new();
		int symCnt = 0;
		WEdgeSym2Raw = new int[86048];
		WEdgeSelfSym = new int[86048];
		WEdgeRaw2Sym = new int[2704156];
		for(int i = 0; i < WEdgeRaw2Sym.Length; i++) {
			if (WEdgeRaw2Sym[i] != 0) {
				continue;
			}
			edge.setWEdge(i);
			for(int sym = 0; sym < 32; sym++) {
				int idx = edge.getWEdge();
				WEdgeRaw2Sym[idx] = symCnt << 5 | sym;
				if (idx == i) {
					WEdgeSelfSym[symCnt] |= 1 << sym;
				}
				edge.doConj(0);
				if ((sym & 3) == 3) {
					edge.doConj(1);
				}
				if ((sym & 7) == 7) {
					edge.doConj(2);
				}
				if ((sym & 0xf) == 0xf) {
					edge.doConj(3);
				}
			}
			WEdgeSym2Raw[symCnt] = i;
			symCnt++;
		}
		WEdgeSymMove = new int[symCnt, VALID_MOVES.Length];
		for(int i = 0; i < symCnt; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				edge.setWEdge(WEdgeSym2Raw[i]);
				edge.doMove(m);
				WEdgeSymMove[i, m] = WEdgeRaw2Sym[edge.getWEdge()];
			}
		}
	}

	public static void initMEdgeMove() {
		Phase3Edge edge = new();
		MEdgeMove = new int[2048, VALID_MOVES.Length];
		MEdgeConj = new int[2048, 32];
		for(int i = 0; i < 2048; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				edge.setMEdge(i);
				edge.doMove(m);
				MEdgeMove[i, m] = edge.getMEdge();
			}

			edge.setMEdge(i);
			for(int sym = 0; sym < 32; sym++) {
				// Console.WriteLine("Idx:" + (CubieCube.SymMultInv[0, sym & 0xf] | sym & 0x10));
				MEdgeConj[i, CubieCube.SymMultInv[0, sym & 0xf] | sym & 0x10] = edge.getMEdge();
				edge.doConj(0);
				if ((sym & 3) == 3) {
					edge.doConj(1);
				}
				if ((sym & 7) == 7) {
					edge.doConj(2);
				}
				if ((sym & 0xf) == 0xf) {
					edge.doConj(3);
				}
			}
		}
	}

	public static void initCenterMove() {
		Phase3Center center = new();
		CenterMove = new int[1225, VALID_MOVES.Length];
		for(int i = 0; i < 1225; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				center.setCenter(i);
				center.doMove(m);
				CenterMove[i, m] = center.getCenter();
			}
		}
	}

	static PruningTable WMEdgeSymPrun;

	public static void initPrun() {
		CenterMEdgePrun = new PruningTable(
		    CenterMove, MEdgeMove,
		    Phase3Center.SOLVED_CENTER, [0, 2047],
		    "Phase3CenterMEdge");

		int[] mEdgeFlip = new int[1];
		
		var symCoord = new SymCoord() {
			N_IDX = 86048,
			N_MOVES = VALID_MOVES.Length,
			N_SYM = 16,
			SelfSym = WEdgeSelfSym,
		};
		symCoord.getMoved = (move) => {
			int val = WEdgeSymMove[symCoord.idx, move];
			mEdgeFlip[0] = (val & 0x10) == 0 ? 0 : 0x7ff;
			return val >> 1 & ~0xf | val & 0xf;
		};
		var rawCoord = new RawCoord() {
			N_IDX = 2048,
		};
		rawCoord.getMoved = (move) => {
			return MEdgeMove[rawCoord.idx, move] ^ mEdgeFlip[0];
		};
		rawCoord.getConj = (idx, conj) => {
			return MEdgeConj[idx, conj];
		};
		WMEdgeSymPrun = new PruningTable(symCoord, rawCoord, null, "Phase3MWEdgeSym");
	}

	public class Phase3Node : Node {
		public int center;
		public int mEdge;
		public int wEdge;
		public override int getPrun() {
			return Math.Max(
			           CenterMEdgePrun.getPrun(center, mEdge),
			           WMEdgeSymPrun.getPrun(wEdge >> 5, MEdgeConj[mEdge, wEdge & 0x1f]));
		}
		public override int doMovePrun(Node node0, int move, int maxl) {
			Phase3Node node = (Phase3Node) node0;
			center = CenterMove[node.center, move];
			mEdge = MEdgeMove[node.mEdge, move];
			wEdge = WEdgeSymMove[node.wEdge >> 5, SymMove[node.wEdge & 0xf, move]] ^ (node.wEdge & 0x10);
			wEdge = wEdge & ~0xf | CubieCube.SymMult[wEdge & 0xf, node.wEdge & 0xf];
			return getPrun();
		}
	}

	public Phase3Search() {
		base.VALID_MOVES = VALID_MOVES;
		for(int i = 0; i < searchNode.Length; i++) {
			searchNode[i] = new Phase3Node();
		}
	}

	public override Node[] initFrom(CubieCube cc) {
		SymMove ??= CubieCube.getSymMove(VALID_MOVES, 16);
		Phase3Center ct = new();
		Phase3Edge ed = new();
		for(int i = 0; i < 8; i++) {
			ct.xCenter[i] = cc.xCenter[16 + (i & 4) + (i + 1) % 4] == 1 ? 0 : -1;
			ct.tCenter[i] = cc.tCenter[16 + (i & 4) + (i + 1) % 4] == 1 ? 0 : -1;
		}
		int center = ct.getCenter();
		var nodes = new List<Node>();
		for(int filter = 8; nodes.Count == 0; filter++) {
			for(int idx = 0; idx < 1024; idx++) {
				Phase3Node node = new();
				int flip = idx << 1 | (BitCount(idx) & 1);
				flip = (flip ^ 0xfff) << 12 | flip;
				for(int i = 0; i < 12; i++) {
					ed.mEdge[i] = cc.mEdge[i] & 1;
					ed.mEdge[i] ^= flip >> (cc.mEdge[i] >> 1) & 1;
				}
				for(int i = 0; i < 24; i++) {
					ed.wEdge[i] = (flip >> cc.wEdge[i] & 1) == 0 ? 0 : -1;
				}
				node.mEdge = ed.getMEdge();
				node.wEdge = WEdgeRaw2Sym[ed.getWEdge()];
				node.center = center;
				if (node.getPrun() > filter) {
					continue;
				}
				nodes.Add(node);
			}
		}
		return [.. nodes]; // TOCHECK: ToArray(new Node[0]);
	}

	public static void main(string[] args) {
		initMEdgeMove();
		initCenterMove();
		initWEdgeSymMove();
		initPrun();
	}
}