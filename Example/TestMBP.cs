using UnityEngine;
using System.Collections;
using System.Threading;

public class TestMBP : MonoBehaviourPlus<TestMBP> {

	public int ID;

	public int sleep = 100;
	public int result = 0;

	protected override void ParallelUpdate () {
		Thread.Sleep(sleep);
		if (MBP_Manager.isDebugOn)
			Debug.Log("ID:"+ID + " finished parallel update for frame:" + MBP_Manager.frame);
	}

	public void NotMultithread() {
		ParallelUpdate();
	}

}
