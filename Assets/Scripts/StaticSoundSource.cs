using UnityEngine;
using System.Collections;

public class StaticSoundSource : MonoBehaviour {

    public float audioPitch = 0f;
    public float audiodB = 0f;
    public float interval = 1.0f;
    private float intervalCounter;

    public bool directionalSound;
    private AudioSource soundSource;

    public delegate void SoundBlastHit(Vector3 hitPos, float pitchVal, float dbVal);
    public static event SoundBlastHit onBlastHit;

    // Use this for initialization
    void Start () {
        intervalCounter = interval;
        soundSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        intervalCounter -= Time.deltaTime;
        if (intervalCounter <= 0) {
            intervalCounter = interval;
            onBlastHit(transform.position, audioPitch, audiodB);
            if (directionalSound) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 20.0f)) {
                    onBlastHit(hit.transform.position, audioPitch, audiodB);
                }
            }
        }
	}

    public AudioSource getAudio() {
        return soundSource;
    }
}
