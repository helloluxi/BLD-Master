using System;
using System.Collections.Generic;
using System.Text;
using static CS.Cube555.Util;
namespace CS.Cube555;

public class SolvingCube : CubieCube {

	private List<int> solution = new(60);
	private int conjIdx = 0;
	private int moveCost = 0;

	public SolvingCube() {}

	public SolvingCube(CubieCube cc) {
		copy(cc);
	}

	public override void copy(CubieCube cc) {
		base.copy(cc);
        if (cc is SolvingCube sc)
        {
            this.conjIdx = sc.conjIdx;
            this.moveCost = sc.moveCost;
            this.solution.Clear();
            this.solution.AddRange(sc.solution);
        }
        else
        {
            this.conjIdx = 0;
            this.moveCost = 0;
            this.solution.Clear();
        }
    }

	public override void doMove(params int[] moves) {
		base.doMove(moves);
		for(int i = 0; i < moves.Length; i++) {
			solution.Add(CubieCube.SymMove[conjIdx, moves[i]]);
		}
		moveCost += moves.Length;
	}

	public override void doConj(int idx) {
		base.doConj(idx);
		conjIdx = CubieCube.SymMult[conjIdx, idx];
	}

	public int[] getSolution() {
		int[] ret = new int[moveCost];
		int i = 0;
		foreach(int m in solution) {
			if (m != -1) {
				ret[i++] = m;
			}
		}
		return ret;
	}

	public void addCheckPoint() {
		solution.Add(-1);
	}

	public int length() {
		return moveCost;
	}

	public string toSolutionString(int verbose) {
		StringBuilder sb = new();
		int[] moves;
		if ((verbose & Search.INVERSE_SOLUTION) == 0) {
			moves = [.. solution];
		} else {
			moves = new int[solution.Count];
			for(int i = 0; i < solution.Count; i++) {
				moves[solution.Count - 1 - i] = solution[i] == -1 ? -1 : solution[i] / 3 * 3 + (2 - solution[i] % 3);
			}
		}
		foreach(int move in moves) {
			if (move == -1) {
				if ((verbose & Search.USE_SEPARATOR) != 0) {
					sb.Append(".  ");
				}
				continue;
			}
			sb.Append(move2str[move]).Append(' ');
		}
		return sb.ToString();
	}

	public override string ToString() {
		StringBuilder sb = new();
		int cnt = 0;
		int cumcnt = 0;
		foreach(int move in solution) {
			if (move == -1) {
				cumcnt += cnt;
				sb.Append($"//({cnt}f,cum={cumcnt}f)\n");
				cnt = 0;
				continue;
			}
			cnt++;
			sb.Append(move2str[move]).Append(' ');
		}
		sb.Append($"//conjIdx={conjIdx}\n");
		sb.Append(base.ToString());
		return sb.ToString();
	}
}