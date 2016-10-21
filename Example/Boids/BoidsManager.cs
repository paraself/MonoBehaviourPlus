//Boids algorithm biref
//1. individuals always try to move to the mass center
//2. individuals always try to align its own velocity with the others
//3. individuals always maintain a safe radius with each other


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class BoidsManager : MonoBehaviour {

	public GameObject boidPrefab;
	public GameObject boidsParent;
	public CircleCollider2D birthZone;
	public GameObject massCenterPrefab;

	public static Vector3 massCenter;

	[Range(10,100)]
	public int maxBoidsNumber = 100;

	[Range(0f,1f)]
	public float massCenterFactor = 1f;

	[Range(0f,1f)]
	public float velocityFactor = 1f;

	[Range(0f,1f)]
	public float repelFactor = 1f; 

	[Range(1f,20f)]
	public float repelRadius = 10f; 


	int currentBoidsNumber;

	// Use this for initialization
	void Start () {
		InvokeRepeating("AddBoid",0f,0.5f);
	}

	void Update () {
		CalcMassCenter();
		Boid.UpdateAll();
	}

	void AddBoid() {
		if (currentBoidsNumber >= maxBoidsNumber) return;
		Vector3 pos = (Vector3)Random.insideUnitCircle * birthZone.radius * birthZone.transform.localScale.x + birthZone.transform.position;
		GameObject boid = Instantiate<GameObject>(boidPrefab);
		boid.GetComponent<Boid>().SetManager = this;
		boid.transform.position = pos;
		boid.transform.parent = boidsParent.transform;
		boid.name = "Boid " + currentBoidsNumber.ToString();
		currentBoidsNumber++;

	}

	void CalcMassCenter () {
		List<Boid> b = Boid.Instances;
		int count = Boid.Instances.Count;
		massCenter = Vector3.zero;
		for (int i = 0;i<count; i++ ) {
			massCenter += b[i].transform.position;
		}
		massCenter /= count;
		massCenterPrefab.transform.position = massCenter;
	}

	void OnPreCull () {
		Boid.WaitAll(100,MonoBehaviourPlus<Boid>.WaitType.Previous);
		//Debug.LogWarning("waiting done");
	}
}
