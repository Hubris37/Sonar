using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour 
{
	public TutorialGoal tutGoal;
	public string nextLvlName;
	[Header("UI")]
	public UIManager UIManager;

	void Start()
	{
		tutGoal.OnGoalEnter += ChangeScene;
	}

	private void ChangeScene()
	{
		if(NiceSceneTransition.instance != null)
		{
			NiceSceneTransition.instance.LoadScene(nextLvlName);
		} else {
			SceneManager.LoadScene(nextLvlName);
		}
	}
}
