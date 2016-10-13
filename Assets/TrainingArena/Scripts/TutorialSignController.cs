using UnityEngine;
using System;
using System.Collections;

public class TutorialSignController : MonoBehaviour 
{
	public delegate void SignFinished();
	public static event SignFinished signFinished;

	public Animator anim;
	public AudioClip accept;

	private AudioSource aud;
	private Typer typer;

	void Awake()
	{
		typer = GetComponent<Typer>();
		aud = GetComponent<AudioSource>();		
	}
	
	void Start()
	{
		typer.MessageDone += ChooseContinueMethod;	
	}

	// Chooses which method should be used to continue to next message
	void ChooseContinueMethod(int messageNum)
	{
		if(messageNum == 0)
			StartCoroutine(WaitForButtonPress());
		if(messageNum == 1)
			StartCoroutine(LookAtSigns());
		if(messageNum == 2)
			StartCoroutine(MoveAround());
		if(messageNum == 3)
			StartCoroutine(TurnAround180());
	}
		
	// Waits for user to press a button 
	IEnumerator WaitForButtonPress()
	{
		bool hasPressed = false;
		while(!hasPressed)
		{
			// A,B,X or Y on the gamepad.
			if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") ||
			Input.GetButtonDown("Fire3") || Input.GetButtonDown("Gamepad Y"))
			{
				aud.PlayOneShot(accept);
				if(typer.HasNextMessage)
					StartCoroutine(typer.TypeMessage());
				hasPressed = true;
			}
			yield return null;
		}
		yield return null;
	}

	// Look at both boats to continue
	IEnumerator LookAtSigns()
	{
		bool hasLookedLeft = false;
		bool hasLookedRight = false;

		while(!hasLookedLeft || !hasLookedRight)
		{
			Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);
			Vector3 origin = Camera.main.transform.position;
			RaycastHit hit;
			if(Physics.Raycast(origin,fwd, out hit, 1000))
			{
				// Clean this, not pretty with two lookalikes
				if(hit.collider.gameObject.name == "NotBoat1" && !hasLookedLeft)
				{
					hit.collider.gameObject.GetComponent<ChangeWhenLookedAt>().ChangeColor();
					aud.PlayOneShot(accept);
					hasLookedLeft = true;
				}
				if(hit.collider.gameObject.name == "NotBoat2" && !hasLookedRight)
				{
					hit.collider.gameObject.GetComponent<ChangeWhenLookedAt>().ChangeColor();
					aud.PlayOneShot(accept);
					hasLookedRight = true;
				}
			}
			yield return null;
		}
		if(typer.HasNextMessage)
			StartCoroutine(typer.TypeMessage());
	}

	IEnumerator MoveAround()
	{
		Vector3 startPos = Camera.main.transform.position;
		float moveDistance = 0f;
		while(moveDistance < .5f)
		{
			Vector3 curPos = Camera.main.transform.position;
			moveDistance += (curPos - startPos).magnitude;
			startPos = curPos;
			yield return null;
		}
		aud.PlayOneShot(accept);
		if(typer.HasNextMessage)
			StartCoroutine(typer.TypeMessage());
	}

	// Checks if user has turned 180 degrees
	IEnumerator TurnAround180()
	{
		bool hasTurned = false;
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
		yield return null;
	}
}
