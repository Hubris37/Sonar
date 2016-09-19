using UnityEngine;
using System.Collections;

public class SoundBlast : MonoBehaviour {

	public float startScale = 0.05f;
	public float speed = 10;
	public float deathTime = 0.5f;

	private float lifeTime = 0;
	private Rigidbody rb;
	private float freq;
	private float dbVal;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();

		transform.localScale = Vector3.one * startScale;
		speed = freq *.01f;
		rb.AddForce(Camera.main.transform.TransformDirection(Vector3.forward) * speed, ForceMode.Impulse);
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.localScale += new Vector3(1f,1f,1f) * Time.deltaTime * 0.5f;
		lifeTime += Time.deltaTime;
		if (lifeTime >= deathTime)
			Destroy (this.gameObject);
	}

	public float Freq
	{
		get{ return freq;}
		set{ freq = value; }
	}

	public float DbVal
	{
		get{ return dbVal;}
		set{ dbVal = value; }
	}
}
