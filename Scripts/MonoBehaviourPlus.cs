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
	AutoResetEvent[] _event = new AutoResetEvent[2] {new AutoResetEvent (true),new AutoResetEvent (false)}; 
	static int eventIndex = 0;
	static int prevEventIndex = 0;
	AutoResetEvent controllerEvent = new AutoResetEvent (false);
	static WaitHandle[][] _events = new WaitHandle[2][] ;
	static AutoResetEvent[] controllerEvents;
	WaitCallback updateDelegate;
	bool isInited = false;
	public bool isPrallelUpdateEnabled = true;
	enum WaitType {Current,Previous};

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
				PurgeNullInstances(true);
			}
		}
	}

	void End () {
		if (Application.isPlaying) {
			isPrallelUpdateEnabled = false;
			instances.Remove(Instance);
			_event[0].Close();
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


	void WaitCallback(object o) {
		while (isPrallelUpdateEnabled) {
			controllerEvent.WaitOne();
			ParallelUpdate ();
			prevEventIndex = eventIndex;
			eventIndex = (eventIndex + 1) % 2;
			_event[eventIndex].Set();
		}
	}



	static void PurgeNullInstances(bool isForceUpdate = false) {
		bool token = false;
		for (int i = instances.Count - 1;i>=0;i--) {
			if (instances[i] == null) {
				instances.RemoveAt(i);
				token = true;
			}
		}
		if (token || isForceUpdate || _events == null || controllerEvents == null) {
			_events[0] = new WaitHandle[instances.Count];
			_events[1] = new WaitHandle[instances.Count];
			controllerEvents = new AutoResetEvent[instances.Count];
			for (int i = 0;i<instances.Count;i++) {
				_events[0][i] = instances[i]._event[0];
				_events[1][i] = instances[i]._event[1];
				controllerEvents[i] = instances[i].controllerEvent;
			}
		}
	}

	public static void WaitPrevious(int timeOutInMS = 1000,WaitType waitType = WaitType.Previous) {
		PurgeNullInstances();
		if (IS_DEBUG_ON) Debug.LogWarning("About to wait!");
		int i = (waitType == WaitType.Previous ) ? prevEventIndex : eventIndex;
		WaitHandle.WaitAll(_events[i],timeOutInMS);
		if (IS_DEBUG_ON) Debug.LogWarning("Waiting finished for frame:" + MBP_Manager.frame);
	}

	public static void UpdateAll() {
		if (IS_DEBUG_ON) Debug.LogWarning("About to signal work thread to perform task!");
		for (int i = 0;i<controllerEvents.Length;i++) controllerEvents[i].Set();
		if (IS_DEBUG_ON) Debug.LogWarning("set to non-signal so work thread only perform once!");
	}

}
