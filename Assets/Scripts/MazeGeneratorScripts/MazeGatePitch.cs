using UnityEngine;
using System.Collections;

public class MazeGatePitch : MazeDoor {

	[Range(0,2)]
	public float easeAmount;
	private Vector3 startPos;
	private Vector3 endPos;
	private float startRot = 0f;
	private float endRot = 90f;
	float percentBetweenPoints;
	float speed = 0.5f;
	float pitchPoint;
	float pitchThreshold = 80;
	float pitchMultiplier = 0.001f;
	float percentTowardsGoal;
	int lowestPoint = 300;
	bool move;

	public Transform key;
	public Transform keyGoal;
	Vector3 keyStandardPos;
	Vector3 keyTargetPos;

	AudioSource Unlock;

	// Use this for initialization
	void Start () {
		FireSoundWave.onBlastHit += RecieveForce;
		pitchPoint = Random.Range(lowestPoint,900);
		keyGoal.localPosition += new Vector3(0f,pitchPoint*pitchMultiplier,0f);
		keyStandardPos = key.localPosition;
		keyTargetPos = keyStandardPos;
		Unlock = GetComponent<AudioSource>();
		/*startPos = hinge.localPosition;
		endPos = startPos + new Vector3(0f,1f,0f);
		startRot = hinge.localRotation;
		endRot = endRot + new Quaternion.Euler(0f, -90f, 0f);
		*/
	
	}

	float Ease(float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
	}
	
	void Update() {
		if(move) {
			if(percentBetweenPoints >= 1){
				percentBetweenPoints = 1;
				move = false;
			} else {
				percentBetweenPoints += Time.deltaTime * speed;
				percentBetweenPoints = Mathf.Clamp01(percentBetweenPoints);
				float easedPercent = Ease (percentBetweenPoints);
				//hinge.localPosition = Vector3.Lerp(startPos, endPos, easedPercent);
				float angle = Mathf.LerpAngle(startRot, endRot, easedPercent);
				hinge.localEulerAngles = new Vector3(0,angle,0);
			}
		}
		key.localPosition = Vector3.MoveTowards(key.localPosition,keyTargetPos,Time.deltaTime);
	}

	void OnTriggerEnter(Collider triggerCollider) {

		if (triggerCollider.gameObject.name == "Player(Clone)") {
			cell.room.Show();
			otherCell.room.Show();
		}
	}

	void RecieveForce(Vector3 hitPos, float pitchVal, float dbVal) {
		Vector3 myPos = transform.position;
		Vector3 heading = (myPos - hitPos);
		float dist = heading.sqrMagnitude;
		Vector3 dir = heading / dist;
		if(percentTowardsGoal != 1){
			//if(!move){
			if(dist < 20) {
				//Debug.Log(pitchVal);
				if(pitchVal < pitchPoint+pitchThreshold && pitchVal > pitchPoint-pitchThreshold) {
					percentTowardsGoal += 0.25f;
				}
				//Vector3 insert = key.transform.TransformPoint(new Vector3(percentTowardsGoal,0,0));
				keyTargetPos = keyStandardPos + new Vector3(percentTowardsGoal*0.4f,pitchVal*pitchMultiplier,0f);
			}
			if(percentTowardsGoal >= 1) {
				percentTowardsGoal = 1;
				move = true;
				Unlock.Play();
				keyTargetPos = keyGoal.localPosition;
			}
			//}
		}
	}

	void OnDestroy() {
		FireSoundWave.onBlastHit -= RecieveForce;
	}
}
