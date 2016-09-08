using UnityEngine;
using System.Collections;

public class ShaderController : MonoBehaviour {

    public static Shader s;
    public Renderer r;

    public GameObject AudioSource;
    private AudioMeasure audioMeasure;

	// Use this for initialization
	void Start () {
        r = GetComponent<Renderer>();
        r.material.shader = Shader.Find("Custom/Echolocation");
        audioMeasure = AudioSource.GetComponent<AudioMeasure>();
    }
	
	// Update is called once per frame
	void Update () {
        float val = 0.05f * (160 + audioMeasure.DbValue);
        r.material.SetFloat("_Radius", val);
	}
}
