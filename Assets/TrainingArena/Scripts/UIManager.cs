using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public IntroTutorial tutorialSign1;
	public SoundTutorial tutorialSign2;

	// Use this for initialization
	private IEnumerator Start() 
	{
		yield return StartCoroutine(tutorialSign1.StartTutorial());
		//yield return StartCoroutine(tutorialSign2.StartTutorial());
	}
}
