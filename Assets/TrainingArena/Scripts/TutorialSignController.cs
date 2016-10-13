using UnityEngine;
using System;
using System.Collections;

public class TutorialSignController : MonoBehaviour 
{
	public delegate void SignFinished();
	public static event SignFinished signFinished;

	public Typer typer;
	public Animator anim;
	public AudioClip accept;

	private AudioSource aud;

	void Awake()
	{
		typer = GetComponent<Typer>();
		aud = GetComponent<AudioSource>();		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(gameObject.name == "TutorialSignIntro")
			IntroAdvancer();
	}

	void IntroAdvancer()
	{
		if(typer.IsTyping)
			return;

		if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") ||
			Input.GetButtonDown("Fire3") || Input.GetButtonDown("Gamepad Y"))
		{
			aud.PlayOneShot(accept);

			if(typer.HasNextMessage)
				StartCoroutine(typer.TypeIn());

			else
			{
				anim.SetTrigger("FadeOut");
				signFinished();
			}
		}
	}
}
