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
    public int startChefAmount = 3;
    private int chefAmount;

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
        chefAmount = startChefAmount;
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
        for (int i = 0; i < chefAmount; ++i)
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
        destroyLevel();
        level = 1;
        chefAmount = startChefAmount;
		BeginGame ();
	}

    private void destroyLevel() {
        Destroy(mazeInstance.gameObject);
        foreach (GameObject bot in bots) {
            Destroy(bot);
        }
        bots.Clear();
    }

	private void WonGame() {
		destroyLevel();
		level++;
        chefAmount++;
		BeginGame ();
	}

    public void LostGame() {
        RestartGame();
    }

    private void spawnAI(GameObject AIPrefab) {
        IntVector2 initCell = new IntVector2();
        initCell.x = Random.Range(0, mazeInstance.size.x);
        initCell.z = Random.Range(0, mazeInstance.size.z);
        Vector3 initPos = mazeInstance.GetCell(initCell).transform.position;
        GameObject bot = Instantiate(AIPrefab, initPos, Quaternion.identity) as GameObject;
        bots.Add(bot);
        bot.GetComponent<EnemyAI>().initializeAI(mazeInstance, mazeInstance.GetCell(initCell));
    }
}
