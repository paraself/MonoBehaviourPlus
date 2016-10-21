using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviourPlus<Boid> {

	Vector3[] pos = new Vector3[2];
	Quaternion[] rotation = new Quaternion[2];

	public Vector3 Position;
	public Vector3 Velocity = Vector3.zero;

	BoidsManager manager;
	public BoidsManager SetManager {
		set {
			manager = value;
		}
	}

	Vector3 tpos;

	// Use this for initialization
	void Start () {
		Position = pos[0] = pos[1] = transform.position;
		rotation[0] = rotation[1] = transform.localRotation;
		tpos = transform.position;
		names = this.name;
	}

	int frameIndex;
	string names;
	protected override void ParallelUpdate () {

		
		//mass center
		Vector3 mv = (BoidsManager.massCenter - pos[frameIndex]).normalized * manager.massCenterFactor;

		//velovity
		Vector3 vv = Vector3.zero;
		var b = Boid.Instances;
		for (int i = 0;i<b.Count ; i++) {
			vv += b[i].Velocity;
		}
		vv /= b.Count;
		vv = vv.normalized * manager.velocityFactor;

		//repel
		Vector3 rv = Vector3.zero;
		for (int i = 0;i<b.Count ; i++) {
			if ((b[i].Position - this.pos[frameIndex]).magnitude < manager.repelRadius ) {
				rv += b[i].Position - this.pos[frameIndex];
			}
		}
		rv = rv.normalized * manager.repelFactor;

		pos[frameIndex] += (mv + vv + rv);
	}

	protected override void BeforeParallelUpdate (int frameIndex)
	{
		pos[frameIndex] = transform.position;
		rotation[frameIndex] = transform.rotation;
		frameIndex = frameIndex;
	}

	protected override void AfterParallelUpdate (int frameIndex)
	{
		Velocity = pos[frameIndex] - Position;
		Position = pos[frameIndex];
		transform.position = pos[frameIndex];
		transform.rotation = rotation[frameIndex];
	}
}
