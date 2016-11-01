using UnityEngine;
using System.Collections;

public class MazeGatePitchRedo : MazeDoor {

	[Range(0,2)]
	public float easeAmount;

	float pitchPoint;
	float pitchCurVal;
	float pitchLastVal;
	float pitchMultiplier;
	float maxPitch = 1000;
	float pitchThreshold = 5;
	int minPitch = 300;

	public Transform key;
	public Transform keyGoal;

	public float unlockTime;
	bool unlocked;

	public AudioClip unlockSound;
	AudioMeasure audioMeasure;
	GameObject player;

	void Start () {
		audioMeasure = GameObject.Find("AudioMeasure source").GetComponent<AudioMeasure>();
		player = GameObject.FindGameObjectWithTag ("Player");
		pitchMultiplier = maxPitch / 360;
		pitchPoint = Random.Range (minPitch, maxPitch);
	}

	float Ease(float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
	}

	IEnumerator Unlock() {
		AudioManager.instance.PlaySound(unlockSound,transform.position);
		yield return new WaitForSeconds (0.75f);

		float percent = 0;
		float unlockSpeed = 1 / unlockTime;
		float initScale = key.localScale.z;
		float val = initScale;

		while (percent < 1) {
			percent += Time.deltaTime * unlockSpeed;
			val = Mathf.Lerp (initScale, 0, percent);
			key.localScale = new Vector3 (key.localScale.x, key.localScale.y, val);
			keyGoal.localScale = new Vector3 (keyGoal.localScale.x, keyGoal.localScale.y, val);
			yield return null;
		}

		yield return new WaitForSeconds (0.5f);

		percent = 0;
		Vector3 initialPos = hinge.transform.position;
		Vector3 targetPos = initialPos + Vector3.down * 2f;

		while (percent < 1) {
			percent += Time.deltaTime * unlockSpeed;
			float easedPercent = Ease (percent);
			hinge.transform.position = Vector3.Lerp (initialPos, targetPos, easedPercent);
			yield return null;
		}

		Destroy (hinge.gameObject);
	}

	void Update() {
		if(!unlocked) {
			if (audioMeasure.PitchValue > 1) {
				float dist = Vector3.Distance (player.transform.position, transform.position);
				if (dist < 3) {
					pitchCurVal = (audioMeasure.PitchValue + pitchLastVal) * 0.5f;
					pitchLastVal = pitchCurVal;
					if (pitchCurVal < pitchPoint + pitchThreshold && pitchCurVal > pitchPoint - pitchThreshold) {
						StartCoroutine (Unlock ());
						unlocked = true;
					}
				}
			}
			key.localEulerAngles = Vector3.forward * (pitchPoint - pitchCurVal) / pitchMultiplier;
		}
	}
}
