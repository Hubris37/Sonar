using UnityEngine;
using System;
using System.Collections;

public class TutorialSignController : MonoBehaviour 
{
	public delegate void SignFinished();
	public static event SignFinished signFinished;

	public Typer typer;
	public Animator anim;

	void Awake()
	{
		typer = GetComponent<Typer>();		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(gameObject.name == "TutorialSignIntro")
			IntroAdvancer();
	}

	void IntroAdvancer()
	{
		if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") ||
			Input.GetButtonDown("Fire3") || Input.GetButtonDown("Gamepad Y"))
		{
			if(typer.IsTyping)
				return;

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
