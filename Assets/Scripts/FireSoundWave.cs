using UnityEngine;
using System.Collections;

public class FireSoundWave : MonoBehaviour {

	public GameObject soundBlast;
    public GameObject audioSrc;
    public float waveFreq = 10; // Number of waves per sec
    [RangeAttribute(-60,10)]
    public float volumeSens;

	private GameObject soundBlastList;
    private AudioMeasure audioMeasure;
    private Transform cameraT;
    private float prevTime, prevSoundCheck;

	// Use this for initialization
	void Start () 
	{
		cameraT = Camera.main.transform;
		audioSrc = GameObject.FindGameObjectWithTag("AudioSource");
		audioMeasure = audioSrc.GetComponent<AudioMeasure>();
		soundBlastList = GameObject.FindGameObjectWithTag("SoundBlastsList");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Time.time - prevSoundCheck > (1/waveFreq))
        {
            if(audioMeasure.DbValue > volumeSens)
            {
                FireSoundBlast();
            }
            prevSoundCheck = Time.time;
        }
	}

	void FireSoundBlast()
	{
		Vector3 fwd = cameraT.TransformDirection(Vector3.forward);
		// Spawm soundBlast and fire it away!
		GameObject o = (GameObject)Instantiate(soundBlast, cameraT.position 
						+ (fwd * 0.5f), Quaternion.LookRotation(fwd));
		SoundBlast oSound = o.GetComponent<SoundBlast>();
		oSound.PitchVal = audioMeasure.PitchValue;
		oSound.DbVal = audioMeasure.DbValue;
		oSound.FireDir = fwd;
		oSound.Fire();
		// Add soundBlast to the list in the Hierarchy
		o.transform.parent = soundBlastList.transform;
	}
}
