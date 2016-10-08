using UnityEngine;

public class MazeDoor : MazePassage {

	public Transform hinge;

	void Start() {
		
	}
	

	public override void Initialize (MazeCell primary, MazeCell other, MazeDirection direction) {
		base.Initialize(primary, other, direction);

		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild(i);
			if (child != hinge) {
				child.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
			} else {
				child.GetChild(0).GetComponent<Renderer>().material = cell.room.settings.floorMaterial;
			}
		}
	}
	
	void OnTriggerEnter(Collider triggerCollider) {
		//print (triggerCollider.gameObject.name);

		if (triggerCollider.gameObject.name == "Player(Clone)") {

			cell.room.Show();
			otherCell.room.Show();
			//hinge.localPosition += new Vector3(0f,1f,0f);
			//move = true;
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
