using UnityEngine;
using System.Collections;

public class ChefRotate : MonoBehaviour {

	public float rotSpeed = 10;
	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();		
	}

	// Use this for initialization
	void Start () 
	{
		anim.SetTrigger("seek");
	}
	
	// Update is called once per frame
	void Update () 
	{
		//transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed);
	}
}
