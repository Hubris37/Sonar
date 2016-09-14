using UnityEngine;
using System.Collections;

public class ShaderController : MonoBehaviour {

    public static Shader s;
    public Renderer r;

    public AudioSource audioSrc;

    private AudioMeasure audioMeasure;

    const int MAX_CIRCLES = 10;
    private float size, clickSize = 0;
    private float[] radius = new float[MAX_CIRCLES];
    private Color[] colors = new Color[MAX_CIRCLES];
    private Vector4[] centers = new Vector4[MAX_CIRCLES];
	private float[] maxRadius = new float[MAX_CIRCLES];
	private float numCircles = 2;

    private Transform cameraT;

    private bool fired, clickFired = false;
    private bool grow, clickGrow = true;

    private float prevTime, clickTime;

    // Use this for initialization
    void Start () {
        cameraT = Camera.main.transform;
        audioSrc = FindObjectOfType<AudioSource>();
        audioMeasure = audioSrc.GetComponent<AudioMeasure>();
        r = GetComponent<Renderer>();
        r.sharedMaterial.shader = Shader.Find("Custom/Echolocation");

        for(int i = 0; i < 10; ++i)
        {
            colors[i] = new Color(0.3f, 0.6f, 0.7f);
        }
        colors[1] = new Color(0.8f, 0.3f, 0.3f);
		maxRadius [1] = 15;
        r.sharedMaterial.SetColorArray("_Color", colors);
    }
	
	// Update is called once per frame
	void Update () {
        if (audioMeasure.DbValue > 0 && !fired)
        {
            size = Mathf.Min(size + (float)(audioMeasure.DbValue*0.1), 10);
			maxRadius [0] = size;
            //Debug.Log(size);
        }
        else if (size > 0)
        {
            if(!fired)
            {
                RaycastHit hit;
                Vector3 fwd = cameraT.TransformDirection(Vector3.forward);
                if (Physics.Raycast(cameraT.position, fwd, out hit))
                {
                    centers[0] = hit.point;
                }

                fired = true;
                prevTime = Time.time;
                grow = true;
                radius[0] += 0.1f;
            }

            if(Time.time - prevTime > 0.005)
            {
                if (grow)
                {
                    radius[0] += .1f;
                    if (radius[0] >= size)
                        grow = false;
                    prevTime = Time.time;
                }
                else
                {
                    radius[0] = size = 0;
                    fired = false;
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && !clickFired)
        {
            clickSize = 10;
        }
        else if (clickSize > 0)
        {
            if(!clickFired)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    centers[1] = hit.point;
                }

                clickFired = true;
                clickTime = Time.time;
                clickGrow = true;
                radius[1] += 0.1f;
            }

            if (Time.time - clickTime > 0.005)
            {
                if (clickGrow)
                {
                    radius[1] += .3f;
                    if (radius[1] >= clickSize)
                        clickGrow = false;
                    clickTime = Time.time;
                }
                else
                {
                    radius[1] = clickSize = 0;
                    clickFired = false;
                }
            }
        }

        r.sharedMaterial.SetVectorArray("_Center", centers);
        r.sharedMaterial.SetFloatArray("_Radius", radius);
		r.sharedMaterial.SetFloatArray("_MaxRadius", maxRadius);
		r.sharedMaterial.SetFloat("_NumCircles", numCircles);
	}
}
