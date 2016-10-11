using UnityEngine;
using System.Collections;

public class StaticSoundSource : MonoBehaviour {

    public float audioPitch = 0f;
    public float audiodB = 0f;
    public float soundRange = 8.0f;
    public float interval = 1.0f;
    private float intervalCounter;
    private float cPitch;
    private float cdB;

    public bool directionalSound;
    private AudioSource soundSource;

    public delegate void SoundBlastHit(Vector3 hitPos, float pitchVal, float dbVal);
    public static event SoundBlastHit onBlastHit;

    // Use this for initialization
    void Start () {
        cPitch = audioPitch;
        cdB = audiodB;
        intervalCounter = interval;
        soundSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        intervalCounter -= Time.deltaTime;
        if (intervalCounter <= 0) {
            intervalCounter = interval;
            onBlastHit(transform.position, cPitch, cdB);
            if (directionalSound) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, soundRange)) {
                    onBlastHit(hit.transform.position, cPitch, cdB);
                }
            }
        }
	}

    public void multAudioParameters(float pitch, float dB) {
        cPitch = audioPitch * pitch;
        cdB = audiodB * dB;
    }

    public AudioSource getAudio() {
        return soundSource;
    }
}
