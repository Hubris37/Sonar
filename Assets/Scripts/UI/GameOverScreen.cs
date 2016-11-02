using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour 
{
	public GameObject gameOver;
	private Animator anim;
	private CanvasGroup cG;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		cG = GetComponent<CanvasGroup>();

		GameManager.isDead += ShowGameOver; 
		GameManager.isReborn += HideGameOver;	
		gameOver.SetActive(false);
	}
	
	private void ShowGameOver()
	{
		//anim.SetBool("PlayerDead", true);
		gameOver.SetActive(true);
		Debug.Log("DeAD");
	}

	private void HideGameOver()
	{
		//anim.SetBool("PlayerDead", false);
		gameOver.SetActive(false);
	}
}
