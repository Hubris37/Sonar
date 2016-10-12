using UnityEngine;
using System.Collections;

public class RotatingCube : MonoBehaviour {

	float speed;
	Vector3 randomAxis;

	// Use this for initialization
	void Start () {
		speed = Random.Range(5f,15f);
		randomAxis = new Vector3(Random.value, Random.value, Random.value);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(randomAxis, speed * Time.deltaTime);
	}
}
