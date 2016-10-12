using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Typesound source https://www.freesound.org/people/pfranzen/sounds/266894/
public class Typer : MonoBehaviour 
{

	public string[] messages;
	public string[] titles;
	public float typeDelay = 0.01f;
	public AudioClip typeSound;
	public Text titleText;
	public Text fillerText;
	public GameObject continueIndic;

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
		continueIndic.SetActive(false);
	}
	
	void OnEnable()
	{
		StartCoroutine(TypeIn());
	}

	public IEnumerator TypeIn()
	{
		isTyping = true;
		fillerText.text = "";
		continueIndic.SetActive(false);
		titleText.text = titles[currentMessage];

		for(int i = 0; i < messages[currentMessage].Length+1; i++)
		{
			fillerText.text = messages[currentMessage].Substring(0,i);
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
