using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundTutorial2 : MonoBehaviour 
{
	public GameObject goal;
	public AudioClip accept;
	public LightController lightController;
	public GameObject anyButtonImg;
	public GameObject gramophonePillar;
	public AudioSource backgroundBirds;
	public GameObject controllsImg;
	public GameObject chefDisp;

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
		MoveInGramophones();
		lightController.TurnOff();
		backgroundBirds.Stop();
		gramophonePillar.GetComponent<ChangeMaterial>().SwapMaterial();

		// Intro
		yield return StartCoroutine(typer.TypeMessage(0));
		yield return StartCoroutine(WaitForButtonPress());

		// Movement Explained
		yield return StartCoroutine(typer.TypeMessage(1));
		controllsImg.SetActive(true);
		yield return StartCoroutine(WaitForButtonPress());
		controllsImg.SetActive(false);

		// Use voice to see and open doors
		yield return StartCoroutine(typer.TypeMessage(2));
		yield return StartCoroutine(WaitForButtonPress());

		// Find Goal
		yield return StartCoroutine(typer.TypeMessage(3));
		goal.SetActive(true);
		yield return StartCoroutine(WaitForButtonPress());
		goal.SetActive(false);

		// Avoid Enemies
		yield return StartCoroutine(typer.TypeMessage(4));
		chefDisp.SetActive(true);
		yield return StartCoroutine(WaitForButtonPress());
		chefDisp.SetActive(false);

		// Press to start game
		yield return StartCoroutine(typer.TypeMessage(5));
		yield return StartCoroutine(WaitForButtonPress());
		EnterRealGame();
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

	void MoveInGramophones()
	{
		gramophonePillar.SetActive(true);
		Animation anim = gramophonePillar.GetComponent<Animation>();
		anim["GramophonePillarMoveUp"].speed = 0.6f;
		anim.Play("GramophonePillarMoveUp");
	}

	void EnterRealGame()
	{
		if (NiceSceneTransition.instance != null) {
            NiceSceneTransition.instance.LoadScene("MainGame");
        }
        else {
            SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
        }
	}
}	