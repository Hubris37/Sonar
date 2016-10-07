using UnityEngine;
using System.Collections;

public class AddForceDoor : MonoBehaviour {

	Rigidbody myRigidbody;
	FireSoundWave blaster;
	public Vector3 offset;
	private Vector3 relativeOffset;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
		FireSoundWave.onBlastHit += RecieveForce;
		relativeOffset = transform.TransformPoint(offset);
	}

	/*
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
		relativeOffset = transform.TransformPoint(offset);
        Gizmos.DrawSphere(relativeOffset, 0.1f);
    }*/

	void RecieveForce(Vector3 hitPos, float pitchVal, float dbVal) {
		Vector3 myPos = transform.TransformPoint(offset);
		Vector3 heading = (myPos - hitPos);
		Vector2 xz = new Vector2(heading.x,heading.z);
		float dist = xz.sqrMagnitude;
		Vector3 dir = heading / dist;

		myRigidbody.AddForce(dir*(dbVal*250/(dist+20f)), ForceMode.Impulse);
	}

	void OnDestroy() {
		FireSoundWave.onBlastHit -= RecieveForce;
	}
}
