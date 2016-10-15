using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialGoal : MonoBehaviour 
{
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
			EnterRealGame();		
	}

	void EnterRealGame()
	{
		if (NiceSceneTransition.instance != null) {
            NiceSceneTransition.instance.LoadScene("movementTest");
        }
        else {
            SceneManager.LoadScene("movementTest", LoadSceneMode.Single);
        }
	}
}
