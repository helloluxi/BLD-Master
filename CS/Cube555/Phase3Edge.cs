using System;
using System.Text;
using static CS.Cube555.Util;
namespace CS.Cube555;

/*
 					13	1
				4			17
				16			5
					0	12
	4	16			0	12			5	17			1	13
9			20	20			11	11			22	22			9
21			8	8			23	23			10	10			21
	19	7			15	3			18	6			14	2
					15	3
				7			18
				19			6
					2	14
 */

public class Phase3Edge {

	public int[] mEdge = new int[12];
	public int[] wEdge = new int[24];
 
	public Phase3Edge() {
		for(int i = 0; i < 12; i++) {
			mEdge[i] = 0;
			wEdge[i] = 0;
			wEdge[i + 12] = -1;
		}
	}

	public override string ToString() {
		StringBuilder sb = new();
		for(int i = 0; i < 12; i++) {
			sb.Append(mEdge[i]).Append(' ');
		}
		sb.Append('|');
		for(int i = 0; i < 24; i++) {
			sb.Append(wEdge[i]).Append(' ');
		}
		return sb.ToString();
	}

	public void setMEdge(int idx) {
		int parity = 0;
		for(int i = 0; i < 11; i++) {
			mEdge[i] = idx & 1;
			idx >>= 1;
			parity ^= mEdge[i];
		}
		mEdge[11] = parity;
	}

	public int getMEdge() {
		int idx = 0;
		for(int i = 0; i < 11; i++) {
			idx |= mEdge[i] << i;
		}
		return idx;
	}

	public void setWEdge(int idx) {
		setComb(wEdge, idx, 12);
	}

	public int getWEdge() {
		return getComb(wEdge, 12);
	}

	public void doMove(int move) {
		move = Phase3Search.VALID_MOVES[move];
		int pow = move % 3;
		switch (move) {
		case ux2:
			swap(wEdge, 9, 22, 11, 20, pow);
			goto case Ux1;
		case Ux1:
		case Ux2:
		case Ux3:
			swap(mEdge, 0, 4, 1, 5, pow);
			swap(wEdge, 0, 4, 1, 5, pow);
			swap(wEdge, 12, 16, 13, 17, pow);
			break;
		case rx2:
			swap(wEdge, 1, 14, 3, 12, pow);
			goto case Rx1;
		case Rx1:
		case Rx2:
		case Rx3:
			swap(mEdge, 5, 10, 6, 11, pow, true);
			swap(wEdge, 5, 22, 6, 23, pow);
			swap(wEdge, 17, 10, 18, 11, pow);
			break;
		case fx2:
			swap(wEdge, 5, 18, 7, 16, pow);
			goto case Fx1;
		case Fx1:
		case Fx2:
		case Fx3:
			swap(mEdge, 0, 11, 3, 8, pow);
			swap(wEdge, 0, 11, 3, 8, pow);
			swap(wEdge, 12, 23, 15, 20, pow);
			break;
		case dx2:
			swap(wEdge, 8, 23, 10, 21, pow);
			goto case Dx1;
		case Dx1:
		case Dx2:
		case Dx3:
			swap(mEdge, 2, 7, 3, 6, pow);
			swap(wEdge, 2, 7, 3, 6, pow);
			swap(wEdge, 14, 19, 15, 18, pow);
			break;
		case lx2:
			swap(wEdge, 0, 15, 2, 13, pow);
			goto case Lx1;
		case Lx1:
		case Lx2:
		case Lx3:
			swap(mEdge, 4, 8, 7, 9, pow, true);
			swap(wEdge, 4, 20, 7, 21, pow);
			swap(wEdge, 16, 8, 19, 9, pow);
			break;
		case bx2:
			swap(wEdge, 4, 19, 6, 17, pow);
			goto case Bx1;
		case Bx1:
		case Bx2:
		case Bx3:
			swap(mEdge, 1, 9, 2, 10, pow);
			swap(wEdge, 1, 9, 2, 10, pow);
			swap(wEdge, 13, 21, 14, 22, pow);
			break;
		}
	}

	public void doConj(int conj) {
		switch (conj) {
		case 0: //x
			swap(wEdge, 1, 14, 3, 12, 0);
			swap(wEdge, 5, 22, 6, 23, 0);
			swap(wEdge, 17, 10, 18, 11, 0);
			swap(wEdge, 0, 15, 2, 13, 2);
			swap(wEdge, 4, 20, 7, 21, 2);
			swap(wEdge, 16, 8, 19, 9, 2);
			swap(mEdge, 5, 10, 6, 11, 0, true);
			swap(mEdge, 4, 8, 7, 9, 2, true);
			swap(mEdge, 0, 1, 2, 3, 0, true);
			for(int i = 0; i < 12; i++) {
				mEdge[i] ^= 1;
				wEdge[i] = -1 - wEdge[i];
				wEdge[i + 12] = -1 - wEdge[i + 12];
			}
			break;
		case 1: //y2
			swap(wEdge, 9, 22, 11, 20, 1);
			swap(wEdge, 0, 4, 1, 5, 1);
			swap(wEdge, 12, 16, 13, 17, 1);
			swap(wEdge, 8, 23, 10, 21, 1);
			swap(wEdge, 2, 7, 3, 6, 1);
			swap(wEdge, 14, 19, 15, 18, 1);
			swap(mEdge, 0, 4, 1, 5, 1);
			swap(mEdge, 2, 7, 3, 6, 1);
			swap(mEdge, 8, 9, 10, 11, 1, true);
			break;
		case 2: //lr2
			swap(wEdge, 0, 12);
			swap(wEdge, 1, 13);
			swap(wEdge, 2, 14);
			swap(wEdge, 3, 15);
			swap(wEdge, 4, 17);
			swap(wEdge, 5, 16);
			swap(wEdge, 6, 19);
			swap(wEdge, 7, 18);
			swap(wEdge, 8, 23);
			swap(wEdge, 9, 22);
			swap(wEdge, 10, 21);
			swap(wEdge, 11, 20);
			swap(mEdge, 4, 5);
			swap(mEdge, 6, 7);
			swap(mEdge, 8, 11);
			swap(mEdge, 9, 10);
			for(int i = 0; i < 24; i++) {
				wEdge[i] = -1 - wEdge[i];
			}
			break;
		case 3: //change lh edges
			for(int i = 0; i < 24; i++) {
				wEdge[i] = -1 - wEdge[i];
			}
			for(int i = 0; i < 12; i++) {
				mEdge[i] ^= 1;
			}
			break;
		}
	}
}