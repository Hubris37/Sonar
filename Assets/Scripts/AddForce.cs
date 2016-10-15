using UnityEngine;
using System.Collections;

public class AddForce : MonoBehaviour {

	Rigidbody myRigidbody;
	FireSoundWave blaster;
	public Vector3 offset;
	//private Vector3 relativeOffset; // never used

	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
		FireSoundWave.onBlastHit += RecieveForce;
		//relativeOffset = transform.TransformPoint(offset);
	}

	void RecieveForce(Vector3 hitPos, float pitchVal, float dbVal) {
		Vector3 myPos = transform.TransformPoint(offset);
		Vector3 heading = (myPos - hitPos);
		float dist = heading.sqrMagnitude;
		Vector3 dir = heading / dist;

		myRigidbody.AddForce(dir*(dbVal*250/(dist+20f)), ForceMode.Impulse);
	}

	void OnDestroy() {
		FireSoundWave.onBlastHit -= RecieveForce;
	}
}
