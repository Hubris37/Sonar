using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public FirstPersonController player;
	GameObject goal;
	public GameObject playerPrefab;
	public GameObject goalPrefab;
	private Maze mazeInstance;
    public AudioMeasure audioMeasure;

    private List<GameObject> bots;
    public GameObject Chef;
    public GameObject Gramophone;
    public GameObject Waiter;
    public int startChefAmount = 3;
    public int startGramophoneAmount = 1;
    public int startWaiterAmount = 1;
    private int chefAmount, gramophoneAmount, waiterAmount;
	private bool playerIsDead = false;

	public int startingLevel = 0;
	int level;
	public IntVector2 startingSize;
	private IntVector2 startingCoordinates;

	public delegate void PlayerState();
	public static event PlayerState isDead;
	public static event PlayerState isReborn;

    private AudioSource audioPlayer;

	private void Awake () {
		mazeInstance = FindObjectOfType<Maze> ();
		Instantiate(playerPrefab);
	}

	private void Start () {
		level = startingLevel;
		startingCoordinates = new IntVector2(0, 0);
        audioPlayer = GetComponent<AudioSource>();
        bots = new List<GameObject>();
		goal = Instantiate (goalPrefab);

		player = FindObjectOfType<FirstPersonController> ();
        audioMeasure = GameObject.Find("AudioMeasure source").GetComponent<AudioMeasure>();
        player.OnGoalTouch += WonGame;
		player.goal = goal;
        chefAmount = startChefAmount;
        gramophoneAmount = startGramophoneAmount;
        waiterAmount = startWaiterAmount;
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
		MazeCell startingCell = mazeInstance.GetCell (startingCoordinates);
		Vector3 pos = startingCell.transform.position;

		player.transform.position = new Vector3(pos.x, pos.y+0.5f, pos.z);
		
		startingCell.room.Show();

		pos = mazeInstance.GetCell (new IntVector2 (mazeInstance.size.x - 1, mazeInstance.size.z - 1)).transform.position;
		goal.transform.position = new Vector3(pos.x, pos.y+0.5f, pos.z);
		player.transform.LookAt(pos);

        spawnAI(Chef, chefAmount, startingCell);
        spawnAI(Gramophone, gramophoneAmount, startingCell);
        spawnAI(Waiter, waiterAmount, startingCell);
    }

	public void GenerateMaze() {
		mazeInstance.playerCoordinates = startingCoordinates;
		mazeInstance.size.x = startingSize.x + level;
		mazeInstance.size.z = startingSize.z + level;
		mazeInstance.Generate();
	}

	private void RestartGame () {
        DestroyLevel();
		level = startingLevel;
        chefAmount = startChefAmount;
		BeginGame ();
		isReborn();
		player.freezeMovement = false;
		playerIsDead = false;
	}

    private void DestroyLevel() {
		mazeInstance.DestroyMaze ();
        foreach (GameObject bot in bots) {
            Destroy(bot);
        }
        bots.Clear();
    }

	private void WonGame() {
		DestroyLevel();
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
        List<MazeRoom> spawnableRooms = mazeInstance.getRooms();
        spawnableRooms.Remove(startingCell.room);
        List<MazeRoom> roomsLeft = new List<MazeRoom>(spawnableRooms);
        // Create as many AIs as specified
        for (int i = 0; i < count; ++i) {
            MazeCell initCell = null;
            MazeRoom room = null;
            // If no free rooms left, set all to available
            if (roomsLeft.Count == 0) {
                roomsLeft = new List<MazeRoom>(spawnableRooms);
                break;
            }
            // Select a random room
            int rand = Random.Range(0, roomsLeft.Count - 1);
            room = roomsLeft[rand];
            initCell = getSpawnableCell(room);
            
			// Remove current room from unoccupied ones
            if (initCell == null) {
                //i--;
                roomsLeft.Remove(room);
                continue;   
            }
			roomsLeft.Remove(room);
			Vector3 initPos = initCell.transform.position;
			GameObject bot = Instantiate(AIPrefab, initPos, Quaternion.identity) as GameObject;
			bot.transform.parent = initCell.transform;
			bots.Add(bot);
			bot.GetComponent<EnemyAI>().initializeAI(mazeInstance, initCell);
            initCell.AISpawnable = false;
        }
    }

    private MazeCell getSpawnableCell(MazeRoom room) {
        // Select a random cell in the room to initialize at
        foreach (MazeCell c in room.getCells()) {
            if (c.AISpawnable)
                return c;
        }
        return null;

    }

}
