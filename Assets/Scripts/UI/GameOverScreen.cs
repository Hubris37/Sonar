using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour 
{
	private Animator anim;
	private CanvasGroup cG;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		cG = GetComponent<CanvasGroup>();

		cG.alpha = 0; //????
		GameManager.isDead += ShowGameOver; 
		GameManager.isReborn += HideGameOver;	
	}
	
	private void ShowGameOver()
	{
		anim.SetBool("PlayerDead", true);
	}

	private void HideGameOver()
	{
		anim.SetBool("PlayerDead", false);
	}
}
