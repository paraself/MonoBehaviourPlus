using UnityEngine;
using System.Collections;

public class TestMBP : MonoBehaviourPlus<TestMBP> {

	public double pi;
	public int maxIteration = 100;

	protected override void ParallelUpdate () {
		pi = 2 * F(1);
	}

	public void NotMultithread() {
		ParallelUpdate();
	}

	double F (int i) {
		//some heavy delay
		for (int p = 0;p<maxIteration * 10;p++) {
			float o = Mathf.Sqrt(p);
		}
		//some heavy delay
		if ( i <= maxIteration)
			return 1 + i / (2.0 * i + 1) * F(i + 1);
		else return 0;
	}

}
