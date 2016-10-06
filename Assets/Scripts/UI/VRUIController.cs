using UnityEngine;
using System.Collections;

// Decides when different UIs for the VR user should be shown.
public class VRUIController : MonoBehaviour 
{

	public GameObject gameOverUI;

	// Use this for initialization
	void Start () 
	{
		GameManager.isDead += ShowGameOver;
		GameManager.isReborn += HideGameOver;
	}
	
	private void ShowGameOver()
	{
		gameOverUI.SetActive(true);
	}

	private void HideGameOver()
	{
		gameOverUI.SetActive(false);
	}
}
