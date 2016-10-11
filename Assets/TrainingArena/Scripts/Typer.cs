using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Typesound source https://www.freesound.org/people/pfranzen/sounds/266894/
public class Typer : MonoBehaviour 
{

	public string[] messages;
	public float typeDelay = 0.01f;
	public AudioClip typeSound;
	public GameObject continueIndic;

	private Text textComp;
	private AudioSource aud;
	private bool isTyping;
	private int currentMessage;

	void Awake()
	{
		textComp = GetComponent<Text>();
	}

	// Use this for initialization
	void Start () 
	{
		currentMessage = 0;
		aud = GetComponent<AudioSource>();
		continueIndic.SetActive(false);
		StartCoroutine(TypeIn());
	}
	
	public IEnumerator TypeIn()
	{
		isTyping = true;
		textComp.text = "";
		continueIndic.SetActive(false);

		for(int i = 0; i < messages[currentMessage].Length+1; i++)
		{
			textComp.text = messages[currentMessage].Substring(0,i);
			aud.PlayOneShot(typeSound);
			yield return new WaitForSeconds(typeDelay);
		}

		isTyping = false;
		currentMessage++;
		continueIndic.SetActive(true);
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

}
