using UnityEngine;
// using System.Collections;
using System.Collections.Generic;

public class ShaderController : MonoBehaviour {

    public bool standardShader = false;
    public Renderer r;

    private const int MAX_CIRCLES = 500; // Maximum circles allowed at once
    private int numCircles = 0;
    private float prevTime, prevSoundCheck;
	private GameObject soundBlastList;
    private float colorCounter = 0;

    // Variable size lists used to be able to add and remove circles easily
    private List<float> radius = new List<float>();
	private List<float> expansionSpeeds = new List<float>();
    private List<Color> colors = new List<Color>();
    private List<Vector4> centers = new List<Vector4>();
    private List<float> maxRadius = new List<float>();
    private List<float> frequencies = new List<float>();

    // Fixed size arrays used because shader needs them fixed
    private float[] radiusArray = new float[MAX_CIRCLES];
    private Color[] colorsArray = new Color[MAX_CIRCLES];
    private Vector4[] centersArray = new Vector4[MAX_CIRCLES];
	private float[] maxRadiusArray = new float[MAX_CIRCLES];
    private float[] frequenciesArray = new float[MAX_CIRCLES];
	//private float[] expansionSpeedsArray = new float[MAX_CIRCLES];

    // Use this for initialization
    void Start () {

        r = GetComponent<Renderer>();
        if(standardShader) // Use standard shader to see everything
            r.sharedMaterial.shader = Shader.Find("Standard");
        else
            r.sharedMaterial.shader = Shader.Find("Custom/Echolocation");

        // Subscribe to SoundBlast onBlastHit function
        SoundBlast.onBlastHit += addCircle;
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    void addCircle(GameObject soundBlast)
    {
        SoundBlast soundData = soundBlast.GetComponent<SoundBlast>();

        // TODO: Tweak these for maxumum performance
        float maxRad = Mathf.Min((float)(soundData.DbVal), 5) + 1;
        Color col = Color.HSVToRGB(.7f, soundData.DbVal * 0.1f, soundData.PitchVal * 0.001f);
        float rad = 0; // Expand this
        Vector3 hitPoint = soundBlast.transform.position;
        float expSpeed = soundData.PitchVal*0.0001f;
        float freq = soundData.PitchVal;

        ++numCircles;
        maxRadius.Add(maxRad);
        radius.Add(rad);
        centers.Add(hitPoint);
        colors.Add(col);
        expansionSpeeds.Add(expSpeed);
        frequencies.Add(freq);

        Destroy(soundBlast);
    }
	
	// Update is called once per frame
	void Update () {
        if(standardShader) // Use standard shader to see everything
            r.sharedMaterial.shader = Shader.Find("Standard");
        else
            r.sharedMaterial.shader = Shader.Find("Custom/Echolocation");
        
        if (Time.time - prevTime > 0.005)
        {
            for(int i = 0; i < numCircles; ++i)
            {
				radius[i] += expansionSpeeds[i];

                if (radius[i] >= maxRadius[i])
                {
                    // Reset the circle's radius in the fixed size array
                    radiusArray[i] = 0;
                    // Remove circle
                    radius.RemoveAt(i);
                    colors.RemoveAt(i);
                    centers.RemoveAt(i);
                    maxRadius.RemoveAt(i);
					expansionSpeeds.RemoveAt(i);
                    frequencies.RemoveAt(i);

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
                    frequenciesArray[j] = frequencies[j];
                }

                r.sharedMaterials[i].SetVectorArray("_Center", centersArray);
                r.sharedMaterials[i].SetFloatArray("_Radius", radiusArray);
                r.sharedMaterials[i].SetFloatArray("_MaxRadius", maxRadiusArray);
                r.sharedMaterials[i].SetColorArray("_Color", colorsArray);
                r.sharedMaterials[i].SetFloatArray("_Frequency", frequenciesArray);
            }
        }
        //print("Number of circles: " + numCircles);
	}
}