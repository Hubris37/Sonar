using UnityEngine;
using System.Collections;

public class MazeGatePitchRedo : MazeDoor {

	[Range(0,2)]
	public float easeAmount;

	float pitchPoint;
	float pitchCurVal;
	float pitchLastVal;
	float pitchMultiplier;
	float maxPitch = 700;
	float pitchThreshold = 10;
	int minPitch = 300;

	public Transform key;
	public Transform keyGoal;

	public float unlockTime;
	bool unlocked;

	public AudioClip unlockSound;
	public AudioClip keyIn;
	public AudioClip doorOpen;

	AudioMeasure audioMeasure;
	GameObject player;

	void Start () {
		audioMeasure = GameObject.Find("AudioMeasure source").GetComponent<AudioMeasure>();
		player = GameObject.FindGameObjectWithTag ("Player");
		pitchMultiplier = 90 / maxPitch;
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

		AudioManager.instance.PlaySound(keyIn,transform.position);
		yield return new WaitForSeconds (0.5f);
		AudioManager.instance.PlaySound(doorOpen,transform.position);

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
				//print (audioMeasure.PitchValue);
				float dist = Vector3.Distance (player.transform.position, transform.position);
				if (dist < 3) {
					pitchCurVal = (audioMeasure.PitchValue + pitchLastVal) * 0.5f;
					pitchLastVal = pitchCurVal;
					if (pitchCurVal < pitchPoint + pitchThreshold && pitchCurVal > pitchPoint - pitchThreshold) {
						pitchPoint = pitchCurVal;
						StartCoroutine (Unlock ());
						unlocked = true;
					}
				}
			}
			float deg = (pitchPoint - pitchCurVal) * pitchMultiplier;
			key.localEulerAngles = Vector3.forward * deg; //(pitchCurVal - pitchPoint) % 360;// * pitchMultiplier;
			//print (deg);
		}

	}
}
