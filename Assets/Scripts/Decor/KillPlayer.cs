using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour {

	GameManager gameM;

	// Use this for initialization
	void Start () {
		gameM = FindObjectOfType<GameManager> ();
	}
	
	void OnTriggerEnter(Collider triggerCollider) {

		if (triggerCollider.tag == "Player") {
			gameM.LostGame ();
		}

		if (triggerCollider.tag == "Enemy") {
			Vector3 x = triggerCollider.gameObject.transform.localScale;
			triggerCollider.gameObject.transform.localScale = new Vector3(x.x, x.y/2, x.z);
		}
	}
}
