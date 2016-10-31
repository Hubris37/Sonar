using UnityEngine;
using System.Collections;

public class ArrowFaceCamera : MonoBehaviour 
{
	private GameObject arrowTarget;

	void Start()
	{
		arrowTarget = GameObject.Find("ArrowTarget");
	}

	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(arrowTarget.transform);
		
		/*Vector3 currentRot = transform.localEulerAngles;
		currentRot.y = currentRot.z = 0;
		currentRot = -currentRot;
		transform.Rotate(currentRot,Space.World);*/
	}
}
