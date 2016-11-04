﻿using UnityEngine;
using System.Collections;

public class TrapCrush : MonoBehaviour {

	public GameObject crusher;
	public GameObject trigger;
	float attackTime = 2f;

	public AudioClip triggerSound;
	public AudioClip attackSound;
	public AudioClip hitGroundSound;

	bool attacking;
	bool lethal;

	public delegate void SoundBlastHit(Vector3 hitPos, float pitchVal, float dbVal);
	public static event SoundBlastHit onBlastHit;

	void Start () {
		crusher.SetActive (false);
	}

	IEnumerator trapTriggered() {
		crusher.SetActive (true);
		float triggerY = trigger.transform.position.y;
		Vector3 triggerSize = trigger.transform.localScale;
		trigger.transform.position += Vector3.down * 0.05f;
		trigger.transform.localScale = new Vector3 (trigger.transform.localScale.x, 0.05f, trigger.transform.localScale.z);
		Vector3 groundPos = new Vector3 (transform.position.x, 0, transform.position.z);
		AudioManager.instance.PlaySound(triggerSound, groundPos);
		yield return new WaitForSeconds (0.1f);
		AudioManager.instance.PlaySound(attackSound, transform.position);
	
		float percent = 0;
		float attackSpeed = 1 / attackTime;
		Vector3 initPos = crusher.transform.position;
		Vector3 attackPos = initPos + Vector3.down * 16;
		Vector3 initSize = crusher.transform.localScale;
		Vector3 attackSize = initSize + Vector3.up * 16;

		while (percent < 1) {
			percent += attackSpeed * Time.deltaTime;

			if (percent > 0.6f) {
				lethal = true;
			}

			crusher.transform.position = Vector3.Lerp (initPos, attackPos, percent);
			crusher.transform.localScale = Vector3.Lerp (initSize, attackSize, percent);
			yield return null;
		}

		AudioManager.instance.PlaySound(hitGroundSound, groundPos);
		onBlastHit (transform.position, 300f, 0.5f);
		yield return new WaitForSeconds (1f);
		lethal = false;
		percent = 0;

		while (percent < 1) {
			percent += attackSpeed * Time.deltaTime;
		
			crusher.transform.position = Vector3.Lerp (attackPos, initPos, percent);
			crusher.transform.localScale = Vector3.Lerp (attackSize, initSize, percent);
			yield return null;
		}

		trigger.transform.localScale = triggerSize;
		trigger.transform.position = new Vector3 (trigger.transform.position.x, triggerY, trigger.transform.position.z);
		attacking = false;
		crusher.SetActive (false);
	}

	void OnTriggerEnter(Collider triggerCollider) {

		if (triggerCollider.tag == "Player" || triggerCollider.tag == "MaskProjectile") {
			if (!attacking) {
				attacking = true;
				StartCoroutine (trapTriggered ());
			}
		}
	}

}
