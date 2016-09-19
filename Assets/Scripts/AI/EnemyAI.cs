using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour {

    public GameObject gameManager;
    private Maze maze;

    private bool moving = false;
    private List<MazeCell> movementPath;
    private MazeCell currentCell;

    public bool patrolsRoom;

	// Use this for initialization
	void Start () {
        maze = gameManager.GetComponent<Maze>();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
            currentCell = hit.transform.gameObject.GetComponent<MazeCell>();
        } else {
            Debug.LogError("Error: Could not find MazeCell for AI.", transform);
        }

	}
	
	// Update is called once per frame
	void Update () {
        findPath();
        move();
        tryGrabPlayer();
	}

    private void findPath() {
        if (movementPath.Count == 0) {
            if (patrolsRoom) {
                // Get a random cell in the current room
                MazeCell targetCell = currentCell.room.getCells()[Random.Range(0, currentCell.room.getCells().Count)];

            }
        }
    }

    private List<MazeCell> bfsSearch(MazeCell targetCell) {
        List<MazeCell> queue = new List<MazeCell>();
        queue.Add(currentCell);
        return queue;
    }

    private void move() {

    }

    private void tryGrabPlayer() {

    }
}
