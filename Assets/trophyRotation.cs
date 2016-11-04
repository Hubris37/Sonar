using UnityEngine;
using System.Collections;

public class trophyRotation : MonoBehaviour 
{
	public float speed = 200000;
	
	// Update is called once per frame
	void Update () 
	{
		transform.RotateAround(transform.position, Vector3.up, speed * Time.deltaTime);
	}
}
