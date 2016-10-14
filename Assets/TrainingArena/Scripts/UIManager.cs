using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public IntroTutorial tutorialSign1;
	public bool playTutorial1 = true;
	public SoundTutorial tutorialSign2;
	public bool playTutorial2 = true;

	// Use this for initialization
	private IEnumerator Start() 
	{
		if(playTutorial1)
			yield return StartCoroutine(tutorialSign1.StartTutorial());
		if(playTutorial2)
			yield return StartCoroutine(tutorialSign2.StartTutorial());
		
		yield return null;
	}
}