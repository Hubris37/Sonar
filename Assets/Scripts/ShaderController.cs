using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShaderController : MonoBehaviour {

    public bool standardShader = false;
    public Renderer r;

    private const int MAX_CIRCLES = 120; // Maximum circles allowed at once !!!CHANGE IN SHADER AND OVERVIEWCAMERA AS WELL!!!
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
        FireSoundWave.onBlastHit += addCircle;
        EnemyAI.onBlastHit += addCircle;
        StaticSoundSource.onBlastHit += addCircle;
        WiiMoteController.onBlastHit += addCircle;
        GameManager.isReborn += ClearCircles;

        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    void addCircle(Vector3 hitPos, float pitchVal, float dbVal)
    {
        if(numCircles < MAX_CIRCLES)
        {
            // TODO: Tweak these for maxumum performance
            float maxRad = dbVal * 25f + 5f;
            float pitchPercent = Mathf.Clamp01(pitchVal * 0.001f);
            Color col = Color.HSVToRGB(pitchPercent, dbVal, pitchPercent + 0.2f);

            float rad = 0; // Expand this
            Vector3 hitPoint = hitPos;
            float expSpeed = pitchVal*0.07f + 68f; // Quick fix for too small/slow values
            float freq = pitchVal;

            ++numCircles;
            maxRadius.Add(maxRad);
            radius.Add(rad);
            centers.Add(hitPoint);
            colors.Add(col);
            expansionSpeeds.Add(expSpeed);
            frequencies.Add(freq);
        }
    }
	
    // Reset circles on game restart
    void ClearCircles()
    {
        maxRadius.Clear();
        radius.Clear();
        centers.Clear();
        colors.Clear();
        expansionSpeeds.Clear();
        frequencies.Clear();
        numCircles = 0;
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
                radius[i] += expansionSpeeds[i]*Time.deltaTime/(2*radius[i]+1f);

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
	}
}