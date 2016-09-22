//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System;
//
//[DisallowMultipleComponent]
//public class MBP_Manager : MonoBehaviour {
//
//	#region Singleton Instance
//	static MBP_Manager _mInstance;
//	public static MBP_Manager Instance {
//		get {
//			if(_mInstance == null)
//			{	
//				MBP_Manager [] managers = Component.FindObjectsOfType<MBP_Manager>();//GameObject.FindObjectsOfType(typeof(T)) as T[];
//				if(managers.Length != 0)
//				{
//					if(managers.Length == 1)
//					{
//						_mInstance = managers[0];
//						DontDestroyOnLoad(_mInstance);
//						//_mInstance.gameObject.name = typeof(T).Name;//dont change the name
//						//Debug.Log("Found one singleton instance for " + typeof(T).Name);
//						return _mInstance;
//					} else {
//						Debug.LogError("You have more than one MBP_Manager in the scene. \n You only need 1, it's a singleton! \n Will return the first one and destroy the others!");
//						_mInstance = managers[0];
//						for (int i = 1; i<managers.Length;i++) {
//							Destroy(managers[i].gameObject);
//						}
//						return _mInstance;
//					}
//				}
//				Debug.LogError("Singleton Manager cannot find instance for MBP_Manager \n Will make a new one!");
//				_mInstance = new GameObject("MBP Manager").AddComponent<MBP_Manager>();
//				return _mInstance;
//			} else return _mInstance;
//		}
//	}
//	#endregion
//
//	static Dictionary<Type,List<MonoBehaviourPlus>> mbps = new Dictionary<Type, List<MonoBehaviourPlus>> ();
//
//	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
//	static void Init () {
//		if (Application.isPlaying) {
//			Debug.Log(Instance.name + " is initing ... ");
//		}
//	}
//
//	public static void Add ( MonoBehaviourPlus mbp) {
//		Type t = mbp.GetType();
//		List<MonoBehaviourPlus> mbpss;
//		mbps.TryGetValue(t,out mbpss);
//		if (mbpss != null) {
//			mbpss.Add(mbp);
//		} else {
//			var l = new List<MonoBehaviourPlus> ();
//			l.Add(mbp);
//			mbps.Add(t,l);
//		}
//	}
//
//	public static void Remove ( MonoBehaviourPlus mbp) {
//		Type t = mbp.GetType();
//		List<MonoBehaviourPlus> mbpss;
//		mbps.TryGetValue(t,out mbpss);
//		if (mbpss != null) {
//			mbpss.Remove(mbp);
//		}
//	}
//
//	void Awake() {
//		
//	}
//
//	#if UNITY_EDITOR
//
//	#endif
//}

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
			s += ("Instance : " + this.GetInstanceID() + " PI: " + instances[i].pi + "\n");
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