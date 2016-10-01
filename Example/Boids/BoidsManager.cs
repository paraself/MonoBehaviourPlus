//Boids algorithm biref
//1. individuals always try to move to the mass center
//2. individuals always try to align its own velocity with the others
//3. individuals always maintain a safe radius with each other


using UnityEngine;
using System.Collections;

public class BoidsManager : MonoBehaviour {

	public GameObject boidPrefab;
	public GameObject boidsParent;
	public CircleCollider2D birthZone;


	[Range(10,100)]
	public int maxBoidsNumber = 100;
	int currentBoidsNumber;

	// Use this for initialization
	void Start () {
		InvokeRepeating("AddBoid",0f,0.5f);
	}
	

	void Update () {
		
	}

	void AddBoid() {
		if (currentBoidsNumber >= maxBoidsNumber) return;
		Vector3 pos = (Vector3)Random.insideUnitCircle * birthZone.radius * birthZone.transform.localScale.x + birthZone.transform.position;
		GameObject boid = Instantiate<GameObject>(boidPrefab);
		boid.transform.position = pos;
		boid.transform.parent = boidsParent.transform;
		currentBoidsNumber++;

	}
}
