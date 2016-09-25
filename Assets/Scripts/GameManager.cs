using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public FirstPersonController player;
	CarController car;
	GameObject goal;
	public GameObject carPrefab;
	public GameObject playerPrefab;
	public GameObject goalPrefab;
	public Maze mazePrefab;
	private Maze mazeInstance;
    public AudioMeasure audioMeasure;

    private List<GameObject> bots;
    public GameObject Chef;

    public int level = 1;

	private void Start () {
        bots = new List<GameObject>();
		goal = Instantiate (goalPrefab);
		Instantiate(playerPrefab);
		//Instantiate(carPrefab);
		player = FindObjectOfType<FirstPersonController> ();
        audioMeasure = GameObject.Find("Audio Source").GetComponent<AudioMeasure>();
        //car = FindObjectOfType<CarController> ();
        player.OnGoalTouch += WonGame;
		player.goal = goal;
		BeginGame();
    }

	private void Update () {
		if (Input.GetKeyDown("z")) {
			RestartGame();
		}
	}

	private void BeginGame () {
		GenerateMaze();

		Vector3 pos = mazeInstance.GetCell (new IntVector2 (0, 0)).transform.position;
		player.transform.position = new Vector3(pos.x, pos.y+.1f, pos.z);
		pos = mazeInstance.GetCell (new IntVector2 (1, 0)).transform.position;
		//car.transform.position = new Vector3(pos.x, pos.y+1f, pos.z);

		pos = mazeInstance.GetCell (new IntVector2 (mazeInstance.size.x - 1, mazeInstance.size.z - 1)).transform.position;
		goal.transform.position = new Vector3(pos.x, pos.y+0.5f, pos.z);
        spawnAI(Chef);
    }

	public void GenerateMaze() {
		/*if(mazeInstance != null){
			if(Application.isPlaying) {
				Destroy(mazeInstance.gameObject);
			} else {
				DestroyImmediate (mazeInstance.gameObject);
			}
		}*/
		mazeInstance = Instantiate (mazePrefab) as Maze;
		mazeInstance.Generate();

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
        IntVector2 initCell = new IntVector2();
        initCell.x = Random.Range(0, mazeInstance.size.x);
        initCell.z = Random.Range(0, mazeInstance.size.z);
        Vector3 initPos = mazeInstance.GetCell(initCell).transform.position;
        initPos.y += 1;
        GameObject bot = Instantiate(AIPrefab, player.transform.position, Quaternion.identity) as GameObject;
        bots.Add(bot);
        bot.GetComponent<EnemyAI>().initializeAI(mazeInstance);
    }
}
