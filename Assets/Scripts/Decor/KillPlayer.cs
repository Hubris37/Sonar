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
	}
}
