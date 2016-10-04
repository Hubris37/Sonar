using UnityEngine;
using System.Collections;

public class StaticSoundSource : MonoBehaviour {

    public float audioLevel;
    public float interval;
    private float intervalCounter;

    public bool directionalSound;

    public delegate void SoundBlastHit(Vector3 hitPos, float pitchVal, float dbVal);
    public static event SoundBlastHit onBlastHit;

    // Use this for initialization
    void Start () {
        intervalCounter = interval;
	}
	
	// Update is called once per frame
	void Update () {
        intervalCounter -= Time.deltaTime;
        if (intervalCounter <= 0) {
            intervalCounter = interval;
            onBlastHit(transform.position, audioLevel, .18f);
            if (directionalSound) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 20.0f)) {
                    onBlastHit(hit.transform.position, audioLevel, 0.18f);
                }
            }
        }
	}
}
