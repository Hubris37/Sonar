using UnityEngine;
using System.Collections;

public class FireSoundWave : MonoBehaviour {

	public GameObject soundBlast;
    public GameObject audioSrc;
    public float waveFreq = 10; // Number of waves per sec
    [RangeAttribute(-60,10)]
    public float volumeSens;
	public float soundSpeed = 340f;

	private GameObject soundBlastList;
    private AudioMeasure audioMeasure;
    private Transform cameraT;
    private float prevTime, prevSoundCheck;

	public delegate void SoundBlastHit(Vector3 hitPos, float pitchVal, float dbVal);
	public static event SoundBlastHit onBlastHit;

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
		//Raycast
		Vector3 fwd = cameraT.TransformDirection(Vector3.forward);
		RaycastHit hit;
		if(Physics.Raycast(transform.position,fwd, out hit, 1000))
		{
			//onBlastHit(hit.point, audioMeasure.PitchValue, audioMeasure.DbValue);
			StartCoroutine(hitDelay(hit.point, audioMeasure.PitchValue, audioMeasure.DbValue));
			
		}
	}

	/** Realistic sound delay! */
	IEnumerator hitDelay(Vector3 hitPos, float pitchVal, float dbVal)
	{
		// s = v * t
		float dist = (hitPos - transform.position).magnitude;
		float delay = dist / soundSpeed;

		yield return new WaitForSeconds(delay);

		onBlastHit(hitPos, pitchVal, dbVal);

		yield return null;
	}
}
