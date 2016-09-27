using UnityEngine;
using System.Collections;

public class ArrowFaceCamera : MonoBehaviour 
{
	public Camera targetCamera;

	void Start()
	{
		targetCamera = GameObject.Find("OverviewCamera").GetComponent<Camera>();
	}

	// Update is called once per frame
	void Update () 
	{
		// get vector to cam
		Vector3 objToCam = targetCamera.transform.position - transform.position;
		// only use the up vector y
     	objToCam.x = objToCam.z = 0.0f;
		// look at camera only around the y axis
      	transform.LookAt( targetCamera.transform.position - objToCam); 
	}
}
