using UnityEngine;
using System.Collections;

/// Currently not in use ///
public class SoundBlast : MonoBehaviour {

	public float startScale = 0.05f;
	public float speed = 50;
	public float deathTime = 0.5f;

	private Rigidbody rb;
	private float lifeTime = 0;
	private float pitchVal;
	private float dbVal;
	private Vector3 fireDir;
	private bool readyFire = false;

	public delegate void SoundBlastHit(GameObject blast);
	public static event SoundBlastHit onBlastHit;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		transform.localScale = Vector3.one * startScale;
	}

	public void Fire(){ readyFire = true; }

	// Allow scripts to subscribe to this on collision event.
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "MazePiece")
		{
			// Check if someone is subscribing to the event
			if(onBlastHit != null) 
				onBlastHit(this.gameObject);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		// TODO: The size of the sound blast aint doin any thing
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
			rb.AddForce(Camera.main.transform.TransformDirection(Vector3.forward) * speed, ForceMode.VelocityChange);
			readyFire = false;
		}		
	}

	/// Getters and Setters ///
	public float PitchVal
	{
		get{ return pitchVal;}
		set{ 
			pitchVal = value;
			speed = pitchVal *.08f; 
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
