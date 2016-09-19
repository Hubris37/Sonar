﻿using UnityEngine;
// using System.Collections;
using System.Collections.Generic;

public class ShaderController : MonoBehaviour {

    public Renderer r;
    public AudioSource audioSrc;
    public float waveFreq = 10; // Number of waves per sec
    [RangeAttribute(-60,10)]
    public float volumeSens;

    private AudioMeasure audioMeasure;

    const int MAX_CIRCLES = 200;
    // Variable size lists used to be able to add and remove circles easily
    private List<float> radius = new List<float>();
	private List<float> expansionSpeed = new List<float>();
    private List<Color> colors = new List<Color>();
    private List<Vector4> centers = new List<Vector4>();
    private List<float> maxRadius = new List<float>();

    // Fixed size arrays used because shader needs them fixed
    private float[] radiusArray = new float[MAX_CIRCLES];
    private Color[] colorsArray = new Color[MAX_CIRCLES];
    private Vector4[] centersArray = new Vector4[MAX_CIRCLES];
	private float[] maxRadiusArray = new float[MAX_CIRCLES];
	//private float[] expansionSpeedArray = new float[MAX_CIRCLES];

	private int numCircles = 0;
    private Transform cameraT;
    private float prevTime, prevSoundCheck;

    // float colorCounter = 0;

    // Use this for initialization
    void Start () {
        cameraT = Camera.main.transform;

        audioSrc = FindObjectOfType<AudioSource>();
        audioMeasure = audioSrc.GetComponent<AudioMeasure>();

        r = GetComponent<Renderer>();
        //r.sharedMaterial.shader = Shader.Find("Custom/Echolocation");
    }
	
	// Update is called once per frame
	void Update () {
        //How often waves should be sent out
        if (Time.time - prevSoundCheck > (1/waveFreq))
        {
            if(numCircles < MAX_CIRCLES && audioMeasure.DbValue > volumeSens)
            {
                // Create a new circle
                RaycastHit hit;
                Vector3 fwd = cameraT.TransformDirection(Vector3.forward);
                if (Physics.Raycast(cameraT.position, fwd, out hit))
                {
                    ++numCircles;
                    maxRadius.Add(Mathf.Min((float)(audioMeasure.DbValue), 5) + 1);
                    centers.Add(hit.point);
					colors.Add(Color.HSVToRGB(.7f,  audioMeasure.DbValue*0.1f, audioMeasure.PitchValue*0.001f));
					//colors.Add(Color.HSVToRGB(audioMeasure.PitchValue*0.001f, .7f,  .8f ));
					expansionSpeed.Add (audioMeasure.PitchValue * 0.0001f);
                    radius.Add(0);

                    // colorCounter = (colorCounter+0.01f)%1; // Can be used to cycle through colors
                }
            }

            prevSoundCheck = Time.time;
        }

        if (Time.time - prevTime > 0.005)
        {
            for(int i = 0; i < numCircles; ++i)
            {
				radius[i] += expansionSpeed[i];

                if (radius[i] >= maxRadius[i])
                {
                    // Reset the circle's radius in the fixed size array
                    radiusArray[i] = 0;
                    // Remove circle
                    radius.RemoveAt(i);
                    colors.RemoveAt(i);
                    centers.RemoveAt(i);
                    maxRadius.RemoveAt(i);
					expansionSpeed.RemoveAt(i);

                    --numCircles;
                    --i;
                }
            }

            prevTime = Time.time;
        }

        // Set shader uniforms
        for(int i = 0; i < r.sharedMaterials.Length; ++i)
        {
            r.sharedMaterials[i].SetInt("_NumCircles", numCircles);
            if(numCircles > 0)
            {
                // Move properties of all the circles to the fixed size arrays
                for(int j = 0; j < numCircles; ++j)
                {
                    centersArray[j] = centers[j];
                    radiusArray[j] = radius[j];
                    maxRadiusArray[j] = maxRadius[j];
                    colorsArray[j] = colors[j];
                }

                r.sharedMaterials[i].SetVectorArray("_Center", centersArray);
                r.sharedMaterials[i].SetFloatArray("_Radius", radiusArray);
                r.sharedMaterials[i].SetFloatArray("_MaxRadius", maxRadiusArray);
                r.sharedMaterials[i].SetColorArray("_Color", colorsArray);
            }
        }
        
	}
}
