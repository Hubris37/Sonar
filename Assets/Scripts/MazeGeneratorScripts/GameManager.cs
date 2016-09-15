using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject Player;
	public Maze mazePrefab;
	private Maze mazeInstance;

	private void Start () {
		BeginGame();
	}

	private void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			RestartGame();
		}
	}

	private void BeginGame () {
		mazeInstance = Instantiate (mazePrefab) as Maze;
		mazeInstance.Generate();
		Vector3 pos = mazeInstance.GetCell (new IntVector2 (0, 0)).transform.position;
		Player.transform.position = new Vector3(pos.x, pos.y+.1f, pos.z);
	}

	private void RestartGame () {
		Destroy (mazeInstance.gameObject);
		BeginGame ();
	}
}
