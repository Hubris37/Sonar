using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

	private int totRoomsCleared = 0;
	public int tempRoomsCleared = 0;
	public bool saveRoomCount = true;
	public Text roomsClearedText;
	public Text tempRoomsClearedText;

    public AudioClip loseSound;
	public AudioClip winSound;

	private void Awake () {
		mazeInstance = FindObjectOfType<Maze> ();
		Instantiate(playerPrefab);
	}

	private void Start () {
		level = startingLevel;
		startingCoordinates = new IntVector2(0, 0);
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

		LoadRoomsCleared(); // load totalnumber of rooms cleared;
		tempRoomsCleared = 0;
    }

	private void Update () {
    	if (Input.GetKeyDown("z")) {
			RestartGame();
		}
		if(playerIsDead) {
			if(Input.GetKeyDown("z") || Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") ||
			Input.GetButtonDown("Fire3") || Input.GetButtonDown("Gamepad Y")) {
				RestartGame();
			}
		}
		
		if(Input.GetKeyDown("l"))
			SceneManager.LoadScene("TrainingArena");

		roomsClearedText.text = "Rooms Cleared(total): " + totRoomsCleared;
		tempRoomsClearedText.text = "Rooms Cleared: " + tempRoomsCleared;
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
		totRoomsCleared++;
		tempRoomsCleared++;
		isReborn();
		BeginGame();
		AudioManager.instance.PlaySound(winSound, player.transform.position);
		SaveRoomsCleared();
	}

    public void LostGame() {
        AudioManager.instance.PlaySound(loseSound, player.transform.position);
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


	public void SaveRoomsCleared()
	{
		if(saveRoomCount)
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/roomsCleared.dat", FileMode.Open);
			
			GameData data = new GameData();
			data.totRoomsCleared = totRoomsCleared;
			
			bf.Serialize(file, data);
			
			file.Close();
		}
	}

	public void LoadRoomsCleared()
	{
		if(File.Exists(Application.persistentDataPath + "/roomsCleared.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/roomsCleared.dat", FileMode.Open);
			GameData data = (GameData)bf.Deserialize(file);
			file.Close();

			if(totRoomsCleared < data.totRoomsCleared) //A bit of safety
				totRoomsCleared = data.totRoomsCleared;
		}
	}
}

[System.Serializable]
class GameData
{
	public int totRoomsCleared;
}
