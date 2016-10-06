using UnityEngine;
using System;
using System.Collections;

public class TutorialGoal : MonoBehaviour {

	public event Action OnGoalEnter;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
			OnGoalEnter();		
	}
}
