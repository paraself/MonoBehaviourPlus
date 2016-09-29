# MonoBehaviourPlus
Multi-threaded monobehaviour for Unity3D  
##Usage

```c#
using UnityEngine;
using System.Collections;

public class MyMonoBehaviourType : MonoBehaviourPlus<MyMonoBehaviourType> {

	//abstract method that you must implement
	protected override void ParallelUpdate () {
		//do your threaded work here, cannot use any unity api inside this
		//...

	}
}
```
Derive a class from ``MonoBehaviourPlus<T>``, and implement an abstract method called ``ParallelUpdate``. Put the heavy calculation load into this method without putting in any Unity api, because Unity's api cannot be used in user threads.  Then in a singleton manager script, you can call two apis to manipulate the parallel update behaviour.

##API

```c#
MyMonoBehaviourType.UpdateAll();
```
This will update the `ParallelUpdate` methods of all the instances multi-threadedly.


```c#
MyMonoBehaviourType.WaitAll(int timeOut = 1000,WaitType waitType = WaitType.Previous);
```
This will make the manager script, which is running on the Unity main thread, to wait for all the instances' parallel updates.
Note there are two parameters that define the waiting behaviour. `timeOut` let you sepcify the maximum waiting time. Enumeration `WaitType` let you specify the waiting type. You can wait for the completion of the previous `UpdateAll` or the current `UpdateAll`. When you call `WaitAll` , the **current** means the last called `UpdateAll` before this `WaitAll`; the **previous** means the one before the **current**. By supporting waiting for both current and preivous `UpdateAll`, it's possible to achieve double frame waiting and make the parallel update span over two frames, thus improve performance in the cost of one or two frames of rendering lag.

##Example
Let's take a game loop update for example.
First, within a game loop, there are four ways to combine `WaitAll` and `UpdateAll`. These are:

```c#
//Type 1 : frame n+2 waits for the UpdateAll called in frame n then call its own UpdateAll.
WaitAll(WaitType.Previous);
UpdateAll();				
```
```c#
//Type 2 : frame n+1 waits for the UpdateAll called in frame n then call its own UpdateAll.
WaitAll(WaitType.Current);
UpdateAll();
```
```c#
//Type 3 : frame n+1 call its own UpdateAll then wait for the UpdateAll called in frame n.
UpdateAll();
WaitAll(WaitType.Previous);
```
```c#
//Type 4 : frame n call its own UpdateAll then wait for its own calling of UpdateAll.
UpdateAll();
WaitAll(WaitType.Current);
```
Take Type 1 for instance, the actual usage of Type 1 might look like this:
```c#
//attach this script onto the main camera
public class SingletonManager : MonoBehaviour {
  void Update() {
    MyMonoBehaviourType.WaitAll(WaitType.Previous);
  }
  void OnPreCull() {
    MyMonoBehaviourType.UpdateAll();
  }
}
```
These four types of combination have different waiting behaviour.
- Type 1 : frame n+2 waits for the UpdateAll called in frame n then call its own UpdateAll.

- Type 2 : frame n+1 waits for the UpdateAll called in frame n then call its own UpdateAll.

- Type 3 : frame n+1 call its own UpdateAll then wait for the UpdateAll called in frame n.

- Type 4 : frame n call its own UpdateAll then wait for its own calling of UpdateAll.

Because when Unity renders gameObjects, it's also possible that ParallelUpdate is still working. So it's important to make a local copy of the member variables onto which `ParallelUpdate` is manipulating.

## Benchmark

The test results of different group are not strickly comparable. Because in multi-threaded test, I use Type 3 to wait for the previous frame, while in non-multithreaded test, the parallel updates are done within a frame.

Update 10 instances for 300 frames on Macbook Air 11 inch 2013.

| ParallelUpdate Load (ms) | Multithreaded Type 3 (ms) | Non-multithreaded (ms) | %    |
| ------------------------ | ------------------------- | ---------------------- | ---- |
| 5                        | 28                        | 56                     | 50   |
| 10                       | 26                        | 110                    | 23   |
| 20                       | 30                        | 210                    | 14.3 |
| 30                       | 35.5                      | 310                    | 11.5 |
| 50                       | 54                        | 510                    | 10.6 |


