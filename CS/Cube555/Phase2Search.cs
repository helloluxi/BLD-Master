using System;
using static CS.Cube555.Util;
namespace CS.Cube555;

public class Phase2Search : PhaseSearch {

	public new static int[] VALID_MOVES = [
	    Ux1, Ux2, Ux3, Fx1, Fx2, Fx3, Dx1, Dx2, Dx3, Bx1, Bx2, Bx3,
	    ux2, rx1, rx2, rx3, fx2, dx2, lx1, lx2, lx3, bx2
	];

	public new static ulong[] SKIP_MOVES = genSkipMoves(VALID_MOVES);

	public static int[, ] TCenterMove;
	public static int[, ] XCenterMove;
	static PruningTable prunTCenter;
	static PruningTable prunXCenter;

	public static void init() {
		initCenterMove();
		initCenterPrun();
	}

	public static void initCenterMove() {
		Phase2Center ct = new();
		TCenterMove = new int[12870, VALID_MOVES.Length];
		XCenterMove = new int[12870, VALID_MOVES.Length];
		for(int i = 0; i < 12870; i++) {
			for(int m = 0; m < VALID_MOVES.Length; m++) {
				ct.setTCenter(i);
				ct.setXCenter(i);
				ct.doMove(m);
				TCenterMove[i, m] = ct.getTCenter();
				XCenterMove[i, m] = ct.getXCenter();
			}
		}
	}

	public static void initCenterPrun() {
		int[, ] EParityMove = new int[2, VALID_MOVES.Length];
		for(int i = 0; i < VALID_MOVES.Length; i++) {
			EParityMove[0, i] = 0 ^ Phase2Center.eParityDiff[i];
			EParityMove[1, i] = 1 ^ Phase2Center.eParityDiff[i];
		}
		prunTCenter = new PruningTable(TCenterMove, EParityMove, null, null, "Phase2TCenter");
		prunXCenter = new PruningTable(XCenterMove, EParityMove, null, null, "Phase2XCenter");
	}

	public class Phase2Node : Node {
		public int tCenter;
		public int xCenter;
		public int eParity;
		public override int getPrun() {
			return Math.Max(prunTCenter.getPrun(tCenter, eParity),
			                prunXCenter.getPrun(xCenter, eParity));
		}
		public override int doMovePrun(Node node0, int move, int maxl) {
			Phase2Node node = (Phase2Node) node0;
			tCenter = TCenterMove[node.tCenter, move];
			xCenter = XCenterMove[node.xCenter, move];
			eParity = node.eParity ^ Phase2Center.eParityDiff[move];
			return getPrun();
		}
	}

	public Phase2Search() {
		base.VALID_MOVES = VALID_MOVES;
		base.MIN_BACK_DEPTH = 5;
		for(int i = 0; i < searchNode.Length; i++) {
			searchNode[i] = new Phase2Node();
		}
	}

	public override Node[] initFrom(CubieCube cc) {
		Phase2Center ct = new();
		for(int i = 0; i < 16; i++) {
			ct.xCenter[i] = cc.xCenter[i] == 0 || cc.xCenter[i] == 3 ? 0 : -1;
			ct.tCenter[i] = cc.tCenter[i] == 0 || cc.tCenter[i] == 3 ? 0 : -1;
		}
		Phase2Node node = new();
		node.xCenter = ct.getXCenter();
		node.tCenter = ct.getTCenter();
		node.eParity = getParity(cc.wEdge);
		return [node];
	}
}