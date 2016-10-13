using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

// Typesound source https://www.freesound.org/people/pfranzen/sounds/266894/
public class Typer : MonoBehaviour 
{
	public event Action<int> MessageDone; 

	public string[] messages;
	public string[] titles;
	public float typeDelay = 0.01f;
	public AudioClip typeSound;
	public Text titleText;
	public Text fillerText;
	public GameObject lookAroundImg;
	public GameObject moveAroundImg;
	public GameObject turnAroundImg;
	public GameObject anyButtonImg;

	private AudioSource aud;
	private bool isTyping;
	private int currentMessage;

	void Awake()
	{
		aud = GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start () 
	{
		currentMessage = 0;
		HideImages();
	}
	
	void OnEnable()
	{
		StartCoroutine(TypeMessage());
	}

	// Type out the message and show apropriate images
	public IEnumerator TypeMessage()
	{
		isTyping = true;
		fillerText.text = "";
		titleText.text = titles[currentMessage];
		HideImages();

		if(currentMessage == 1)
		{
			lookAroundImg.SetActive(true);
			lookAroundImg.GetComponent<Animator>().SetTrigger("FadeIn");
		} 
		if(currentMessage == 2) moveAroundImg.SetActive(true);
		if(currentMessage == 3) turnAroundImg.SetActive(true);

		for(int i = 0; i < messages[currentMessage].Length+1; i++)
		{
			fillerText.text = messages[currentMessage].Substring(0,i);
			yield return new WaitForSeconds(typeDelay);
		}

		if(currentMessage == 0) anyButtonImg.SetActive(true);
		//if(currentMessage == 1) anyButtonImg.SetActive(true);
		//if(currentMessage == 2) anyButtonImg.SetActive(true);

		isTyping = false;
		MessageDone(currentMessage);
		currentMessage++;
	}

	public void HideImages()
	{
		lookAroundImg.SetActive(false);
		moveAroundImg.SetActive(false);
		turnAroundImg.SetActive(false);
		anyButtonImg.SetActive(false);
	}

	public bool HasNextMessage
	{
		get{ if(currentMessage < messages.Length) return true;
			 else return false; }
	}
	public bool IsTyping
	{
		get{ return isTyping; }
	}
	public int CurrentMessage
	{
		get{ return currentMessage; }
	}
}
