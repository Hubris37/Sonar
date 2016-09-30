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

    private AudioSource audioPlayer;

	private void Start () {
        audioPlayer = GetComponent<AudioSource>();
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
		IntVector2 startingCoordinates = new IntVector2(0, 0);
		MazeCell startingCell = mazeInstance.GetCell (startingCoordinates);
		Vector3 pos = startingCell.transform.position;

		player.transform.position = new Vector3(pos.x, pos.y+1f, pos.z);
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
		mazeInstance.Generate(level);

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
        audioPlayer.Play();
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
            if (roomsLeft.Count == 0) {
                roomsLeft = mazeInstance.getRooms();
				if(roomsLeft.Count != 1) {
					roomsLeft.Remove(startingCell.room);
				}
			}
            // Select a random room
			MazeRoom room = roomsLeft[Random.Range(0, roomsLeft.Count-1)];
			// Select a random cell in the room to initialize at
			MazeCell initCell = room.getCells()[Random.Range(0, room.getCells().Count-1)];
			// Remove current room from unoccupied ones
			roomsLeft.Remove(room);
			Vector3 initPos = initCell.transform.position;
			GameObject bot = Instantiate(AIPrefab, initPos, Quaternion.identity) as GameObject;
			bots.Add(bot);
			bot.GetComponent<EnemyAI>().initializeAI(mazeInstance, initCell);
			
        }
    }
}
