using UnityEngine;
using System.Collections;

public class AddForce : MonoBehaviour {

	Rigidbody rigidbody;
	FireSoundWave blaster;
	public Vector3 offset;
	private Vector3 relativeOffset;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		//blaster = FindObjectOfType<FireSoundWave> ();
		FireSoundWave.onBlastHit += RecieveForce;
		relativeOffset = transform.TransformPoint(offset);//transform.InverseTransformPoint(transform.position + offset);

	}
	/*
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
		relativeOffset = transform.TransformPoint(offset);
        Gizmos.DrawSphere(relativeOffset, 0.1f);
    }*/

	
	// Update is called once per frame
	void FixedUpdate () {
		
	}

	void RecieveForce(Vector3 hitPos, float pitchVal, float dbVal) {
		Vector3 myPos = relativeOffset;// + relativeOffset;// + (transform.localPosition + offset);
		Vector3 heading = (myPos - hitPos);
		float dist = heading.sqrMagnitude;
		Vector3 dir = heading / dist;
		//float dist = Vector3.Distance(hitPos, transform.position);
		rigidbody.AddForce(dir*(1/(dist+0.1f)), ForceMode.Impulse);
	}

	void OnDestroy() {
		FireSoundWave.onBlastHit -= RecieveForce;
	}
}
