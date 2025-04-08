using System;
using System.Collections.Generic;
using static CS.Cube555.Util;
namespace CS.Cube555;

public class PhaseSearch {
	public SolutionChecker callback = null;
	public int[] solution = new int[255];
	public int ccidx;

	public class PhaseEntry : IComparable<PhaseEntry> {
		public Node node;
		public int prun;
		public int cumCost;
		public int estCost;
		public int ccidx;

		public int CompareTo(PhaseEntry entry) {
			if (this == entry) {
				return 0;
			}
			if (estCost != entry.estCost) {
				return estCost - entry.estCost;
			}
			if (cumCost != entry.cumCost) {
				return cumCost - entry.cumCost;
			}
			return 1;
		}
	}

	public void solve(SolvingCube[] cc, SolutionChecker callback) {
		solve(cc, callback, int.MaxValue);
	}

	public void solve(SolvingCube[] cc, SolutionChecker callback, int trySize) {
		if (SKIP_MOVES == null) {
			SKIP_MOVES = genSkipMoves(VALID_MOVES);
			NEXT_AXIS = genNextAxis(VALID_MOVES);
		}
		this.callback = callback;
		long startTime = Util.SystemNanoTime();

		SortedSet<PhaseEntry> entries = new();
		for (ccidx = 0; ccidx < cc.Length; ccidx++) {
			Node[] nodes = initFrom(cc[ccidx]);
			int cumCost = cc[ccidx].length();
			for(int i = 0; i < nodes.Length; i++) {
				PhaseEntry entry = new();
				entry.node = nodes[i];
				entry.prun = nodes[i].getPrun();
				entry.cumCost = cumCost;
				entry.estCost = cumCost + entry.prun;
				entry.ccidx = ccidx;
				entries.Add(entry);
				if (entries.Count > trySize) {
					entries.Remove(entries.Max); // TOCHECK
				}
			}
		}
		// nodeCnt = 0;
		for(int maxl = 0; maxl < 100; maxl++) {
			foreach (PhaseEntry entry in entries) {
				ccidx = entry.ccidx;
				if (maxl >= entry.estCost &&
				        idaSearch(entry.node, 0, maxl - entry.cumCost, VALID_MOVES.Length, entry.prun) == 0) {
					return;
				}
			}
		}
		// Console.WriteLine(nodeCnt);
	}

	public virtual Node[] initFrom(CubieCube cc) {
		return null;
	}


	public Node[] searchNode = new Node[30];
	// public static int nodeCnt = 0;

	private int idaSearch(Node node, int depth, int maxl, int lm, int prun) {
		if (prun == 0 && node.isSolved() && maxl < MIN_BACK_DEPTH) {
			return maxl != 0 ? 1 : callback.check(solution, depth, ccidx);
		}
		ulong skipMoves = SKIP_MOVES[lm];
		for(int move = 0; move < VALID_MOVES.Length; move++) {
			if ((skipMoves >> move & 1) != 0) {
				continue;
			}
			// nodeCnt++;
			prun = searchNode[depth].doMovePrun(node, move, maxl);
			if (prun >= maxl) {
				move += (int)(NEXT_AXIS >> move & 3UL & (ulong)(maxl - prun));
				continue;
			}
			solution[depth] = VALID_MOVES[move];
			int ret = idaSearch(searchNode[depth], depth + 1, maxl - 1, move, prun);
			if (ret == 0) {
				return 0;
			}
		}
		return 1;
	}

	public ulong[] SKIP_MOVES;
	public int[] VALID_MOVES;
	public ulong NEXT_AXIS;
	public int MIN_BACK_DEPTH = 1;
}


public abstract class Node {
	/**
		*  other requirements besides getPrun() == 0
		*/
	public bool isSolved() {
		return true;
	}
	public abstract int doMovePrun(Node node, int move, int maxl);
	public abstract int getPrun();
}