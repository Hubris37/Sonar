using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public GameObject[] tutorialSigns;

	private int currentSign = 0;

	// Use this for initialization
	void Start () 
	{
		DeactivateSigns();
		TutorialSignController.signFinished += NextSign;
		NextSign();
	}

	private void NextSign()
	{
		if(currentSign >= tutorialSigns.Length) return;

		DeactivateSigns();
		tutorialSigns[currentSign].SetActive(true);
		currentSign++;
	}

	private void DeactivateSigns()
	{
		for(int i = 0; i < tutorialSigns.Length; i++)
		{
			tutorialSigns[i].SetActive(false);
		}
	}

}
