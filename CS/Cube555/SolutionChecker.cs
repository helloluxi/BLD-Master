using System;
namespace CS.Cube555;

public class SolutionChecker {

	public SolvingCube[] ccList;

	public SolutionChecker(SolvingCube[] ccList) {
		this.ccList = ccList;
	}

	public int check(int[] solution, int length, int ccidx) {
		SolvingCube sc = new(ccList[ccidx]);
		sc.doMove(copySolution(solution, length));
		sc.addCheckPoint();
		return check_(sc);
	}

	public Func<SolvingCube, int> check_ = null;

	public static int[] copySolution(int[] solution, int length) {
		int[] solutionCopy = new int[length];
		for(int i = 0; i < length; i++) {
			solutionCopy[i] = solution[i];
		}
		return solutionCopy;
	}
}