using System;
using System.Collections.Generic;
using static CS.Cube555.Util;
namespace CS.Cube555;

public class Search {
	public const int USE_SEPARATOR = 0x1;
	public const int INVERSE_SOLUTION = 0x2;

	public static int phase1SolsSize = 200;
	public static int phase2SolsSize = 500;
	public static int phase3SolsSize = 500;
	public static int phase4SolsSize = 500;
	public static int phase5SolsSize = 1;

	public static class Logger {
		public static long startTime;
		public static long cumSolLen = 0;
		public static long[] cumPhaseT = new long[5];
		public static void start() {
			// startTime = SystemNanoTime();
		}
		public static void logTime(int phase) {
			// cumPhaseT[phase] += SystemNanoTime() - startTime;
			// Console.WriteLine($"Phase{phase + 1} Finished in {(SystemNanoTime() - startTime) / 1000000}ms");
			// startTime = SystemNanoTime();
		}
		public static void print(int div) {
			// Console.WriteLine(
			//     string.Format(
			//         "AvgLen=%.2f P1T=%4dms P2T=%4dms P3T=%4dms P4T=%4dms P5T=%4dms TOT=%4dms",
			//         cumSolLen * 1.0 / div,
			//         cumPhaseT[0] / div / 1000000,
			//         cumPhaseT[1] / div / 1000000,
			//         cumPhaseT[2] / div / 1000000,
			//         cumPhaseT[3] / div / 1000000,
			//         cumPhaseT[4] / div / 1000000,
			//         (cumPhaseT[0] + cumPhaseT[1] + cumPhaseT[2] + cumPhaseT[3] + cumPhaseT[4]) / div / 1000000
			//     ));
		}
	}

	public static bool isInited = false;

	public Search() => init();
	public static void init() { // TOCHECK: synchronized
		if (isInited) {
			return;
		}
		CubieCube.init();
		Phase1Search.init();
		Phase2Search.init();
		Phase3Search.init();
		Phase4Search.init();
		Phase5Search.init();
		isInited = true;
	}

	Phase1Search p1search = new();
	Phase2Search p2search = new();
	Phase3Search p3search = new();
	Phase4Search p4search = new();
	Phase5Search p5search = new();
	List<SolvingCube> p1sols = new();
	List<SolvingCube> p2sols = new();
	List<SolvingCube> p3sols = new();
	List<SolvingCube> p4sols = new();
	List<SolvingCube> p5sols = new();
	SolvingCube[] p1cc;
	SolvingCube[] p2cc;
	SolvingCube[] p3cc;
	SolvingCube[] p4cc;
	SolvingCube[] p5cc;

	public string SolveReduction(string facelet, int verbose) { // TOCHECK:  synchronized
		CubieCube cc = new();
		int verifyReduction = cc.fromFacelet(facelet);
		if (verifyReduction != 0) {
			return "Error " + verifyReduction;
		}
		p1sols.Clear();
		p2sols.Clear();
		p3sols.Clear();
		p4sols.Clear();
		p5sols.Clear();

		Logger.start();

		SolvingCube sc = new(cc);

		p1cc = new SolvingCube[3]; // TOCHECK: new p1cc
		for(int i = 0; i < 3; i++) {
			p1cc[i] = new SolvingCube(sc);
			sc.doConj(16);
		}
		p1search.solve(p1cc, new SolutionChecker(p1cc) {
			check_ = sc =>
            {
				p1sols.Add(sc);
				return p1sols.Count >= phase1SolsSize ? 0 : 1;
			}
		});
		Logger.logTime(0);

		p2cc = [.. p1sols]; // TOCHECK: new p2cc
		p2search.solve(p2cc, new SolutionChecker(p2cc) {
			check_ = sc =>
            {
				for(int i = 0; i < 3; i++) {
					p2sols.Add(new SolvingCube(sc));
					sc.doConj(16);
				}
				return p2sols.Count >= phase2SolsSize ? 0 : 1;
			}
		});
		Logger.logTime(1);

		p3cc = [.. p2sols]; // TOCHECK: ToArray(new SolvingCube[0])
		p3search.solve(p3cc, new SolutionChecker(p3cc) {
			check_ = sc =>
            {
				int maskY = 0;
				int maskZ = 0;
				for(int i = 0; i < 4; i++) {
					maskY |= 1 << (sc.wEdge[8 + i] % 12);
					maskY |= 1 << (sc.wEdge[8 + i + 12] % 12);
					maskY |= 1 << (sc.mEdge[8 + i] >> 1);
					maskZ |= 1 << (sc.wEdge[4 + i] % 12);
					maskZ |= 1 << (sc.wEdge[4 + i + 12] % 12);
					maskZ |= 1 << (sc.mEdge[4 + i] >> 1);
				}
				if (BitCount(maskY) <= 8) {
					p3sols.Add(sc);
				}
				if (BitCount(maskZ) <= 8) {
					sc.doConj(1);
					p3sols.Add(new SolvingCube(sc));
				}
				return p3sols.Count >= phase3SolsSize ? 0 : 1;
			}
		});
		Logger.logTime(2);

		p4cc = [.. p3sols]; // TOCHECK: ToArray(new SolvingCube[0])
		p4search.solve(p4cc, new SolutionChecker(p4cc) {
			check_ = sc =>
            {
				sc.doConj(1);
				p4sols.Add(sc);
				return p4sols.Count >= phase4SolsSize ? 0 : 1;
			}
		});
		Logger.logTime(3);

		p5cc = [.. p4sols]; // TOCHECK: ToArray(new SolvingCube[0])
		p5search.solve(p5cc, new SolutionChecker(p5cc) {
			check_ = sc =>
            {
				p5sols.Add(sc);
				return p5sols.Count >= phase5SolsSize ? 0 : 1;
			}
		});
		Logger.logTime(4);

		sc = p5sols[0];
		Logger.cumSolLen += sc.length();

		cc.doMove(sc.getSolution());
		cc.doCornerMove(sc.getSolution());

		string cube3sol = Global.Search3.Solution(CubieCube.to333Facelet(cc.toFacelet()), 21, 100000000, 0, verbose);

		return (verbose & INVERSE_SOLUTION) == 0 ? sc.toSolutionString(verbose) + cube3sol : cube3sol + sc.toSolutionString(verbose);
	}
}