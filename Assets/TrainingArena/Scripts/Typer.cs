using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

// Types out messages, one letter at a time.
public class Typer : MonoBehaviour 
{
	public string[] messages;
	public string[] titles;
	public float typeDelay = 0.01f;
	public AudioClip typeSound;
	public Text titleText;
	public Text fillerText;

	private AudioSource aud;
	private bool isTyping;
	private int messageNum = 0;

	void Awake()
	{
		aud = GetComponent<AudioSource>();
	}

	// Type out the message and show apropriate images
	public IEnumerator TypeMessage(int messageNum)
	{
		isTyping = true;
		fillerText.text = "";
		titleText.text = titles[messageNum];

		for(int i = 0; i < messages[messageNum].Length; i++)
		{
			fillerText.text = messages[messageNum].Substring(0,i);
			yield return new WaitForSeconds(typeDelay);
		}
	}

	public bool HasNextMessage
	{
		get{ if(messageNum < messages.Length) return true;
			 else return false; }
	}
	public bool IsTyping
	{
		get{ return isTyping; }
	}
}
