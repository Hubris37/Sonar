using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Maybe implement delay to color change
public class ChangeWhenLookedAt : MonoBehaviour 
{
	public void ChangeColor()
	{
		GetComponent<Image>().color = Color.green;
	}
}
