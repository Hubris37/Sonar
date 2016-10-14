using UnityEngine;
using System;
using System.Collections;

public class IntroTutorial : MonoBehaviour 
{
	public delegate void SignFinished();
	public static event SignFinished signFinished;

	public Animator anim;
	public AudioClip accept;

	public GameObject lookAroundImg;
	public GameObject moveAroundImg;
	public GameObject turnAroundImg;
	public GameObject anyButtonImg;

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
		anyButtonImg.SetActive(true);

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
		anyButtonImg.SetActive(false);
	}

	// Look at both boats to continue
	IEnumerator LookAtSigns()
	{
		bool hasLookedLeft = false;
		bool hasLookedRight = false;

		lookAroundImg.SetActive(true);
		lookAroundImg.GetComponent<Animator>().SetTrigger("FadeIn");

		while(!hasLookedLeft || !hasLookedRight)
		{
			Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);
			Vector3 origin = Camera.main.transform.position;
			RaycastHit hit;
			if(Physics.Raycast(origin,fwd, out hit, 1000))
			{
				// Clean this, not pretty with two lookalikes
				if(hit.collider.gameObject.name == "NotBoatLeft" && !hasLookedLeft)
				{
					hit.collider.gameObject.GetComponent<ChangeWhenLookedAt>().ChangeColor();
					aud.PlayOneShot(accept);
					hasLookedLeft = true;
				}
				if(hit.collider.gameObject.name == "NotBoatRight" && !hasLookedRight)
				{
					hit.collider.gameObject.GetComponent<ChangeWhenLookedAt>().ChangeColor();
					aud.PlayOneShot(accept);
					hasLookedRight = true;
				}
			}
			yield return null;
		}
		lookAroundImg.SetActive(false);
	}

	// Move a smal distance to continue
	IEnumerator MoveAround()
	{
		Vector3 startPos = Camera.main.transform.position;
		float moveDistance = 0f;

		moveAroundImg.SetActive(true);

		while(moveDistance < .5f)
		{
			Vector3 curPos = Camera.main.transform.position;
			moveDistance += (curPos - startPos).magnitude;
			startPos = curPos;
			yield return null;
		}
		aud.PlayOneShot(accept);
		moveAroundImg.SetActive(false);
	}

	// Checks if user has turned 180 degrees
	IEnumerator TurnAround180()
	{
		bool hasTurned = false;

		moveAroundImg.SetActive(true);

		while(!hasTurned)
		{
			Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);
			RaycastHit hit;
			if(Physics.Raycast(transform.position,fwd, out hit, 1000))
			{
				Debug.Log(hit.collider.gameObject.name);
				if(hit.collider.gameObject.name == "StartSoundTutorialPlane")
				{
					aud.PlayOneShot(accept);
					hasTurned = true;
				}
			}
			yield return null;
		}
		moveAroundImg.SetActive(false);
	}
}