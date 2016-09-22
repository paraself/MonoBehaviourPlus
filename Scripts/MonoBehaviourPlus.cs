/// <summary>
/// Mono behaviour plus is a multi-threaded monobehaviour wrapper,
/// Using Curiously Recurring Template Pattern
/// http://stackoverflow.com/questions/1327568/curiously-recurring-template-pattern-and-generics-constraints-c
/// http://stackoverflow.com/questions/10939907/how-to-write-a-good-curiously-recurring-template-pattern-crtp-in-c-sharp
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
	
	static List<T> instances = new List<T> ();
	//ManualResetEvent _event = new ManualResetEvent (false);
	WaitCallback updateDelegate;
	bool isDone = true;
	static Stopwatch sw = new Stopwatch ();

	protected virtual void Awake () {
		if (Application.isPlaying) {
			instances.Add(Instance);
			updateDelegate = (object obj) => {
				ParallelUpdate();
				//_event.Set();
				isDone = true;
			};
		}
	}

	protected virtual void OnDestroy () {
		if (Application.isPlaying) instances.Remove(Instance);
	}

	protected abstract void ParallelUpdate();

	static void PurgeNullInstances() {
		for (int i = instances.Count - 1;i>=0;i--) {
			if (instances[i] == null) instances.RemoveAt(i);
		}
	}

	/// <summary>
	/// Wait for the update deployed in the previous frame, and then deploy update for the current frame
	/// </summary>
	/// <param name="timeOutInMS">Time out in MillieSeconds</param>
	/// <param name="isUpdate">If set to <c>true</c> Update will be queued after wait.</param>
	public static void WaitAndUpdate(int timeOutInMS = 1000,bool isUpdate = true) {
		PurgeNullInstances();
		int instancesCount = instances.Count;
		int count = 0;
		sw.Reset();
		sw.Start();
		do {
			for (int i = 0;i < instances.Count;i++) {
				if (instances[i].isDone) {
					if (isUpdate) {
						instances[i].isDone = false;
						ThreadPool.QueueUserWorkItem(instances[i].updateDelegate);
					}
					count++;
				}
			}
			if (count == instancesCount) return;
			else {
				sw.Stop();
				if (sw.Elapsed.Milliseconds > timeOutInMS ) {
					for (int i = 0;i<instances.Count;i++) instances[i].isDone = true;
					Debug.LogError("Time out!");
					return;
				}
				sw.Start();
			}
		} while (count < instancesCount );
	}

	/// <summary>
	/// Wait the Update
	/// </summary>
	/// <param name="timeOutInMS">Time out in M.</param>
	public static void Wait(int timeOutInMS = 1000) {
		WaitAndUpdate(timeOutInMS,false);
	}

	/// <summary>
	/// Updates all instances.
	/// </summary>
	public static void UpdateAll() {
		PurgeNullInstances();
		for (int i = 0;i < instances.Count;i++) {
			if (instances[i].isDone) {
				instances[i].isDone = false;
				ThreadPool.QueueUserWorkItem(instances[i].updateDelegate);
			}
		}
	}

}
