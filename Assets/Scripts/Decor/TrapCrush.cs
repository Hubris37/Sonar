using UnityEngine;
using System.Collections;

public class TrapCrush : MonoBehaviour {

	public GameObject crusher;
	BoxCollider trigger;
	public float attackTime; 

	public AudioClip triggerSound;
	public AudioClip attackSound;
	public AudioClip hitGroundSound;

	bool attacking;
	bool lethal;

	void Start () {
		trigger = GetComponentInChildren<BoxCollider> ();
	}

	IEnumerator trapTriggered() {
		AudioManager.instance.PlaySound(triggerSound, transform.position);
		yield return new WaitForSeconds (0.5f);
		AudioManager.instance.PlaySound(attackSound, transform.position);
	
		float percent = 0;
		float attackSpeed = 1 / attackTime;
		Vector3 initPos = crusher.transform.position;
		Vector3 attackPos = initPos + Vector3.down * 2;
		Vector3 initSize = crusher.transform.localScale;
		Vector3 attackSize = initSize + Vector3.down * 2;

		while (percent < 1) {
			percent += attackSpeed * Time.deltaTime;

			if (percent > 0.6f) {
				lethal = true;
			}

			crusher.transform.position = Vector3.Lerp (initPos, attackPos, percent);
			crusher.transform.localScale = Vector3.Lerp (initSize, attackSize, percent);
			yield return null;
		}

		AudioManager.instance.PlaySound(hitGroundSound, transform.position);
		yield return new WaitForSeconds (1f);
		lethal = false;
		AudioManager.instance.PlaySound(attackSound, transform.position);
		percent = 0;

		while (percent < 1) {
			percent += attackSpeed * Time.deltaTime;
		
			crusher.transform.position = Vector3.Lerp (attackPos, initPos, percent);
			crusher.transform.localScale = Vector3.Lerp (attackSize, initSize, percent);
			yield return null;
		}

		attacking = false;
	}

	void OnTriggerEnter(Collider triggerCollider) {

		if (triggerCollider.tag == "Player") {
			if (!attacking) {
				attacking = true;
				StartCoroutine (trapTriggered ());
			}
		}
	}

}
