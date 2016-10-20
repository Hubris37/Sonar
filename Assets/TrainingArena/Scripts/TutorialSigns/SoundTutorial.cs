using UnityEngine;
using System;
using System.Collections;

public class SoundTutorial : MonoBehaviour 
{
	public GameObject goal;
	public AudioClip accept;
	public LightTrigger lights;
	public GameObject anyButtonImg;
	public GameObject gramophonePillar;
	public GameObject pathForward;
	public GameObject walkOverEdgeCollider;
	public AudioSource backgroundBirds;

	private Typer typer;
	private AudioSource aud;
	private int numHitReq = 3;
	private int numberOfSoundHits = 0;

	void Awake()
	{
		typer = GetComponent<Typer>();
		aud = GetComponent<AudioSource>();		
	}
	
	// Plays the turorial in the desired order.
	public IEnumerator StartTutorial()
	{
		// Intro
		yield return StartCoroutine(typer.TypeMessage(0));
		yield return StartCoroutine(WaitForButtonPress());

		// Spawn Gramophone
		MoveInGramophones();
		yield return StartCoroutine(typer.TypeMessage(1));
		yield return StartCoroutine(WaitForButtonPress());

		// Turn down lights
		yield return StartCoroutine(typer.TypeMessage(2));
		yield return StartCoroutine(lights.FadeOut());
		backgroundBirds.Stop();
		yield return new WaitForSeconds(1);
		aud.PlayOneShot(accept);
		yield return StartCoroutine(typer.TypeMessage(3));
		yield return StartCoroutine(WaitForButtonPress());

		// Make user Speak
		yield return StartCoroutine(typer.TypeMessage(4));
		yield return StartCoroutine(WaitForUserToMakeNoice());
		yield return StartCoroutine(typer.TypeMessage(5));
		yield return StartCoroutine(WaitForButtonPress());

		// Find goal
		yield return StartCoroutine(typer.TypeMessage(6));
		goal.SetActive(true);
		yield return StartCoroutine(WaitForButtonPress());

		// Open door
		yield return StartCoroutine(typer.TypeMessage(7));
		yield return StartCoroutine(WaitForButtonPress());
		yield return StartCoroutine(typer.TypeMessage(8));
		pathForward.SetActive(true);
		walkOverEdgeCollider.SetActive(false);
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

	IEnumerator WaitForUserToMakeNoice()
	{
		FireSoundWave.onBlastHit += MeasureNoiceMade;
		while(numberOfSoundHits < numHitReq)
		{
			yield return null;
		}
		aud.PlayOneShot(accept);
	}

	void MeasureNoiceMade(Vector3 hitPos, float pitchVal, float dbVal)
	{
		numberOfSoundHits++;
	}

	void MoveInGramophones()
	{
		gramophonePillar.SetActive(true);
		Animation anim = gramophonePillar.GetComponent<Animation>();
		anim["GramophonePillarMoveUp"].speed = 0.6f;
		anim.Play("GramophonePillarMoveUp");
	}
}	