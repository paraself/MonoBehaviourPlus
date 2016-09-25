# MonoBehaviourPlus
Multi-threaded monobehaviour for Unity3D  
Usage: 
 
```c#
using UnityEngine;
using System.Collections;

public class TestMBP : MonoBehaviourPlus<TestMBP> {

	public double pi;
	public int maxIteration = 100;
	
	//abstract method that you must implement
	protected override void ParallelUpdate () {
		//do your threaded work here, cannot use any unity api inside this
		//...
		
	}
	
	//virtual method that you can override
	protected override void BeforeParallelUpdate () {
	
	}
	
	//virtual method that you can override
	protected override void AfterParallelUpdate () {
	
	}

}

```
Derived a class from ``MonoBehaviourPlus<T>``, and implement an abstract method called ``ParallelUpdate``. Put the heavy calculation load into this method without putting in any Unity api, because Unity's api cannot be used in user threads.  

Then in a manager script, call:

```c#
TestMBP.Wait();
```
This will make the manager script which is running on the Unity main thread to wait for all the TestMBP's instances' parallel update. And then you can call:

```c#
TestMBP.UpdateAll();
```
to run all the instances' parallel update metho multi-threadedly.