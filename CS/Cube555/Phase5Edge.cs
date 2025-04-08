using System;

using static CS.Cube555.Util;
namespace CS.Cube555;

/*
		13	1
	4			17
	16			5
		0	12

		15	3
	7			18
	19			6
		2	14
 */

public class Phase5Edge {

	public int[] mEdge = new int[8];
	public int[] lEdge = new int[8]; // 0 ~ 7
	public int[] hEdge = new int[8]; // 12 ~ 19
	public bool isStd = true;

	public Phase5Edge() {
		for(int i = 0; i < 8; i++) {
			mEdge[i] = i;
			hEdge[i] = i;
			lEdge[i] = i;
		}
	}

	public void setLEdge(int idx) {
		standardlize();
		setPerm(lEdge, idx);
	}

	public void setHEdge(int idx) {
		standardlize();
		setPerm(hEdge, idx);
	}

	public int getLEdge() {
		standardlize();
		return getPerm(lEdge);
	}

	public int getHEdge() {
		standardlize();
		return getPerm(hEdge);
	}

	public void standardlize() {
		if (isStd) {
			return;
		}
		int[] mEdgeInv = new int[8];
		for(int i = 0; i < 8; i++) {
			mEdgeInv[mEdge[i]] = i;
		}

		for(int i = 0; i < 8; i++) {
			mEdge[i] = i;
			lEdge[i] = mEdgeInv[lEdge[i]];
			hEdge[i] = mEdgeInv[hEdge[i]];
		}
		isStd = true;
	}

	public void doMove(int move) {
		move = Phase5Search.VALID_MOVES[move];
		int pow = move % 3;
		isStd = false;
		switch (move) {
		case Ux1:
		case Ux2:
		case Ux3:
			swap(mEdge, 0, 4, 1, 5, pow);
			swap(lEdge, 0, 4, 1, 5, pow);
			swap(hEdge, 0, 4, 1, 5, pow);
			break;
		case rx2:
			swap(lEdge, 1, 3);
			swap(hEdge, 0, 2);
			goto case Rx2;
		case Rx2:
			swap(mEdge, 5, 6);
			swap(lEdge, 5, 6);
			swap(hEdge, 5, 6);
			break;
		case fx2:
			swap(lEdge, 5, 7);
			swap(hEdge, 4, 6);
			goto case Fx2;
		case Fx2:
			swap(mEdge, 0, 3);
			swap(lEdge, 0, 3);
			swap(hEdge, 0, 3);
			break;
		case Dx1:
		case Dx2:
		case Dx3:
			swap(mEdge, 2, 7, 3, 6, pow);
			swap(lEdge, 2, 7, 3, 6, pow);
			swap(hEdge, 2, 7, 3, 6, pow);
			break;
		case lx2:
			swap(lEdge, 0, 2);
			swap(hEdge, 1, 3);
			goto case Lx2;
		case Lx2:
			swap(mEdge, 4, 7);
			swap(lEdge, 4, 7);
			swap(hEdge, 4, 7);
			break;
		case bx2:
			swap(lEdge, 4, 6);
			swap(hEdge, 5, 7);
			goto case Bx2;
		case Bx2:
			swap(mEdge, 1, 2);
			swap(lEdge, 1, 2);
			swap(hEdge, 1, 2);
			break;
		}
	}

	public void doConj(int conj) {
		isStd = false;
		switch (conj) {
		case 0: //y
			swap(mEdge, 0, 4, 1, 5, 0);
			swap(lEdge, 0, 4, 1, 5, 0);
			swap(hEdge, 0, 4, 1, 5, 0);
			swap(mEdge, 2, 7, 3, 6, 2);
			swap(lEdge, 2, 7, 3, 6, 2);
			swap(hEdge, 2, 7, 3, 6, 2);
			break;
		case 1: //x2
			swap(mEdge, 0, 2);
			swap(lEdge, 0, 2);
			swap(hEdge, 0, 2);
			swap(mEdge, 1, 3);
			swap(lEdge, 1, 3);
			swap(hEdge, 1, 3);
			swap(mEdge, 4, 7);
			swap(hEdge, 4, 7);
			swap(lEdge, 4, 7);
			swap(mEdge, 5, 6);
			swap(hEdge, 5, 6);
			swap(lEdge, 5, 6);
			break;
		case 2: //lr mirror
			for(int i = 0; i < 8; i++) {
				int tmp = lEdge[i];
				lEdge[i] = hEdge[i];
				hEdge[i] = tmp;
			}
			swap(mEdge, 4, 5);
			swap(lEdge, 4, 5);
			swap(hEdge, 4, 5);
			swap(mEdge, 6, 7);
			swap(lEdge, 6, 7);
			swap(hEdge, 6, 7);
			break;
		}
	}
}