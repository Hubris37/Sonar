using UnityEngine;
using System.Collections;

public class SoundBlast : MonoBehaviour {

	public float startScale = 0.05f;
	public float speed = 10;
	public float deathTime = 0.5f;

	private Rigidbody rb;
	private float lifeTime = 0;
	private float freq;
	private float dbVal;
	private Vector3 fireDir;
	private bool readyFire = false;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		transform.localScale = Vector3.one * startScale;
	}

	public void Fire(){ readyFire = true; }

	// When we collide with a wall send wave info to shaderController
	// Then destroy the wave
	void OnTriggerEnter(Collider other)
	{
		//print("Other is: " + other.name);
		if(other.tag == "MazePiece")
		{
			//print("MazeCollision");
			Destroy(this.gameObject);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		transform.localScale += new Vector3(1f,1f,1f) * Time.deltaTime * 0.5f;
		lifeTime += Time.deltaTime;
		if (lifeTime >= deathTime)
			Destroy (this.gameObject);
	}

	// Do force stuff in FixedUpdate
	void FixedUpdate()
	{
		if(readyFire)
		{
			rb.AddForce(Camera.main.transform.TransformDirection(Vector3.forward) * speed, ForceMode.Impulse);
			readyFire = false;
		}		
	}

	/// Getters and Setters ///
	public float Freq
	{
		get{ return freq;}
		set{ 
			freq = value;
			speed = freq *.01f; 
		}
	}

	public float DbVal
	{
		get{ return dbVal;}
		set{ dbVal = value; }
	}

	public Vector3 FireDir
	{
		get { return fireDir; }
		set { fireDir = value; }
	}
}
