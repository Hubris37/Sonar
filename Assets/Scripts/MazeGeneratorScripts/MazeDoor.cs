using UnityEngine;

public class MazeDoor : MazePassage {

	public Transform hinge;
	[Range(0,2)]
	public float easeAmount;
	private Vector3 startPos;
	private Vector3 endPos;
	float percentBetweenPoints;
	float speed = 1f;
	bool move;

	void Start() {
		startPos = hinge.localPosition;
		endPos = startPos + new Vector3(0f,1f,0f);
	}

	void Update() {
		if(move) {
			if(percentBetweenPoints >= 1){
				percentBetweenPoints = 1;
				/*percentBetweenPoints = 0;
				Vector3 temp = startPos;
				startPos = endPos;
				endPos = temp;
				*/
				move = false;
			} else {
				percentBetweenPoints += Time.deltaTime * speed;
				percentBetweenPoints = Mathf.Clamp01(percentBetweenPoints);
				float easedPercent = Ease (percentBetweenPoints);
				hinge.localPosition = Vector3.Lerp(startPos, endPos, easedPercent);
			}
		}
	}

	float Ease(float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
	}

	void OnTriggerEnter(Collider triggerCollider) {
		//print (triggerCollider.gameObject.name);

		if (triggerCollider.gameObject.name == "Player(Clone)") {
			//hinge.localPosition += new Vector3(0f,1f,0f);
			move = true;
			//hinge.localRotation = hinge.localRotation = Quaternion.Euler(0f, -90f, 0f);
		}
	}

	void OnTriggerExit(Collider triggerCollider) {
		//print (triggerCollider.gameObject.name);

		if (triggerCollider.gameObject.name == "Player(Clone)") {

			//move = true;
			//hinge.localPosition -= new Vector3(0f,1f,0f);
			//hinge.localRotation = hinge.localRotation = Quaternion.Euler(0f, -90f, 0f);
		}
	}
}
