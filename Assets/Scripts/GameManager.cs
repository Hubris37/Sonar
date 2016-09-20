﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	FirstPersonController player;
	CarController car;
	GameObject goal;
	public GameObject carPrefab;
	public GameObject playerPrefab;
	public GameObject goalPrefab;
	public Maze mazePrefab;
	private Maze mazeInstance;

    private List<GameObject> bots;
    public GameObject Chef;

    public int level = 1;

	private void Start () {
        bots = new List<GameObject>();
		goal = Instantiate (goalPrefab);
		Instantiate(playerPrefab);
		Instantiate(carPrefab);
		player = FindObjectOfType<FirstPersonController> ();
		car = FindObjectOfType<CarController> ();
		player.OnGoalTouch += WonGame;
		player.goal = goal;
		BeginGame();
        spawnAI(Chef);
    }

	private void Update () {
		if (Input.GetKeyDown("z")) {
			RestartGame();
		}
	}

	private void BeginGame () {
		mazeInstance = Instantiate (mazePrefab) as Maze;
		mazeInstance.Generate();

		Vector3 pos = mazeInstance.GetCell (new IntVector2 (0, 0)).transform.position;
		player.transform.position = new Vector3(pos.x, pos.y+.1f, pos.z);
		pos = mazeInstance.GetCell (new IntVector2 (1, 0)).transform.position;
		car.transform.position = new Vector3(pos.x, pos.y+1f, pos.z);

		pos = mazeInstance.GetCell (new IntVector2 (mazeInstance.size.x - 1, mazeInstance.size.z - 1)).transform.position;
		goal.transform.position = new Vector3(pos.x, pos.y+0.5f, pos.z);
	}

	private void RestartGame () {
		Destroy (mazeInstance.gameObject);
        foreach (GameObject bot in bots) {
            Destroy(bot);
        }
        level = 1;
		BeginGame ();
	}

	private void WonGame() {
		Destroy (mazeInstance.gameObject);
		level++;
		BeginGame ();
	}

    private void spawnAI(GameObject AIPrefab) {
        // Vector3 initPos = new Vector3(Player.transform.position);
        GameObject bot = Instantiate(AIPrefab, player.transform.position, Quaternion.identity) as GameObject;
        bots.Add(bot);
        bot.GetComponent<EnemyAI>().initializeAI(mazeInstance);
    }
}
