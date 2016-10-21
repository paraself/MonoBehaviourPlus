/// <summary>
/// Mono behaviour plus is a multi-threaded monobehaviour wrapper
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Reflection;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public abstract class MonoBehaviourPlus<T> : MonoBehaviour where T : MonoBehaviourPlus<T> {

	#region derived class instance
	T _instance = null;
	public T Instance {
		get {
			if (_instance == null) _instance = (T)this;
			return _instance;
		}
	}
	#endregion

	const bool IS_DEBUG_ON = false;
	
	static List<T> instances = new List<T> ();
	public static List<T> Instances {
		get {
			return instances;
		}
	}

	static int eventIndex = 0;
	static int prevEventIndex = 1;
	int _eventIndex;
	AutoResetEvent controllerEvent = new AutoResetEvent (false);
	AutoResetEvent[] _event = new AutoResetEvent[2] {new AutoResetEvent (true),new AutoResetEvent (true)}; 
//	static WaitHandle[][] _events = new WaitHandle[2][] ;
//	static AutoResetEvent[] controllerEvents;
	WaitCallback updateDelegate;
	bool isInited = false;
	public bool isPrallelUpdateEnabled = true;
	public enum WaitType {Current,Previous};

	void Begin () {
		if (Application.isPlaying) {
			updateDelegate = WaitCallback;
			bool s = ThreadPool.QueueUserWorkItem(updateDelegate);
			if (!s) {
				Debug.LogError("Cannot queue " 
					+ Instance.GetType().ToString() 
					+ " : " + Instance.GetInstanceID() 
					+ " to update list, parallel updating for " 
					+ Instance.GetType().ToString()
					+ " is disbaled!"
				);
				isPrallelUpdateEnabled = false;
			} else {
				instances.Add(Instance);
				isInited = true;
				isPrallelUpdateEnabled = true;
				//PurgeNullInstances();

			}
		}
	}

	void End () {
		if (Application.isPlaying) {
			isPrallelUpdateEnabled = false;
			instances.Remove(Instance);
			_event[0].Set();//in case any thread is waiting on this event
			_event[0].Close();
			_event[1].Set();
			_event[1].Close();
			isInited = false;
		}
	}

	protected virtual void Awake () {
		if (isInited==false) Begin();
	}

	protected virtual void OnEnable () {
		if (isInited==false) Begin();
	}

	protected virtual void OnDisable () {
		if (isInited==true) End();
	}

	protected virtual void OnDestroy () {
		if (isInited==true) End();
	}

	protected abstract void ParallelUpdate();

	/// <summary>
	/// This method is called before parallel update, you can put in Unity's api inside this method
	/// </summary>
	protected virtual void BeforeParallelUpdate(int frameIndex) {}

	/// <summary>
	/// This method is called after WaitAll is complelete. You can put in any Unity's api inside this method
	/// </summary>
	protected virtual void AfterParallelUpdate(int frameIndex) {}


	void WaitCallback(object o) {
		while (isPrallelUpdateEnabled) {
			controllerEvent.WaitOne();
			_eventIndex = eventIndex;
			ParallelUpdate ();
			_event[_eventIndex].Set();
		}
	}

	static void PurgeNullInstances() {
//		bool token = false;
		for (int i = instances.Count - 1;i>=0;i--) {
			if (instances[i] == null) {
				instances.RemoveAt(i);
//				token = true;
			}
		}
//		if (token || isForceUpdate || _events == null || controllerEvents == null) {
//			_events[0] = new WaitHandle[instances.Count];
//			_events[1] = new WaitHandle[instances.Count];
//			controllerEvents = new AutoResetEvent[instances.Count];
//			for (int i = 0;i<instances.Count;i++) {
//				_events[0][i] = instances[i]._event[0];
//				_events[1][i] = instances[i]._event[1];
//				controllerEvents[i] = instances[i].controllerEvent;
//			}
//		}
	}

	public static void WaitAll(int timeOutInMS = 5000,WaitType waitType = WaitType.Previous) {
		PurgeNullInstances();
		if (IS_DEBUG_ON) Debug.LogWarning("About to wait!");
		int i = (waitType == WaitType.Previous ) ? prevEventIndex : eventIndex;
		//WaitHandle.WaitAll(_events[i],timeOutInMS);
		for (int j =0;j<instances.Count;j++) {
			instances[j]._event[i].WaitOne(timeOutInMS);
		}
		for (int j =0;j<instances.Count;j++) {
			instances[j].AfterParallelUpdate(i);
		}
		if (IS_DEBUG_ON) Debug.LogWarning("Waiting finished for frame:" + MBP_Manager.frame);
	}

	public static void UpdateAll() {
		prevEventIndex = eventIndex;
		eventIndex = (eventIndex + 1) % 2;
		if (IS_DEBUG_ON) Debug.LogWarning(" about to signal work thread to perform task!");
		//Debug.Log(instances.Count);
		for (int i = 0;i<instances.Count;i++) {
			instances[i].BeforeParallelUpdate(eventIndex);
			instances[i].controllerEvent.Set();
		}
		if (IS_DEBUG_ON) Debug.LogWarning("set to non-signal so work thread only perform once!");
	}

}
