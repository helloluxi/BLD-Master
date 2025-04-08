using System;
using System.Collections.Generic;
using System.Threading;
namespace CS.Cube555;

public class Util {
#region Constants
	public const int U1 = 0;
	public const int U2 = 1;
	public const int U3 = 2;
	public const int U4 = 3;
	public const int U5 = 4;
	public const int U6 = 5;
	public const int U7 = 6;
	public const int U8 = 7;
	public const int U9 = 8;
	public const int U10 = 9;
	public const int U11 = 10;
	public const int U12 = 11;
	public const int U13 = 12;
	public const int U14 = 13;
	public const int U15 = 14;
	public const int U16 = 15;
	public const int U17 = 16;
	public const int U18 = 17;
	public const int U19 = 18;
	public const int U20 = 19;
	public const int U21 = 20;
	public const int U22 = 21;
	public const int U23 = 22;
	public const int U24 = 23;
	public const int U25 = 24;
	public const int R1 = 25;
	public const int R2 = 26;
	public const int R3 = 27;
	public const int R4 = 28;
	public const int R5 = 29;
	public const int R6 = 30;
	public const int R7 = 31;
	public const int R8 = 32;
	public const int R9 = 33;
	public const int R10 = 34;
	public const int R11 = 35;
	public const int R12 = 36;
	public const int R13 = 37;
	public const int R14 = 38;
	public const int R15 = 39;
	public const int R16 = 40;
	public const int R17 = 41;
	public const int R18 = 42;
	public const int R19 = 43;
	public const int R20 = 44;
	public const int R21 = 45;
	public const int R22 = 46;
	public const int R23 = 47;
	public const int R24 = 48;
	public const int R25 = 49;
	public const int F1 = 50;
	public const int F2 = 51;
	public const int F3 = 52;
	public const int F4 = 53;
	public const int F5 = 54;
	public const int F6 = 55;
	public const int F7 = 56;
	public const int F8 = 57;
	public const int F9 = 58;
	public const int F10 = 59;
	public const int F11 = 60;
	public const int F12 = 61;
	public const int F13 = 62;
	public const int F14 = 63;
	public const int F15 = 64;
	public const int F16 = 65;
	public const int F17 = 66;
	public const int F18 = 67;
	public const int F19 = 68;
	public const int F20 = 69;
	public const int F21 = 70;
	public const int F22 = 71;
	public const int F23 = 72;
	public const int F24 = 73;
	public const int F25 = 74;
	public const int D1 = 75;
	public const int D2 = 76;
	public const int D3 = 77;
	public const int D4 = 78;
	public const int D5 = 79;
	public const int D6 = 80;
	public const int D7 = 81;
	public const int D8 = 82;
	public const int D9 = 83;
	public const int D10 = 84;
	public const int D11 = 85;
	public const int D12 = 86;
	public const int D13 = 87;
	public const int D14 = 88;
	public const int D15 = 89;
	public const int D16 = 90;
	public const int D17 = 91;
	public const int D18 = 92;
	public const int D19 = 93;
	public const int D20 = 94;
	public const int D21 = 95;
	public const int D22 = 96;
	public const int D23 = 97;
	public const int D24 = 98;
	public const int D25 = 99;
	public const int L1 = 100;
	public const int L2 = 101;
	public const int L3 = 102;
	public const int L4 = 103;
	public const int L5 = 104;
	public const int L6 = 105;
	public const int L7 = 106;
	public const int L8 = 107;
	public const int L9 = 108;
	public const int L10 = 109;
	public const int L11 = 110;
	public const int L12 = 111;
	public const int L13 = 112;
	public const int L14 = 113;
	public const int L15 = 114;
	public const int L16 = 115;
	public const int L17 = 116;
	public const int L18 = 117;
	public const int L19 = 118;
	public const int L20 = 119;
	public const int L21 = 120;
	public const int L22 = 121;
	public const int L23 = 122;
	public const int L24 = 123;
	public const int L25 = 124;
	public const int B1 = 125;
	public const int B2 = 126;
	public const int B3 = 127;
	public const int B4 = 128;
	public const int B5 = 129;
	public const int B6 = 130;
	public const int B7 = 131;
	public const int B8 = 132;
	public const int B9 = 133;
	public const int B10 = 134;
	public const int B11 = 135;
	public const int B12 = 136;
	public const int B13 = 137;
	public const int B14 = 138;
	public const int B15 = 139;
	public const int B16 = 140;
	public const int B17 = 141;
	public const int B18 = 142;
	public const int B19 = 143;
	public const int B20 = 144;
	public const int B21 = 145;
	public const int B22 = 146;
	public const int B23 = 147;
	public const int B24 = 148;
	public const int B25 = 149;

	public const byte Ux1 = 0;
	public const byte Ux2 = 1;
	public const byte Ux3 = 2;
	public const byte Rx1 = 3;
	public const byte Rx2 = 4;
	public const byte Rx3 = 5;
	public const byte Fx1 = 6;
	public const byte Fx2 = 7;
	public const byte Fx3 = 8;
	public const byte Dx1 = 9;
	public const byte Dx2 = 10;
	public const byte Dx3 = 11;
	public const byte Lx1 = 12;
	public const byte Lx2 = 13;
	public const byte Lx3 = 14;
	public const byte Bx1 = 15;
	public const byte Bx2 = 16;
	public const byte Bx3 = 17;
	public const byte ux1 = 18;
	public const byte ux2 = 19;
	public const byte ux3 = 20;
	public const byte rx1 = 21;
	public const byte rx2 = 22;
	public const byte rx3 = 23;
	public const byte fx1 = 24;
	public const byte fx2 = 25;
	public const byte fx3 = 26;
	public const byte dx1 = 27;
	public const byte dx2 = 28;
	public const byte dx3 = 29;
	public const byte lx1 = 30;
	public const byte lx2 = 31;
	public const byte lx3 = 32;
	public const byte bx1 = 33;
	public const byte bx2 = 34;
	public const byte bx3 = 35;
	#endregion

	public static readonly string[] move2str = [
	    "U  ", "U2 ", "U' ", "R  ", "R2 ", "R' ", "F  ", "F2 ", "F' ",
	    "D  ", "D2 ", "D' ", "L  ", "L2 ", "L' ", "B  ", "B2 ", "B' ",
	    "Uw ", "Uw2", "Uw'", "Rw ", "Rw2", "Rw'", "Fw ", "Fw2", "Fw'",
	    "Dw ", "Dw2", "Dw'", "Lw ", "Lw2", "Lw'", "Bw ", "Bw2", "Bw'"
	];

	public static int[, ] Cnk = new int[25, 25];
	public static int[] fact = new int[13];

	static Util() {
		for(int i = 0; i < 25; i++) {
			Cnk[i, i] = 1;
			Cnk[i, 0] = 1;
		}
		for(int i = 1; i < 25; i++) {
			for(int j = 1; j <= i; j++) {
				Cnk[i, j] = Cnk[i - 1, j] + Cnk[i - 1, j - 1];
			}
		}
		fact[0] = 1;
		for(int i = 1; i < 13; i++) {
			fact[i] = fact[i - 1] * i;
		}
	}

	public static int BitCount(int n){
		// This is a fast implementation similar to Java's Integer.bitCount()
        n = n - ((n >> 1) & 0x55555555);
        n = (n & 0x33333333) + ((n >> 2) & 0x33333333);
        n = (n + (n >> 4)) & 0x0F0F0F0F;
        n = n + (n >> 8);
        n = n + (n >> 16);
        return n & 0x3F;
	}

	public static long SystemNanoTime() {
		return DateTime.UtcNow.Ticks * 100L;
	}

	public static ulong[] genSkipMoves(int[] VALID_MOVES) {
		ulong[] ret = new ulong[VALID_MOVES.Length + 1];
		for(int last = 0; last < VALID_MOVES.Length; last++) {
			ret[last] = 0;
			int la = VALID_MOVES[last] / 3;
			for(int move = 0; move < VALID_MOVES.Length; move++) {
				int axis = VALID_MOVES[move] / 3;
				if (axis == la || axis % 3 == la % 3 && axis >= la) {
					ret[last] |= 1UL << move;
				}
			}
		}
		return ret;
	}

	public static ulong genNextAxis(int[] VALID_MOVES) {
		ulong ret = 0;
		for(int i = 0; i < VALID_MOVES.Length; i++) {
			if (VALID_MOVES[i] % 3 == 0) {
				//if Mx1 makes state farther, Mx2 and Mx3 should be skipped
				// (next_axis >> i & 3) == 2 for Mx1, 1 for Mx2, 0 for Mx3
				ret |= 1UL << (i + 1);
			}
		}
		return ret;
	}

	public static int[] setPerm(int[] arr, int idx, int n, bool even) {
		ulong val = 0xFEDCBA9876543210UL;
		int parity = 0;
		if (even) {
			idx <<= 1;
		}
		n--;
		for(int i = 0; i < n; ++i) {
			int p = fact[n - i];
			int v = ~~(idx / p);
			parity ^= v;
			idx %= p;
			v <<= 2;
			arr[i] = (int) (val >> v & 0xf);
			ulong m = (1UL << v) - 1;
			val = (val & m) + (val >> 4 & ~m);
		}
		if (even && (parity & 1) != 0) {
			arr[n] = arr[n - 1];
			arr[n - 1] = (int) (val & 0xf);
		} else {
			arr[n] = (int) (val & 0xf);
		}
		return arr;
	}

	public static int[] setPerm(int[] arr, int idx, int n) {
		return setPerm(arr, idx, n, false);
	}

	public static int[] setPerm(int[] arr, int idx) {
		return setPerm(arr, idx, arr.Length, false);
	}

	public static int getPerm(int[] arr, int n, bool even) {
		int idx = 0;
		ulong val = 0xFEDCBA9876543210UL;
		for(int i = 0; i < n - 1; ++i) {
			int v = arr[i] << 2;
			idx = (n - i) * idx + (int) (val >> v & 0xf);
			val -= 0x1111111111111110UL << v;
		}
		return even ? (idx >> 1) : idx;
	}

	public static int getPerm(int[] arr, int n) {
		return getPerm(arr, n, false);
	}

	public static int getPerm(int[] arr) {
		return getPerm(arr, arr.Length, false);
	}

	public static int[] setComb(int[] arr, int idx, int r, int n) {
		for(int i = n - 1; i >= 0; i--) {
			if (idx >= Cnk[i, r]) {
				idx -= Cnk[i, r--];
				arr[i] = 0;
			} else {
				arr[i] = -1;
			}
		}
		return arr;
	}

	public static int[] setComb(int[] arr, int idx, int r) {
		return setComb(arr, idx, r, arr.Length);
	}

	public static int getComb(int[] arr, int r, int n) {
		int idx = 0;
		for(int i = n - 1; i >= 0; i--) {
			if (arr[i] != -1) {
				idx += Cnk[i, r--];
			}
		}
		return idx;
	}

	public static int getComb(int[] arr, int r) {
		return getComb(arr, r, arr.Length);
	}

	public static int getSComb(int[] arr, int n) {
		int idx = 0;
		int r = n / 2;
		for(int i = n - 1; i >= 0; i--) {
			if (arr[i] != arr[n - 1]) {
				idx += Cnk[i, r--];
			}
		}
		return idx;
	}

	public static int getSComb(int[] arr) {
		return getSComb(arr, arr.Length);
	}

	public static void copyFromComb(int[] src, int[] dst) {
		int r = 0;
		for(int i = 0; i < src.Length; i++) {
			if (src[i] != -1) {
				dst[r++] = src[i];
			}
		}
	}

	public static void copyToComb(int[] src, int[] dst) {
		int r = 0;
		for(int i = 0; i < dst.Length; i++) {
			if (dst[i] != -1) {
				dst[i] = src[r++];
			}
		}
	}

	public static int getParity(int idx, int n) {
		int parity = 0;
		for(int i = n - 2; i >= 0; --i) {
			parity ^= idx % (n - i);
			idx /= n - i;
		}
		return parity & 1;
	}

	public static int getParity(int[] arr) {
		int parity = 0;
		for(int i = 0; i < arr.Length - 1; i++) {
			for(int j = i + 1; j < arr.Length; j++) {
				if (arr[i] > arr[j]) {
					parity ^= 1;
				}
			}
		}
		return parity;
	}

	public static void swap(int[] arr, int a, int b) {
		int temp = arr[a];
		arr[a] = arr[b];
		arr[b] = temp;
	}

	public static void swapCorner(int[] arr, int a, int b, int c, int d, int pow) {
		int temp;
		switch (pow) {
		case 0:
			temp = (arr[d] + 8) % 24;
			arr[d] = (arr[c] + 16) % 24;
			arr[c] = (arr[b] + 8) % 24;
			arr[b] = (arr[a] + 16) % 24;
			arr[a] = temp;
			return;
		case 1:
			temp = arr[a];
			arr[a] = arr[c];
			arr[c] = temp;
			temp = arr[b];
			arr[b] = arr[d];
			arr[d] = temp;
			return;
		case 2:
			temp = (arr[a] + 8) % 24;
			arr[a] = (arr[b] + 16) % 24;
			arr[b] = (arr[c] + 8) % 24;
			arr[c] = (arr[d] + 16) % 24;
			arr[d] = temp;
			return;
		}
	}

	public static void swap(int[] arr, int a, int b, int c, int d, int pow, bool flip) {
		int xor = flip ? 1 : 0;
		int temp;
		switch (pow) {
		case 0:
			temp = arr[d] ^ xor;
			arr[d] = arr[c] ^ xor;
			arr[c] = arr[b] ^ xor;
			arr[b] = arr[a] ^ xor;
			arr[a] = temp;
			return;
		case 1:
			temp = arr[a];
			arr[a] = arr[c];
			arr[c] = temp;
			temp = arr[b];
			arr[b] = arr[d];
			arr[d] = temp;
			return;
		case 2:
			temp = arr[a] ^ xor;
			arr[a] = arr[b] ^ xor;
			arr[b] = arr[c] ^ xor;
			arr[c] = arr[d] ^ xor;
			arr[d] = temp;
			return;
		}
	}

	public static void swap(int[] arr, int a, int b, int c, int d, int pow) {
		swap(arr, a, b, c, d, pow, false);
	}

	public static int indexOf(int[] arr, int value) {
		for(int i = 0; i < arr.Length; i++) {
			if (arr[i] == value) {
				return i;
			}
		}
		return -1;
	}
	public static int[, ] packSolved(int[] Solved1, int[] Solved2) {
		Solved1 ??= [0];
		Solved2 ??= [0];
		int[,] Solved = new int[Solved1.Length * Solved2.Length, 2];
		int idx = 0;
		for (int idx1 = 0; idx1 < Solved1.Length; idx1++) {
			for (int idx2 = 0; idx2 < Solved2.Length; idx2++) {
				Solved[idx, 0] = Solved1[idx1];
				Solved[idx, 1] = Solved2[idx2];
				idx++;
			}
		}
		return Solved;
	}


}


public class Coord {
	public int N_IDX = 1;
	public int N_MOVES = 0;
	public int idx;
	public Action<int> set;
	public Func<int, int> getMoved;
	public Coord() {
		set = i => idx = i;
	}
}

public class SymCoord : Coord {
	public int N_SYM;
	public int[] SelfSym;
}

public class RawCoord : Coord {
	public Func<int, int, int> getConj;
	public RawCoord() {
		getConj = (idx, conj) => idx;
	}
}

public class TableSymCoord : SymCoord {
	public int[,] moveTable;
	public TableSymCoord(int[,] moveTable, int[] SelfSym, int N_SYM) {
		this.moveTable = moveTable;
		this.SelfSym = SelfSym;
		this.N_SYM = N_SYM;
		this.N_IDX = moveTable.GetLength(0);
		this.N_MOVES = moveTable.GetLength(1);
		this.getMoved = move => moveTable[idx, move];
	}
}

public class TableRawCoord : RawCoord {
	public int[, ] moveTable;
	public int[, ] conjTable;
	public TableRawCoord(int[, ] moveTable, int[, ] conjTable) {
		this.moveTable = moveTable;
		this.conjTable = conjTable;
		this.N_IDX = moveTable.GetLength(0);
		this.N_MOVES = moveTable.GetLength(1);
		this.getMoved = move => moveTable[idx, move];
		this.getConj = (idx, conj) => conjTable[idx, conj];
	}
}

public class PruningTable {
	public int N_STATE;
	public int N_STATE2;
	public int[] Prun;
	public int TABLE_MASK = 0x7fffffff;

	private static void setPrun(int[] Prun, int idx, int xorval) {
		Prun[idx >> 3] ^= xorval << (idx << 2);
	}

	private static int getPrun(int[] Prun, int idx) {
		return Prun[idx >> 3] >> (idx << 2) & 0xf;
	}

	public PruningTable(Coord coord, int[] Solved, string filename) {
		initPrunTable(coord, Solved, filename);
	}

	public PruningTable(SymCoord coord, int[] Solved, string filename) {
		Solved ??= [0];
		int[, ] Solved2 = new int[Solved.Length, 2];
		for(int i = 0; i < Solved.Length; i++) {
			Solved2[i, 0] = Solved[i];
		}
		initPrunTable(coord, new RawCoord() {
			getMoved = move => 0,
		}, Solved2, filename);
	}

	public PruningTable(SymCoord symCoord, RawCoord rawCoord, int[,] Solved, string filename) {
		initPrunTable(symCoord, rawCoord, Solved, filename);
	}

	public PruningTable(SymCoord symCoord, RawCoord rawCoord, int[,] Solved, int maxl, int TABLE_SIZE, string filename) {
		initPrunTablePartial(symCoord, rawCoord, Solved, maxl, TABLE_SIZE, filename);
	}

	public PruningTable(int[,] Move, int[] Solved, string filename) {
		initPrunTable(new TableRawCoord(Move, null), Solved, filename);
	}

	public PruningTable(int[,] Move1, int[,] Move2, int[] Solved1, int[] Solved2, string filename) {
		N_STATE2 = Move2.GetLength(0);
		N_STATE = Move1.GetLength(0) * Move2.GetLength(0);
		Solved1 ??= [0];
		Solved2 ??= [0];
		int[] Solved = new int[Solved1.Length * Solved2.Length];
		for (int idx1 = 0; idx1 < Solved1.Length; idx1++) {
			for (int idx2 = 0; idx2 < Solved2.Length; idx2++) {
				Solved[idx1*Solved2.Length+idx2] = Solved1[idx1] * N_STATE2 + Solved2[idx2];
			}
		}
		int state2 = 0;
		var coord = new Coord() {
			N_IDX = Move1.GetLength(0) * Move2.GetLength(0),
			N_MOVES = Move1.GetLength(1),
		};
		coord.set = i => {
			coord.idx = i / N_STATE2;
			state2 = i % N_STATE2;
		};
		coord.getMoved = move => Move1[coord.idx, move] * N_STATE2 + Move2[state2, move];
		initPrunTable(coord, Solved, filename);
	}

	private void initPrunTablePartial(SymCoord symCoord, RawCoord rawCoord, int[, ] Solved, int maxl, int TABLE_SIZE, string filename) {
		N_STATE = symCoord.N_IDX * rawCoord.N_IDX;
		N_STATE2 = rawCoord.N_IDX;
		int N_SYM = symCoord.N_SYM;
		int N_MOVES = symCoord.N_MOVES;
		TABLE_MASK = TABLE_SIZE - 1;
		Prun = Tools.LoadFromFile($"Cache/{filename}.data");
		if (Prun != null) {
			return;
		}
		Solved ??= new int[, ] {{0, 0}};
		Dictionary<long, byte> PrunP = new();
		int done = 0;
		long realDone = 0;
		int depth = 0;
		for(int row = 0; row < Solved.GetLength(0); ++row){
			long idx = Solved[row, 0] * (long) N_STATE2 + Solved[row, 1];
			PrunP[idx] = 0;
			done++;
			realDone += N_SYM / Util.BitCount(symCoord.SelfSym[Solved[row, 0]]);
		}
		int cumDone = done;
		long cumRealDone = cumDone;
		long startTime = Util.SystemNanoTime();
		do {
			done = 0;
			realDone = 0;
			byte fill = (byte) (depth + 1);
			Dictionary<long, byte> PrunPClone = new(PrunP);
			foreach(var (key, value) in PrunPClone) {
				if (value != depth) {
					continue;
				}
				long i = key;
				symCoord.set((int) (i / N_STATE2));
				rawCoord.set((int) (i % N_STATE2));
				for(int m = 0; m < N_MOVES; m++) {
					int newSym = symCoord.getMoved(m);
					int newRaw = rawCoord.getConj(rawCoord.getMoved(m), newSym % N_SYM);
					newSym /= N_SYM;
					long newIdx = newSym * (long) N_STATE2 + newRaw;
					if (PrunP.TryAdd(newIdx, fill)) { // TOCHECK
						continue;
					}
					done++;
					realDone += N_SYM / Util.BitCount(symCoord.SelfSym[newSym]);
					for(int j = 1, symState = symCoord.SelfSym[newSym]; (symState >>= 1) != 0; j++) {
						if ((symState & 1) != 1) {
							continue;
						}
						long newIdx2 = newSym * (long) N_STATE2 + rawCoord.getConj(newRaw, j);
						if (PrunP.TryAdd(newIdx2, fill)) { // TOCHECK
							continue;
						}
						done++;
						realDone += N_SYM / Util.BitCount(symCoord.SelfSym[newSym]);
					}
				}
			}
			cumDone += done;
			cumRealDone += realDone;
			depth++;
		} while (done > 0 && depth < maxl);

		Prun = new int[TABLE_SIZE >> 3];
		for(int i = 0; i < Prun.Length; i++) {
			Prun[i] = 0x11111111 * (maxl + 1);
		}
			foreach(var (key, value) in PrunP) {
			int idx = (int) (key & TABLE_MASK);
			int val = value;
			int prun = getPrun(Prun, idx);
			if (val < prun) {
				setPrun(Prun, idx, val ^ prun);
			}
		}
		int[] depthCnt = new int[16];
		for(int i = 0; i < TABLE_SIZE; i++) {
			depthCnt[getPrun(Prun, i)]++;
		}
		Tools.SaveToFile($"Cache/{filename}.data", Prun);
	}

	private void initPrunTable(SymCoord symCoord, RawCoord rawCoord, int[,] Solved, string filename) {
		N_STATE = symCoord.N_IDX * rawCoord.N_IDX;
		N_STATE2 = rawCoord.N_IDX;
		int N_SYM = symCoord.N_SYM;
		int N_MOVES = symCoord.N_MOVES;
		Prun = Tools.LoadFromFile($"Cache/{filename}.data");
		if (Prun != null) {
			return;
		}
		Solved ??= new int[,] {{0, 0}};
		Prun = new int[(N_STATE + 7) / 8];
		for(int i = 0; i < Prun.Length; i++) {
			Prun[i] = -1;
		}
		int done = 0;
		long realDone = 0;
		int depth = 0;
		for(int row = 0; row < Solved.GetLength(0); ++row){
			int idx = Solved[row, 0] * N_STATE2 + Solved[row, 1];
			setPrun(Prun, idx, 0xf);
			done++;
			realDone += N_SYM / Util.BitCount(symCoord.SelfSym[Solved[row, 0]]);
		}
		int cumDone = done;
		long cumRealDone = cumDone;
		long startTime = Util.SystemNanoTime();
		do {
			done = 0;
			realDone = 0;
			bool inv = cumDone > N_STATE / 2;
			// bool inv = true;
			int select = inv ? 0xf : depth;
			int check = inv ? depth : 0xf;
			int fill = depth + 1;
			depth++;
			int val = 0;
			for(int i = 0; i < N_STATE; i++, val >>= 4) {
				if ((i & 7) == 0) {
					val = Prun[i >> 3];
					if (!inv && val == -1) {
						i += 7;
						continue;
					}
				}
				if ((val & 0xf) != select) {
					continue;
				}
				symCoord.set(i / N_STATE2);
				rawCoord.set(i % N_STATE2);
				for(int m = 0; m < N_MOVES; m++) {
					int newSym = symCoord.getMoved(m);
					int newRaw = rawCoord.getConj(rawCoord.getMoved(m), newSym % N_SYM);
					newSym /= N_SYM;
					int newIdx = newSym * N_STATE2 + newRaw;
					if (getPrun(Prun, newIdx) != check) {
						continue;
					}
					done++;
					if (inv) {
						setPrun(Prun, i, fill ^ 0xf);
						realDone += N_SYM / Util.BitCount(symCoord.SelfSym[i / N_STATE2]);
						break;
					}
					setPrun(Prun, newIdx, fill ^ 0xf);
					realDone += N_SYM / Util.BitCount(symCoord.SelfSym[newSym]);

					for(int j = 1, symState = symCoord.SelfSym[newSym]; (symState >>= 1) != 0; j++) {
						if ((symState & 1) != 1) {
							continue;
						}
						int newIdx2 = newSym * N_STATE2 + rawCoord.getConj(newRaw, j);
						if (getPrun(Prun, newIdx2) != check) {
							continue;
						}
						setPrun(Prun, newIdx2, fill ^ 0xf);
						done++;
						realDone += N_SYM / Util.BitCount(symCoord.SelfSym[newSym]);
					}
				}
			}
			cumDone += done;
			cumRealDone += realDone;
		} while (done > 0 && depth < 15);
		Tools.SaveToFile($"Cache/{filename}.data", Prun);
	}

	private void initPrunTable(Coord coord, int[] Solved, string filename) {
		N_STATE = coord.N_IDX;
		Prun = Tools.LoadFromFile($"Cache/{filename}.data");
		if (Prun != null) {
			return;
		}
		Solved ??= [0];
		Prun = new int[(N_STATE + 7) / 8];
		for(int i = 0; i < Prun.Length; i++) {
			Prun[i] = -1;
		}
		int done = 0;
		int depth = 0;
		foreach(int idx in Solved) {
			setPrun(Prun, idx, 0xf);
			done++;
		}
		long startTime = Util.SystemNanoTime();
		int cumDone = done;
		do {
			done = 0;
			bool inv = cumDone > N_STATE / 2;
			int select = inv ? 0xf : depth;
			int check = inv ? depth : 0xf;
			int fill = depth + 1;
			depth++;
			int val = 0;
			for(int i = 0; i < N_STATE; i++, val >>= 4) {
				if ((i & 7) == 0) {
					val = Prun[i >> 3];
					if (!inv && val == -1) {
						i += 7;
						continue;
					}
				}
				if ((val & 0xf) != select) {
					continue;
				}
				coord.set(i);
				for(int m = 0; m < coord.N_MOVES; m++) {
					int newIdx = coord.getMoved(m);
					if (getPrun(Prun, newIdx) != check) {
						continue;
					}
					done++;
					if (inv) {
						setPrun(Prun, i, fill ^ 0xf);
						break;
					}
					setPrun(Prun, newIdx, fill ^ 0xf);
				}
			}
			cumDone += done;
		} while (done > 0 && depth <= 15);
		Tools.SaveToFile($"Cache/{filename}.data", Prun);
	}

	public int getPrun(int state1, int state2) {
		return getPrun(Prun, (state1 * N_STATE2 + state2) & TABLE_MASK);
	}

	public int getPrun(int state) {
		return getPrun(Prun, state & TABLE_MASK);
	}
}