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

		if (triggerCollider.tag == "Player") {

			cell.room.Show();
			otherCell.room.Show();

		}
	}

	void OnTriggerExit(Collider triggerCollider) {

		if (triggerCollider.tag == "Player") {

		}
	}
}
