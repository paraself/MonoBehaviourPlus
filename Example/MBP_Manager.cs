

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


	void Start () {
		sw.Reset();
		instances = Object.FindObjectsOfType<TestMBP>();
		foreach ( var i in instances) {
			i.sleep = calculationLoad;
		}
		sw.Start();
	}

	void LateUpdate() {
		if (isMultithreading) {
			//TestMBP.Wait();
			sw.Stop();
			if (isDebugOn) Debug.LogWarning("Elapsed MS:" + sw.Elapsed.Milliseconds);
			sw.Reset();
			sw.Start();
			frame ++;
			TestMBP.UpdateAll();
		}
		else {
			sw.Reset();
			sw.Start();
			foreach(var i in instances) {
				i.NotMultithread();
			}
			sw.Stop();
			if (isDebugOn) Debug.LogWarning("Elapsed MS:" + sw.Elapsed.Milliseconds);
		}
		

	}

	void OnWillRenderObject () {
		TestMBP.Wait();
	}


}