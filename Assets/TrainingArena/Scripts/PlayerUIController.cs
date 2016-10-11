using UnityEngine;
using System.Collections;

public class PlayerUIController : MonoBehaviour {

	public GameObject niceTransitionUI;

	void Awake()
	{
		niceTransitionUI.SetActive(true);
	}
}
