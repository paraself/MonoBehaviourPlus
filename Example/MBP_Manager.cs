

using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class MBP_Manager : MonoBehaviour {

	public bool isMultithreading;
	public int calculationLoad = 200;
	TestMBP[] instances;
	Stopwatch sw = new Stopwatch ();


	void Start () {
		sw.Reset();
		instances = Object.FindObjectsOfType<TestMBP>();
		foreach ( var i in instances) {
			i.maxIteration = calculationLoad;
		}
	}

	void Update() {
		string s = string.Empty;
		for (int i = 0; i < instances.Length;i++) {
			s += ("Instance : " + instances[i].GetInstanceID() + " PI: " + instances[i].pi + "\n");
		}
		Debug.Log(s);
	}

	void LateUpdate() {
		sw.Reset();
		sw.Start();
		if (isMultithreading)
			TestMBP.WaitAndUpdate(10000000);
		else
			foreach(var i in instances) {
				i.NotMultithread();
			}
		sw.Stop();
		Debug.LogWarning("Elapsed MS:" + sw.Elapsed.Milliseconds);
	}
}