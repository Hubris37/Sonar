using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShaderController : MonoBehaviour {

    public Renderer r;
    public AudioSource audioSrc;
    public float waveFreq = 10; // Number of waves per sec
	public GameObject soundBlastWave;
	public bool useSoundBLAST = true;

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

	private GameObject soundBlastList;

    // float colorCounter = 0;

    // Use this for initialization
    void Start () {
        cameraT = Camera.main.transform;

        audioSrc = FindObjectOfType<AudioSource>();
        audioMeasure = audioSrc.GetComponent<AudioMeasure>();

        r = GetComponent<Renderer>();
        r.sharedMaterial.shader = Shader.Find("Custom/Echolocation");

		soundBlastList = GameObject.FindGameObjectsWithTag ("SoundBlastsList")[0];
    }
	
	// Update is called once per frame
	void Update () {
        //How often waves should be sent out
        if (Time.time - prevSoundCheck > (1/waveFreq))
        {
            if(numCircles < MAX_CIRCLES && audioMeasure.DbValue > 0)
            {
                // Create a new circle
                RaycastHit hit;
                Vector3 fwd = cameraT.TransformDirection(Vector3.forward);
                if (Physics.Raycast(cameraT.position, fwd, out hit))
                {
                    ++numCircles;
                    maxRadius.Add(Mathf.Min((float)(audioMeasure.DbValue), 5));
                    centers.Add(hit.point);
					colors.Add(Color.HSVToRGB(.7f,  audioMeasure.DbValue*0.1f, audioMeasure.PitchValue*0.001f));
					//colors.Add(Color.HSVToRGB(audioMeasure.PitchValue*0.001f, .7f,  .8f ));
					expansionSpeed.Add (audioMeasure.PitchValue * 0.0001f);
                    radius.Add(0);

                    // colorCounter = (colorCounter+0.01f)%1; // Can be used to cycle through colors
					// Spawn sound blast wave
					if (useSoundBLAST) 
					{
						GameObject o = (GameObject)Instantiate(soundBlastWave,cameraT.position + (fwd * 0.5f), Quaternion.LookRotation(fwd));
						o.GetComponent<SoundBlast> ().Freq = audioMeasure.PitchValue;
						o.GetComponent<SoundBlast> ().DbVal = audioMeasure.DbValue;
						o.transform.parent = soundBlastList.transform;	
					}

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
        r.sharedMaterial.SetInt("_NumCircles", numCircles);
        if(numCircles > 0)
        {
            // Move properties of all the circles to the fixed size arrays
            for(int i = 0; i < numCircles; ++i)
            {
                centersArray[i] = centers[i];
                radiusArray[i] = radius[i];
                maxRadiusArray[i] = maxRadius[i];
                colorsArray[i] = colors[i];
            }

            r.sharedMaterial.SetVectorArray("_Center", centersArray);
            r.sharedMaterial.SetFloatArray("_Radius", radiusArray);
            r.sharedMaterial.SetFloatArray("_MaxRadius", maxRadiusArray);
            r.sharedMaterial.SetColorArray("_Color", colorsArray);
        }
	}
}
