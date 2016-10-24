using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	float masterVolumePercent = 1;

	public static AudioManager instance;

	void Awake() {
		if(instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
	
	public void PlaySound(AudioClip clip, Vector3 pos) {
		if(clip != null) {
			AudioSource.PlayClipAtPoint (clip, pos, masterVolumePercent);
		}
	}
}
