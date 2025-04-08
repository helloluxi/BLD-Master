using System;
using System.Text;
using static CS.Cube555.Util;
using System.Runtime.InteropServices;
namespace CS.Cube555;

/*

Facelet:
						U1	U2	U3	U4	U5
						U6	U7	U8	U9	U10
						U11	U12	U13	U14	U15
						U16	U17	U18	U19	U20
						U21	U22	U23	U24	U25

L1	L2	L3	L4	L5		F1	F2	F3	F4	F5		R1	R2	R3	R4	R5		B1	B2	B3	B4	B5
L6	L7	L8	L9	L10		F6	F7	F8	F9	F10		R6	R7	R8	R9	R10		B6	B7	B8	B9	B10
L11	L12	L13	L14	L15		F11	F12	F13	F14	F15		R11	R12	R13	R14	R15		B11	B12	B13	B14	B15
L16	L17	L18	L19	L20		F16	F17	F18	F19	F20		R16	R17	R18	R19	R20		B16	B17	B18	B19	B20
L21	L22	L23	L24	L25		F21	F22	F23	F24	F25		R21	R22	R23	R24	R25		B21	B22	B23	B24	B25

						D1	D2	D3	D4	D5
						D6	D7	D8	D9	D10
						D11	D12	D13	D14	D15
						D16	D17	D18	D19	D20
						D21	D22	D23	D24	D25

Center:
           0  0  1
           3     1
           3  2  2

20 20 21   8  8  9    16 16 17   12 12  13
23    21   11    9    19    17   15     13
23 22 22   11 10 10   19 18 18   15 14  14

           4  4  5
           7     5
           7  6  6

Edge:
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

public class CubieCube {

	// For pretty print
	public static int[] PRint_FACELET = [
	    U1, U2, U3, U4, U5,
	    U6, U7, U8, U9, U10,
	    U11, U12, U13, U14, U15,
	    U16, U17, U18, U19, U20,
	    U21, U22, U23, U24, U25,
	    L1, L2, L3, L4, L5, F1, F2, F3, F4, F5, R1, R2, R3, R4, R5, B1, B2, B3, B4, B5,
	    L6, L7, L8, L9, L10, F6, F7, F8, F9, F10, R6, R7, R8, R9, R10, B6, B7, B8, B9, B10,
	    L11, L12, L13, L14, L15, F11, F12, F13, F14, F15, R11, R12, R13, R14, R15, B11, B12, B13, B14, B15,
	    L16, L17, L18, L19, L20, F16, F17, F18, F19, F20, R16, R17, R18, R19, R20, B16, B17, B18, B19, B20,
	    L21, L22, L23, L24, L25, F21, F22, F23, F24, F25, R21, R22, R23, R24, R25, B21, B22, B23, B24, B25,
	    D1, D2, D3, D4, D5,
	    D6, D7, D8, D9, D10,
	    D11, D12, D13, D14, D15,
	    D16, D17, D18, D19, D20,
	    D21, D22, D23, D24, D25
	];

	public static int[] MAP333_FACELET = [
	    U1, U3, U5, U11, U13, U15, U21, U23, U25,
	    R1, R3, R5, R11, R13, R15, R21, R23, R25,
	    F1, F3, F5, F11, F13, F15, F21, F23, F25,
	    D1, D3, D5, D11, D13, D15, D21, D23, D25,
	    L1, L3, L5, L11, L13, L15, L21, L23, L25,
	    B1, B3, B5, B11, B13, B15, B21, B23, B25
	];

	public static int[] TCENTER = [
	    U8, U14, U18, U12,
	    D8, D14, D18, D12,
	    F8, F14, F18, F12,
	    B8, B14, B18, B12,
	    R8, R14, R18, R12,
	    L8, L14, L18, L12
	];

	public static int[] XCENTER = [
	    U7, U9, U19, U17,
	    D7, D9, D19, D17,
	    F7, F9, F19, F17,
	    B7, B9, B19, B17,
	    R7, R9, R19, R17,
	    L7, L9, L19, L17
	];

	public static int[, ] MEDGE = new int[, ] {
		{U23, F3}, {U3, B3}, {D23, B23}, {D3, F23},
		{U11, L3}, {U15, R3}, {D15, R23}, {D11, L23},
		{L15, F11}, {L11, B15}, {R15, B11}, {R11, F15}
	};

	public static int[, ] WEDGE = new int[, ] {
		{U22, F2}, {U4, B2}, {D22, B24}, {D4, F24},
		{U6, L2}, {U20, R2}, {D20, R24}, {D6, L24},
		{L20, F16}, {L6, B10}, {R20, B16}, {R6, F10},
		{F4, U24}, {B4, U2}, {B22, D24}, {F22, D2},
		{L4, U16}, {R4, U10}, {R22, D10}, {L22, D16},
		{F6, L10}, {B20, L16}, {B6, R10}, {F20, R16}
	};

	public static int[, ] CORNER = new int[, ] {
		{U25, R1, F5}, {U21, F1, L5}, {U1, L1, B5}, {U5, B1, R5},
		{D5, F25, R21}, {D1, L25, F21}, {D21, B25, L21}, {D25, R25, B21}
	};

	public static CubieCube SOLVED = new();
 
	public int[] tCenter = new int[24];
	public int[] xCenter = new int[24];
	public int[] mEdge = new int[12];
	public int[] wEdge = new int[24];
	public CornerCube corner = new();
 
	public CubieCube() {
		for(int i = 0; i < 24; i++) {
			tCenter[i] = TCENTER[i] / 25;
			xCenter[i] = XCENTER[i] / 25;
			wEdge[i] = i;
		}
		for(int i = 0; i < 12; i++) {
			mEdge[i] = i << 1;
		}
	}

	CubieCube(CubieCube cc) {
		copy(cc);
	}

	public virtual void copy(CubieCube cc) {
		for(int i = 0; i < 24; i++) {
			tCenter[i] = cc.tCenter[i];
			xCenter[i] = cc.xCenter[i];
			wEdge[i] = cc.wEdge[i];
		}
		for(int i = 0; i < 12; i++) {
			mEdge[i] = cc.mEdge[i];
		}
	}

	public static string to333Facelet(string facelet) {
		StringBuilder sb = new();
		foreach(int i in MAP333_FACELET) {
			sb.Append(facelet[i]);
		}
		return sb.ToString();
	}

	static string fill333Facelet(string facelet, string facelet333) {
		StringBuilder sb = new(facelet);
		for(int i = 0; i < MAP333_FACELET.Length; i++) {
			sb[MAP333_FACELET[i]] = facelet333[i];
		}
		return sb.ToString();
	}

	public int fromFacelet(string facelet) {
		int[] face = new int[150];
		long colorCnt = 0;
		try {
			string colors = new(
			    [
			        facelet[U13], facelet[R13], facelet[F13],
			        facelet[D13], facelet[L13], facelet[B13]
			    ]
			);
			for(int i = 0; i < 150; i++) {
				face[i] = colors.IndexOf(facelet[i]);
				if (face[i] == -1) {
					return -1;
				}
				colorCnt += 1L << face[i] * 8;
			}
		} catch (Exception) {
			return -1;
		}
		int tCenterCnt = 0;
		int xCenterCnt = 0;
		for(int i = 0; i < 24; i++) {
			tCenter[i] = face[TCENTER[i]];
			xCenter[i] = face[XCENTER[i]];
			tCenterCnt += 1 << (tCenter[i] << 2);
			xCenterCnt += 1 << (xCenter[i] << 2);
		}
		int mEdgeCnt = 0;
		int mEdgeChk = 0;
		for(int i = 0; i < 12; i++) {
			for(int j = 0; j < 12; j++) {
				if (face[MEDGE[i, 0]] == MEDGE[j, 0] / 25
				        && face[MEDGE[i, 1]] == MEDGE[j, 1] / 25
				        || face[MEDGE[i, 0]] == MEDGE[j, 1] / 25
				        && face[MEDGE[i, 1]] == MEDGE[j, 0] / 25
				   ) {
					int ori_ = face[MEDGE[i, 0]] == MEDGE[j, 0] / 25 ? 0 : 1;
					mEdge[i] = j << 1 | ori_;
					mEdgeCnt |= 1 << j;
					mEdgeChk ^= ori_;
					break;
				}
			}
		}
		int wEdgeCnt = 0;
		for(int i = 0; i < 24; i++) {
			for(int j = 0; j < 24; j++) {
				if (face[WEDGE[i, 0]] == WEDGE[j, 0] / 25
				        && face[WEDGE[i, 1]] == WEDGE[j, 1] / 25) {
					wEdge[i] = j;
					wEdgeCnt |= 1 << j;
					break;
				}
			}
		}

        int cornerCnt = 0;
        int cornerChk = 0;
		for(int i = 0; i < 8; i++) {
            int ori;
            for (ori = 0; ori < 3; ori++)
            {
                if (face[CORNER[i, ori]] == 0 || face[CORNER[i, ori]] == 3)
                {
                    break;
                }
            }
            int col1 = face[CORNER[i, (ori + 1) % 3]];
			int col2 = face[CORNER[i, (ori + 2) % 3]];
			for(int j = 0; j < 8; j++) {
				if (col1 == CORNER[j, 1] / 25 && col2 == CORNER[j, 2] / 25) {
					corner.cp[i] = j;
					corner.co[i] = ori;
					cornerChk += ori;
					cornerCnt |= 1 << j;
					break;
				}
			}
		}
		int[] ep = new int[12];
		for(int i = 0; i < 12; i++) {
			ep[i] = mEdge[i] >> 1;
		}
		if (colorCnt != 0x191919191919L) {
			return -1;
		} else if (tCenterCnt != 0x444444) {
			return -2;
		} else if (xCenterCnt != 0x444444) {
			return -3;
		} else if (mEdgeCnt != 0xfff) {
			return -4;
		} else if (wEdgeCnt != 0xffffff) {
			return -5;
		} else if (cornerCnt != 0xff) {
			return -6;
		} else if (mEdgeChk != 0) {
			return -7;
		} else if (cornerChk % 3 != 0) {
			return -8;
		} else if (getParity(ep) != getParity(corner.cp)) {
			return -9;
		}
		return 0;
	}

	public string toFacelet() {
		char[] face = new char[150];
		string colors = "URFDLB";
		for(int i = 0; i < 150; i++) {
			face[i] = i % 25 == 12 ? colors[i / 25] : '-';
		}
		for(int i = 0; i < 24; i++) {
			face[TCENTER[i]] = colors[tCenter[i]];
			face[XCENTER[i]] = colors[xCenter[i]];
			for(int j = 0; j < 2; j++) {
				face[WEDGE[i, j]] = colors[WEDGE[wEdge[i], j] / 25];
			}
		}
		for(int i = 0; i < 12; i++) {
			int perm = mEdge[i] >> 1;
			int ori = mEdge[i] & 1;// Orientation of this cubie
			for(int j = 0; j < 2; j++) {
				face[MEDGE[i, j ^ ori]] = colors[MEDGE[perm, j] / 25];
			}
		}
		for(int i = 0; i < 8; i++) {
			int j = corner.cp[i];
			int ori = corner.co[i];
			for(int n = 0; n < 3; n++) {
				face[CORNER[i, (n + ori) % 3]] = colors[CORNER[j, n] / 25];
			}
		}
		return new string(face);
	}

	public override string ToString() {
		// Enable ANSI color support in Windows
		if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
			var handle = GetStdHandle(-11);
            GetConsoleMode(handle, out uint mode);
            SetConsoleMode(handle, mode | 0x4);
		}

		string facelet = toFacelet();
		string colors = "URFDLB-";
		string[] controls = [
		    "\x1b[93m", // U -> yellow
		    "\x1b[32m", // R -> green
		    "\x1b[31m", // F -> red
		    "\x1b[37m", // D -> white
		    "\x1b[34m", // L -> blue
		    "\x1b[33m", // B -> orange
		    "\x1b[30m"  // - -> black
		];
		string[] arr = new string[150];
		for(int i = 0; i < 150; i++) {
			char val = facelet[PRint_FACELET[i]];
			arr[i] = controls[colors.IndexOf(val)] + " " + val + "\x1b[0m";
		}
		return string.Format(
		           "           {0}{1}{2}{3}{4}\n" +
		           "           {5}{6}{7}{8}{9}\n" +
		           "           {10}{11}{12}{13}{14}\n" +
		           "           {15}{16}{17}{18}{19}\n" +
		           "           {20}{21}{22}{23}{24}\n" +
		           "{25}{26}{27}{28}{29} {30}{31}{32}{33}{34} {35}{36}{37}{38}{39} {40}{41}{42}{43}{44}\n" +
		           "{45}{46}{47}{48}{49} {50}{51}{52}{53}{54} {55}{56}{57}{58}{59} {60}{61}{62}{63}{64}\n" +
		           "{65}{66}{67}{68}{69} {70}{71}{72}{73}{74} {75}{76}{77}{78}{79} {80}{81}{82}{83}{84}\n" +
		           "{85}{86}{87}{88}{89} {90}{91}{92}{93}{94} {95}{96}{97}{98}{99} {100}{101}{102}{103}{104}\n" +
		           "{105}{106}{107}{108}{109} {110}{111}{112}{113}{114} {115}{116}{117}{118}{119} {120}{121}{122}{123}{124}\n" +
		           "           {125}{126}{127}{128}{129}\n" +
		           "           {130}{131}{132}{133}{134}\n" +
		           "           {135}{136}{137}{138}{139}\n" +
		           "           {140}{141}{142}{143}{144}\n" +
		           "           {145}{146}{147}{148}{149}\x1b[0m",
		           arr);
	}

	public void doCornerMove(params int[] moves) {
		foreach(int move in moves) {
			corner.doMove(move % 18);
		}
	}

	public virtual void doMove(params int[] moves) {
		foreach(int move in moves) {
			int pow = move % 3;
			switch (move) {
			case ux1:
			case ux2:
			case ux3:
				swap(xCenter, 8, 20, 12, 16, pow);
				swap(xCenter, 9, 21, 13, 17, pow);
				swap(tCenter, 8, 20, 12, 16, pow);
				swap(wEdge, 9, 22, 11, 20, pow);
				goto case Ux1;
			case Ux1:
			case Ux2:
			case Ux3:
				swap(xCenter, 0, 1, 2, 3, pow);
				swap(tCenter, 0, 1, 2, 3, pow);
				swap(mEdge, 0, 4, 1, 5, pow);
				swap(wEdge, 0, 4, 1, 5, pow);
				swap(wEdge, 12, 16, 13, 17, pow);
				break;
			case rx1:
			case rx2:
			case rx3:
				swap(xCenter, 1, 15, 5, 9, pow);
				swap(xCenter, 2, 12, 6, 10, pow);
				swap(tCenter, 1, 15, 5, 9, pow);
				swap(wEdge, 1, 14, 3, 12, pow);
				goto case Rx1;
			case Rx1:
			case Rx2:
			case Rx3:
				swap(xCenter, 16, 17, 18, 19, pow);
				swap(tCenter, 16, 17, 18, 19, pow);
				swap(mEdge, 5, 10, 6, 11, pow, true);
				swap(wEdge, 5, 22, 6, 23, pow);
				swap(wEdge, 17, 10, 18, 11, pow);
				break;
			case fx1:
			case fx2:
			case fx3:
				swap(xCenter, 2, 19, 4, 21, pow);
				swap(xCenter, 3, 16, 5, 22, pow);
				swap(tCenter, 2, 19, 4, 21, pow);
				swap(wEdge, 5, 18, 7, 16, pow);
				goto case Fx1;
			case Fx1:
			case Fx2:
			case Fx3:
				swap(xCenter, 8, 9, 10, 11, pow);
				swap(tCenter, 8, 9, 10, 11, pow);
				swap(mEdge, 0, 11, 3, 8, pow);
				swap(wEdge, 0, 11, 3, 8, pow);
				swap(wEdge, 12, 23, 15, 20, pow);
				break;
			case dx1:
			case dx2:
			case dx3:
				swap(xCenter, 10, 18, 14, 22, pow);
				swap(xCenter, 11, 19, 15, 23, pow);
				swap(tCenter, 10, 18, 14, 22, pow);
				swap(wEdge, 8, 23, 10, 21, pow);
				goto case Dx1;
			case Dx1:
			case Dx2:
			case Dx3:
				swap(xCenter, 4, 5, 6, 7, pow);
				swap(tCenter, 4, 5, 6, 7, pow);
				swap(mEdge, 2, 7, 3, 6, pow);
				swap(wEdge, 2, 7, 3, 6, pow);
				swap(wEdge, 14, 19, 15, 18, pow);
				break;
			case lx1:
			case lx2:
			case lx3:
				swap(xCenter, 0, 8, 4, 14, pow);
				swap(xCenter, 3, 11, 7, 13, pow);
				swap(tCenter, 3, 11, 7, 13, pow);
				swap(wEdge, 0, 15, 2, 13, pow);
				goto case Lx1;
			case Lx1:
			case Lx2:
			case Lx3:
				swap(xCenter, 20, 21, 22, 23, pow);
				swap(tCenter, 20, 21, 22, 23, pow);
				swap(mEdge, 4, 8, 7, 9, pow, true);
				swap(wEdge, 4, 20, 7, 21, pow);
				swap(wEdge, 16, 8, 19, 9, pow);
				break;
			case bx1:
			case bx2:
			case bx3:
				swap(xCenter, 1, 20, 7, 18, pow);
				swap(xCenter, 0, 23, 6, 17, pow);
				swap(tCenter, 0, 23, 6, 17, pow);
				swap(wEdge, 4, 19, 6, 17, pow);
				goto case Bx1;
			case Bx1:
			case Bx2:
			case Bx3:
				swap(xCenter, 12, 13, 14, 15, pow);
				swap(tCenter, 12, 13, 14, 15, pow);
				swap(mEdge, 1, 9, 2, 10, pow);
				swap(wEdge, 1, 9, 2, 10, pow);
				swap(wEdge, 13, 21, 14, 22, pow);
				break;
			}
		}
	}

	public virtual void doConj(int idx) {
		CubieCube a = new(this);
		CubieCube sinv = CubeSym[SymMultInv[0, idx]];
		CubieCube s = CubeSym[idx];
		for(int i = 0; i < 12; i++) {
			this.mEdge[i] = sinv.mEdge[a.mEdge[s.mEdge[i] >> 1] >> 1]
			                ^ (a.mEdge[s.mEdge[i] >> 1] & 1)
			                ^ (s.mEdge[i] & 1);
		}
		for(int i = 0; i < 24; i++) {
			this.tCenter[i] = SOLVED.tCenter[sinv.tCenter[COLOR_TO_CENTER[a.tCenter[s.tCenter[i]]]]];
			this.xCenter[i] = SOLVED.xCenter[sinv.xCenter[COLOR_TO_CENTER[a.xCenter[s.xCenter[i]]]]];
			this.wEdge[i] = sinv.wEdge[a.wEdge[s.wEdge[i]]];
		}
	}

	static CubieCube[] CubeSym = new CubieCube[48];
	public static int[, ] SymMult = new int[48, 48];
	public static int[, ] SymMultInv = new int[48, 48];
	public static int[, ] SymMove = new int[48, 36];

	public static void CubeMult(CubieCube a, CubieCube b, CubieCube prod) {
		for(int i = 0; i < 12; i++) {
			prod.mEdge[i] = a.mEdge[b.mEdge[i] >> 1] ^ (b.mEdge[i] & 1);
		}
		for(int i = 0; i < 24; i++) {
			prod.tCenter[i] = a.tCenter[b.tCenter[i]];
			prod.xCenter[i] = a.xCenter[b.xCenter[i]];
			prod.wEdge[i] = a.wEdge[b.wEdge[i]];
		}
	}

	public static int[] COLOR_TO_CENTER = [0, 16, 8, 4, 20, 12];

	public static void init() {
		CornerCube.initMove();
		CubieCube c = new();
		for(int i = 0; i < 24; i++) {
			c.tCenter[i] = i;
			c.xCenter[i] = i;
		}
		for(int i = 0; i < 48; i++) {
			CubeSym[i] = new CubieCube(c);

			// x
			c.doMove(rx1, lx3);
			swap(c.tCenter, 0, 14, 4, 8, 0);
			swap(c.tCenter, 2, 12, 6, 10, 0);
			swap(c.mEdge, 0, 1, 2, 3, 0, true);

			if ((i & 0x3) == 0x3) {
				// y2
				c.doMove(ux2, dx2);
				swap(c.tCenter, 9, 21, 13, 17, 1);
				swap(c.tCenter, 11, 23, 15, 19, 1);
				swap(c.mEdge, 8, 9, 10, 11, 1, true);
			}
			if ((i & 0x7) == 0x7) {
				// lr mirror
				swap(c.tCenter, 1, 3);
				swap(c.tCenter, 5, 7);
				swap(c.tCenter, 9, 11);
				swap(c.tCenter, 13, 15);
				swap(c.tCenter, 16, 20);
				swap(c.tCenter, 17, 23);
				swap(c.tCenter, 18, 22);
				swap(c.tCenter, 19, 21);
				swap(c.xCenter, 0, 1);
				swap(c.xCenter, 2, 3);
				swap(c.xCenter, 4, 5);
				swap(c.xCenter, 6, 7);
				swap(c.xCenter, 8, 9);
				swap(c.xCenter, 10, 11);
				swap(c.xCenter, 12, 13);
				swap(c.xCenter, 14, 15);
				swap(c.xCenter, 16, 21);
				swap(c.xCenter, 17, 20);
				swap(c.xCenter, 18, 23);
				swap(c.xCenter, 19, 22);
				swap(c.wEdge, 0, 12);
				swap(c.wEdge, 1, 13);
				swap(c.wEdge, 2, 14);
				swap(c.wEdge, 3, 15);
				swap(c.wEdge, 4, 17);
				swap(c.wEdge, 5, 16);
				swap(c.wEdge, 6, 19);
				swap(c.wEdge, 7, 18);
				swap(c.wEdge, 8, 23);
				swap(c.wEdge, 9, 22);
				swap(c.wEdge, 10, 21);
				swap(c.wEdge, 11, 20);
				swap(c.mEdge, 4, 5);
				swap(c.mEdge, 6, 7);
				swap(c.mEdge, 8, 11);
				swap(c.mEdge, 9, 10);
			}
			if ((i & 0xf) == 0xf) {
				// URF -> RFU <=> x y
				c.doMove(rx1, lx3);
				swap(c.tCenter, 0, 14, 4, 8, 0);
				swap(c.tCenter, 2, 12, 6, 10, 0);
				swap(c.mEdge, 0, 1, 2, 3, 0, true);
				c.doMove(ux1, dx3);
				swap(c.tCenter, 9, 21, 13, 17, 0);
				swap(c.tCenter, 11, 23, 15, 19, 0);
				swap(c.mEdge, 8, 9, 10, 11, 0, true);
			}
		}
		for(int i = 0; i < 48; i++) {
			for(int j = 0; j < 48; j++) {
				CubeMult(CubeSym[i], CubeSym[j], c);
				for(int k = 0; k < 48; k++) {
					if (System.Linq.Enumerable.SequenceEqual(CubeSym[k].wEdge, c.wEdge)) {
						SymMult[i, j] = k;
						SymMultInv[k, j] = i;
						break;
					}
				}
			}
		}

		for(int move = 0; move < 36; move++) {
			for(int s = 0; s < 48; s++) {
				c = new CubieCube();
				c.doMove(move);
				c.doConj(SymMultInv[0, s]);
				for(int move2 = 0; move2 < 36; move2++) {
					CubieCube d = new();
					d.doMove(move2);
					if (System.Linq.Enumerable.SequenceEqual(c.wEdge, d.wEdge)) {
						SymMove[s, move] = move2;
						break;
					}
				}
			}
		}
	}

	public static int[, ] getSymMove(int[] moves, int nsym) {
		int[] symList = new int[nsym];
		for(int i = 0; i < nsym; i++) {
			symList[i] = i;
		}
		return getSymMove(moves, symList);
	}

	public static int[, ] getSymMove(int[] moves, int[] symList) {
		int[, ] ret = new int[symList.Length, moves.Length];
		for(int s = 0; s < symList.Length; s++) {
			for(int m = 0; m < moves.Length; m++) {
				ret[s, m] = indexOf(moves, SymMove[symList[s], moves[m]]);
			}
		}
		return ret;
	}

	public class CornerCube {

		private static CornerCube[] moveCube = new CornerCube[18];

		public int[] cp = [0, 1, 2, 3, 4, 5, 6, 7];
		public int[] co = [0, 0, 0, 0, 0, 0, 0, 0];

		public CornerCube() {}

		public CornerCube(int cperm, int twist) {
			setPerm(cp, cperm);
			int twst = 0;
			for(int i = 6; i >= 0; i--) {
				twst += co[i] = twist % 3;
				twist /= 3;
			}
			co[7] = (15 - twst) % 3;
		}

		public CornerCube(CornerCube c) {
			copy(c);
		}

		public void copy(CornerCube c) {
			for(int i = 0; i < 8; i++) {
				this.cp[i] = c.cp[i];
				this.co[i] = c.co[i];
			}
		}

		public static void CornMult(CornerCube a, CornerCube b, CornerCube prod) {
			for(int corn = 0; corn < 8; corn++) {
				prod.cp[corn] = a.cp[b.cp[corn]];
				prod.co[corn] = (a.co[b.cp[corn]] + b.co[corn]) % 3;
			}
		}

		public void doMove(int move) {
			CornerCube cc = new();
			CornMult(this, moveCube[move], cc);
			copy(cc);
		}

		public static void initMove() {
			moveCube[0] = new CornerCube(15120, 0);
			moveCube[3] = new CornerCube(21021, 1494);
			moveCube[6] = new CornerCube(8064, 1236);
			moveCube[9] = new CornerCube(9, 0);
			moveCube[12] = new CornerCube(1230, 412);
			moveCube[15] = new CornerCube(224, 137);
			for(int a = 0; a < 18; a += 3) {
				for(int p = 0; p < 2; p++) {
					moveCube[a + p + 1] = new CornerCube();
					CornMult(moveCube[a + p], moveCube[a], moveCube[a + p + 1]);
				}
			}
		}
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll")]
	private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

	[DllImport("kernel32.dll")]
	private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
}