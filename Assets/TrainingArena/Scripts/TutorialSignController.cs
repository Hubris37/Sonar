using UnityEngine;
using System.Collections;

public class TutorialSignController : MonoBehaviour {

	public Typer typer;
	public Animator anim;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") ||
			Input.GetButtonDown("Fire3") || Input.GetButtonDown("Gamepad Y"))
		{
			if(typer.IsTyping)
				return;

			if(typer.HasNextMessage)
				StartCoroutine(typer.TypeIn());

			else
				anim.SetTrigger("FadeOut");
		}	
	}
}
