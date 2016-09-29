using UnityEngine;
using System.Collections;
using System.Threading;

public class TestMBP : MonoBehaviourPlus<TestMBP> {

	public int ID;					//assign a ID from inspector
	public int sleep = 100;			//
	public int result = 0;			//each frame the result is added one

	protected override void ParallelUpdate () {
		Thread.Sleep(sleep);
		result ++;
//		if (MBP_Manager.isDebugOn)
//			Debug.Log("ID:"+ID + " done for Frame:" + MBP_Manager.frame + " Result: " + result);
	}

	public void NotMultithread() {
		ParallelUpdate();
	}

}
