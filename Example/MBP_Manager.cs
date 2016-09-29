

using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class MBP_Manager : MonoBehaviour {

	public bool isMultithreading;
	public static bool isDebugOn = true;
	public int calculationLoad = 200;
	TestMBP[] instances;
	Stopwatch sw = new Stopwatch ();

	public static int frame = 0;

	float[] elapsedTime;
	public int maxNumber = 10000;


	void Start () {
		elapsedTime = new float[maxNumber];
		sw.Reset();
		instances = Object.FindObjectsOfType<TestMBP>();
		foreach ( var i in instances) {
			i.sleep = calculationLoad;
		}
		sw.Start();
	}

	void LateUpdate() {
		if (isMultithreading) {
			sw.Stop();
			if (frame < maxNumber)
				elapsedTime[frame] = sw.Elapsed.Milliseconds;
			if (isDebugOn) Debug.LogWarning("Frame : " + frame + " Elapsed MS:" + sw.Elapsed.Milliseconds);
			sw.Reset();
			sw.Start();
			frame ++;
			TestMBP.UpdateAll();
		}
		else {
			sw.Reset();
			sw.Start();
			frame ++;
			foreach(var i in instances) {
				i.NotMultithread();
			}
			sw.Stop();
			if (frame < maxNumber)
				elapsedTime[frame] = sw.Elapsed.Milliseconds;
			if (isDebugOn) Debug.LogWarning("Frame : " + frame + " Elapsed MS:" + sw.Elapsed.Milliseconds);
		}
		

	}

	void OnPreCull() {
		if (isMultithreading == false) return;
		TestMBP.WaitAll();
		float totalTime=0f;
		for (int i = 0;i < maxNumber ; i++) {
			totalTime += elapsedTime[i];
		}
		if (frame >= maxNumber) {
			Debug.Log("Average Time:" + totalTime / maxNumber);
			Debug.Break();
		}
	}


}