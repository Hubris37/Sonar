using UnityEngine;
using System.Collections;

public class AddForce : MonoBehaviour {

	Rigidbody rigidbody;
	FireSoundWave blaster;
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		//blaster = FindObjectOfType<FireSoundWave> ();
		FireSoundWave.onBlastHit += RecieveForce;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}

	void RecieveForce(Vector3 hitPos, float pitchVal, float dbVal) {

		Vector3 heading = (transform.position - hitPos)*0.1f;
		float dist = heading.sqrMagnitude;
		Vector3 dir = heading / dist;
		//float dist = Vector3.Distance(hitPos, transform.position);
		rigidbody.AddForce(dir*(1/dist), ForceMode.Impulse);
	}

	void OnDestroy() {
		FireSoundWave.onBlastHit -= RecieveForce;
	}
}
