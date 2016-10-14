using UnityEngine;
using System;
using System.Collections;

public class SoundTutorial : MonoBehaviour 
{
	public delegate void SignFinished();
	public static event SignFinished signFinished;

	public AudioClip accept;

	public GameObject anyButtonImg;
	public GameObject gramophonePillar;
	public LightTrigger lights;

	private AudioSource aud;
	private Typer typer;
	private int numberOfSoundHits = 0;
	private int numHitReq = 5;

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
		gramophonePillar.SetActive(true);
		yield return StartCoroutine(typer.TypeMessage(1));
		yield return StartCoroutine(WaitForButtonPress());

		// Turn down lights
		yield return StartCoroutine(typer.TypeMessage(2));
		yield return StartCoroutine(lights.FadeOut());
		yield return new WaitForSeconds(1);
		yield return StartCoroutine(typer.TypeMessage(3));
		yield return StartCoroutine(WaitForButtonPress());

		// Make user Speak
		yield return StartCoroutine(typer.TypeMessage(4));
		yield return StartCoroutine(WaitForUserToMakeNoice());
		yield return StartCoroutine(typer.TypeMessage(5));

		// Find goal

		// Open doors

		// Avoid chefs

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
}	