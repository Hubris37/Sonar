using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Decides when different UIs for the VR user should be shown.
public class VRUIController : MonoBehaviour 
{
	public GameObject gameOverUI;
	public Text roomsCleared;

	private GameManager gm;

	void Awake()
	{
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();		
	}

	// Use this for initialization
	void Start () 
	{
		GameManager.isDead += ShowGameOver;
		GameManager.isReborn += HideGameOver;
	}
	
	private void ShowGameOver()
	{
		gameOverUI.SetActive(true);
		string stringEnd = "";
		if(gm.tempRoomsCleared == 1) stringEnd = "Room";
		else stringEnd = "Rooms";
		roomsCleared.text = "You Cleared " + gm.tempRoomsCleared + " Rooms";
	}

	private void HideGameOver()
	{
		gameOverUI.SetActive(false);
	}
}
