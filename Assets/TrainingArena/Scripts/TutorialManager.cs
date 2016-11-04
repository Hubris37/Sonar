using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

	public IntroTutorial tutorialSign1;
	public bool playTutorial1 = true;
	public SoundTutorial2 tutorialSign2;
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

	void Update()
	{
		if(Input.GetKeyDown("j"))
		{
			if (NiceSceneTransition.instance != null) {
            	NiceSceneTransition.instance.LoadScene("MainGame");
        	}
        	else {
            	SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
        	}
		}		
	}
}