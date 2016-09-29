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
	private bool playerIsDead = false;

    public int level = 1;

	public delegate void PlayerState();
	public static event PlayerState isDead;
	public static event PlayerState isReborn;

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
		if(playerIsDead)
			if(Input.GetKeyDown("z") || Input.GetButtonDown("Fire1"))
				RestartGame();
	}

	private void BeginGame () {
		GenerateMaze();

		Vector3 pos = mazeInstance.GetCell (new IntVector2 (0, 0)).transform.position;
		player.transform.position = new Vector3(pos.x, pos.y+1.25f, pos.z);
		MazeCell startingCell = mazeInstance.GetCell (new IntVector2 (1, 0));
		pos = startingCell.transform.position;
		startingCell.room.Show();
		//car.transform.position = new Vector3(pos.x, pos.y+1f, pos.z);

		pos = mazeInstance.GetCell (new IntVector2 (mazeInstance.size.x - 1, mazeInstance.size.z - 1)).transform.position;
		goal.transform.position = new Vector3(pos.x, pos.y+0.5f, pos.z);
        spawnAI(Chef, chefAmount, startingCell);
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
		isReborn();
		player.freezeMovement = false;
		playerIsDead = false;
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
        isDead();
		playerIsDead = true;
		player.freezeMovement = true;
		//RestartGame();
    }

    private void spawnAI(GameObject AIPrefab, int count, MazeCell startingCell) {
        List<MazeRoom> roomsLeft = mazeInstance.getRooms();
        // Create as many AIs as specified
        for (int i = 0; i < count; ++i) {
            // If no free rooms left, set all to available
            if (roomsLeft.Count == 0)
                roomsLeft = mazeInstance.getRooms();
            // Select a random room
			MazeRoom room = roomsLeft[Random.Range(0, roomsLeft.Count)];
			while(room == startingCell.room){
				room = roomsLeft[Random.Range(0, roomsLeft.Count)];
			}

			// Select a random cell in the room to initialize at
			MazeCell initCell = room.getCells()[Random.Range(0, room.getCells().Count)];
			// Remove current room from unoccupied ones
			roomsLeft.Remove(room); Vector3 initPos = initCell.transform.position;
			GameObject bot = Instantiate(AIPrefab, initPos, Quaternion.identity) as GameObject;
			bots.Add(bot);
			bot.GetComponent<EnemyAI>().initializeAI(mazeInstance, initCell);
			
        }
    }
}
