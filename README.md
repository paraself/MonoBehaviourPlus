# MonoBehaviourPlus
Multi-threaded monobehaviour for Unity3D  
Usage: 
 
```c#
using UnityEngine;
using System.Collections;

public class TestMBP : MonoBehaviourPlus<TestMBP> {

	public double pi;
	public int maxIteration = 100;

	protected override void ParallelUpdate () {
		pi = 2 * F(1);
	}

	public void NotMultithread() {
		ParallelUpdate();
	}

	double F (int i) {
		//some heavy delay
		for (int p = 0;p<maxIteration * 10;p++) {
			float o = Mathf.Sqrt(p);
		}
		//some heavy delay
		if ( i <= maxIteration)
			return 1 + i / (2.0 * i + 1) * F(i + 1);
		else return 0;
	}

}

```
Derived a class from ``MonoBehaviourPlus<T>``, and implement an abstract method called ``ParallelUpdate``. Put the heavy calculation load into this method without putting in any Unity api, because Unity's api cannot be used in user threads.  

Then in a manager script, call:

```c#
TestMBP.WaitAndUpdate();
```
to wait for the previous frame's parallel updates, and then perform the parallel updates for the current frame. This makes the parallel update run across entire game update loop.  
Alternatively, you can:

```c#
TestMBP.Wait();
```
This will make the manager script which is running on the Unity main thread to wait for all the TestMBP's instances' parallel update. And then you can call:

```c#
TestMBP.UpdateAll();
```
to run all the instances' parallel update metho multi-threadedly.  
Usually ``WaitAndUpdate()`` have better performace and is recommended to use.