﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	FirstPersonController player;
	GameObject goal;
	public GameObject goalPrefab;
	public Maze mazePrefab;
	private Maze mazeInstance;

	public int level = 1;

	private void Start () {
		goal = Instantiate (goalPrefab);
		player = FindObjectOfType<FirstPersonController> ();
		player.OnGoalTouch += WonGame;
		player.goal = goal;
		BeginGame();
	}

	private void Update () {
		if (Input.GetKeyDown(KeyCode.LeftControl)) {
			RestartGame();
		}
	}

	private void BeginGame () {
		mazeInstance = Instantiate (mazePrefab) as Maze;
		mazeInstance.Generate();

		Vector3 pos = mazeInstance.GetCell (new IntVector2 (0, 0)).transform.position;
		player.transform.position = new Vector3(pos.x, pos.y+.1f, pos.z);

		pos = mazeInstance.GetCell (new IntVector2 (mazeInstance.size.x - 1, mazeInstance.size.z - 1)).transform.position;
		goal.transform.position = new Vector3(pos.x, pos.y+0.5f, pos.z);
	}

	private void RestartGame () {
		Destroy (mazeInstance.gameObject);
		level = 1;
		BeginGame ();
	}

	private void WonGame() {
		Destroy (mazeInstance.gameObject);
		level++;
		BeginGame ();
	}
}
