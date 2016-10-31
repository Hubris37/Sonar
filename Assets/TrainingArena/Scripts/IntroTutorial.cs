using UnityEngine;
using System;
using System.Collections;

public class IntroTutorial : MonoBehaviour 
{
	//public Animator anim;
	public AudioClip accept;

	public GameObject lookAroundExp;
	public GameObject lookAroundTargets;
	public GameObject moveAroundExp;
	public GameObject turnAroundExp;
	public GameObject anyButtonExp;

	private AudioSource aud;
	private Typer typer;

	void Awake()
	{
		typer = GetComponent<Typer>();
		aud = GetComponent<AudioSource>();
	}
	
	// Plays the turorial in the right order.
	public IEnumerator StartTutorial()
	{
		Debug.Log("Hej");
		// Intro
		yield return StartCoroutine(typer.TypeMessage(0));
		yield return StartCoroutine(WaitForButtonPress());

		// Look around
		yield return StartCoroutine(typer.TypeMessage(1));
		yield return StartCoroutine(LookAtSigns());
		
		// Move around
		yield return StartCoroutine(typer.TypeMessage(2));
		yield return StartCoroutine(MoveAround());

		// Turn around
		yield return StartCoroutine(typer.TypeMessage(3));
		yield return StartCoroutine(TurnAround180());

		// Fulhack to let sound effect finish
		yield return new WaitForSeconds(0.2f);
		gameObject.SetActive(false);
	}

	// Waits for user to press a button 
	IEnumerator WaitForButtonPress()
	{
		bool hasPressed = false;
		anyButtonExp.SetActive(true);

		while(!hasPressed)
		{
			// A,B,X or Y on the gamepad.
			if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") ||
			Input.GetButtonDown("Fire3") || Input.GetButtonDown("Gamepad Y"))
			{
				aud.PlayOneShot(accept);
				hasPressed = true;
			}
			yield return null;
		}
		anyButtonExp.SetActive(false);
	}

	// Look at both boats to continue
	IEnumerator LookAtSigns()
	{
		bool hasLookedLeft = false;
		bool hasLookedRight = false;

		lookAroundExp.SetActive(true);
		lookAroundTargets.SetActive(true);
		
		lookAroundExp.GetComponent<Animator>().SetTrigger("FadeIn");
		
		// Using legacy animation for being able to check if animation is playing.
		// Don't know how to do it with state-of-the-art Animator
		Animation animTarg = lookAroundTargets.GetComponent<Animation>(); 
		animTarg["LookAtTargetsMoveIn"].speed = 0.6f;
		animTarg.Play("LookAtTargetsMoveIn");

		while(!hasLookedLeft || !hasLookedRight)
		{
			Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);
			Vector3 origin = Camera.main.transform.position;
			RaycastHit hit;
			if(Physics.Raycast(origin,fwd, out hit, 1000))
			{
				// Clean this, not pretty with two lookalikes
				if(hit.collider.gameObject.transform.parent.name == "LeftLookAt" && !hasLookedLeft)
				{
					hit.collider.gameObject.GetComponent<ChangeWhenLookedAt>().ChangeColor();
					aud.PlayOneShot(accept);
					hasLookedLeft = true;
				}
				if(hit.collider.gameObject.transform.parent.name == "RightLookAt" && !hasLookedRight)
				{
					hit.collider.gameObject.GetComponent<ChangeWhenLookedAt>().ChangeColor();
					aud.PlayOneShot(accept);
					hasLookedRight = true;
				}
			}
			yield return null;
		}
		lookAroundExp.GetComponent<Animator>().SetTrigger("FadeOut");

		animTarg["LookAtTargetsMoveOut"].speed = 0.6f;
		animTarg.Play("LookAtTargetsMoveOut");

		while(animTarg.isPlaying) yield return null;

		lookAroundExp.SetActive(false);
		lookAroundTargets.SetActive(false);
	}

	// Move a smal distance to continue
	IEnumerator MoveAround()
	{
		Vector3 startPos = Camera.main.transform.position;
		float moveDistance = 0f;

		moveAroundExp.SetActive(true);

		while(moveDistance < .5f)
		{
			Vector3 curPos = Camera.main.transform.position;
			moveDistance += (curPos - startPos).magnitude;
			startPos = curPos;
			yield return null;
		}
		aud.PlayOneShot(accept);
		moveAroundExp.SetActive(false);
	}

	// Checks if user has turned 180 degrees
	IEnumerator TurnAround180()
	{
		bool hasTurned = false;

		turnAroundExp.SetActive(true);

		while(!hasTurned)
		{
			Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);
			RaycastHit hit;
			if(Physics.Raycast(transform.position,fwd, out hit, 1000))
			{
				if(hit.collider.gameObject.name == "StartSoundTutorialPlane")
				{
					aud.PlayOneShot(accept);
					hasTurned = true;
				}
			}
			yield return null;
		}
		turnAroundExp.SetActive(false);
	}
}