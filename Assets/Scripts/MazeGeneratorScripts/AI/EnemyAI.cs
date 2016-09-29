using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour {

    private Maze maze;
    private List<MazeCell> mazeNodes;

    private bool moving = false;
    private List<MazeCell> movementPath;
    private MazeCell startCell;

    [Header("Behaviour modifiers")]
    public float aggroRange = 1.0f;
    public float grabRange = 0.6f;
    public int patrolMinRoomSize = 4;
    public bool patrolsRoom = true;

    [Header("Running modifiers")]
    public MazeCell currentPositionCell;
    public float movementSpeed = 1.4f;
    public float chasingSpeedMultiplier = 1.2f;


    [Header("Jumping modifiers")]
    public float jumpDistance = 0.5f;
    public float jumpChaseMultiplier = 1.5f;
    public float jumpHeight = 0.4f;
    public int jumpCooldown = 30;
    private int jumpCounter = 0;
    public int jumpOffset = 10;
    private int jumpOffsetCounter = 0;
    private Vector3 jumpStartPos;

    private Vector3 playerPos;
    private float playerNoise;

    private GameManager gameManager;

    private bool isChasing = false;
    private Animator anim;

    public delegate void SoundBlastHit(Vector3 hitPos, float pitchVal, float dbVal);
	public static event SoundBlastHit onBlastHit;
    private bool makeSound = true;

    // Use this for initialization
    void Start() {
        movementPath = new List<MazeCell>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision collision) {
        print(collision.transform.name);
    }

    public void initializeAI(Maze m, MazeCell c = null) {
        maze = m;
        mazeNodes = maze.getCellList();
        currentPositionCell = c;
        if (currentPositionCell == null)
            findCurrentCell();
        startCell = currentPositionCell;
        if (currentPositionCell.room.getCells().Count < patrolMinRoomSize) {
            patrolsRoom = false;
        }
    }

    private void findCurrentCell() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
            currentPositionCell = hit.transform.parent.transform.gameObject.GetComponent<MazeCell>();
        }
        else {
            Debug.LogWarning("Error: Could not find current MazeCell of AI.", transform);
        }
    }

    // Update is called once per frame
    void Update() {
        findPath();
        move();
        checkAggro();
    }

    public void getPlayerInformation() {
        playerPos = gameManager.player.transform.position;
        playerNoise = gameManager.audioMeasure.DbValue;
    }

    private void checkAggro() {
        getPlayerInformation();
        if (!isChasing) {
            float detectionRadius = aggroRange + Mathf.Max(0, playerNoise);
            Vector3 dif = (playerPos - transform.position);
            dif.y = 0;
            isChasing = (dif.magnitude < detectionRadius) ? true : false;
        }
        // Check if in line of sight
        if (isChasing) {
            Vector3 pos = transform.position;
            pos.y += 0.5f;
            Vector3 dir = pos - playerPos;
            RaycastHit hit;
            if (Physics.Raycast(playerPos, dir.normalized, out hit, dir.magnitude)) {
                 print(hit.transform.name);
                if (!hit.transform.name.Contains("Body") && !hit.transform.name.Contains("Door")) {
                    isChasing = false;
                    findCurrentCell();
                    movementPath = aStar(startCell, mazeNodes);
                }
            }
        }
        anim.SetBool("chasing", isChasing);
    }

    private void findPath() {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Seek")) {
            anim.SetBool("seek", false);
        }
        if (movementPath.Count == 0) {
            anim.SetBool("seek", true);
            List<MazeCell> map;
            if (patrolsRoom) {
                // Get a random cell in the current room
                map = currentPositionCell.room.getCells();
            }
            else {
                map = mazeNodes;
            }
            movementPath = aStar(findTargetPosition(map), mazeNodes);
        }
    }

    private MazeCell findTargetPosition(List<MazeCell> map) {
        return map[Random.Range(0, map.Count)];
        MazeCell target = currentPositionCell;
        float maxDist = 0;
        foreach (MazeCell m in map) {
            target = (heuristicCost(currentPositionCell, target) < heuristicCost(currentPositionCell, m)) ? m : target;
        }
        return target;
    }

    private List<MazeCell> aStar(MazeCell targetCell, List<MazeCell> map) {
        List<MazeCell> closedSet = new List<MazeCell>();
        List<MazeCell> openSet = new List<MazeCell>();
        openSet.Add(currentPositionCell);

        Dictionary<MazeCell, MazeCell> cameFrom = new Dictionary<MazeCell, MazeCell>();

        Dictionary<MazeCell, int> gScore = new Dictionary<MazeCell, int>();
        Dictionary<MazeCell, int> fScore = new Dictionary<MazeCell, int>();
        foreach (MazeCell m in map) {
            gScore.Add(m, int.MaxValue);
            fScore.Add(m, int.MaxValue);
            cameFrom.Add(m, currentPositionCell);
        }

        gScore[currentPositionCell] = 0;
        fScore[currentPositionCell] = heuristicCost(currentPositionCell, targetCell);

        while (openSet.Count != 0) {
            MazeCell c = openSet[0];
            int minF = fScore[c];
            foreach (MazeCell m in openSet) {
                if (fScore[m] < minF) {
                    c = m;
                    minF = fScore[m];
                }
            }
            if (c == targetCell) return reconstructPath(cameFrom, c);
            openSet.Remove(c);
            closedSet.Add(c);
            foreach (MazeCell n in c.getNeighbours()) {
                if (closedSet.Contains(n) || !map.Contains(n)) continue;

                int tentGScore = gScore[c] + 1;
                if (!openSet.Contains(n))
                    openSet.Add(n);
                else if (tentGScore >= gScore[n])
                    continue;
                cameFrom[n] = c;
                gScore[n] = tentGScore;
                fScore[n] = gScore[n] + heuristicCost(n, targetCell);
            }
        }
        // Debug.Log("Error: AI could not find a path.", transform);
        return openSet;
    }

    private int heuristicCost(MazeCell from, MazeCell to) {
        int dx = Mathf.Abs(from.coordinates.x - to.coordinates.x);
        int dy = Mathf.Abs(from.coordinates.z - to.coordinates.z);
        int D = 1;
        return D * (dx + dy);
    }

    private List<MazeCell> reconstructPath(Dictionary<MazeCell, MazeCell> cameFrom, MazeCell current) {
        List<MazeCell> path = new List<MazeCell>();
        path.Add(current);
        while (current != currentPositionCell) {

            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private void move() {
        Vector3 dif, movePoint;
        float jumpDist = jumpDistance;
        // If chasing player, move towards him/her
        if (isChasing) {
            dif = playerPos - transform.position;
            dif.y = 0;
            movePoint = playerPos;
            //movementMultiplier = chasingSpeedMultiplier;
            jumpDist *= jumpChaseMultiplier;
            tryGrabPlayer();
        }
        else {
            // Else, move on calculated path
            int tilesLeft = movementPath.Count;
            if (tilesLeft == 0) return;
            float thresh = 0.5f;
            dif = movementPath[0].transform.position - transform.position;
            dif.y = 0;
            movePoint = movementPath[0].transform.position;
            if (dif.magnitude < thresh) {
                currentPositionCell = movementPath[0];
                movementPath.RemoveAt(0);
                if (movementPath.Count == 0) return;
                // movementPath = tryDiagonal(movementPath);
            }
        }
        movePoint.y = transform.position.y;
        transform.LookAt(movePoint);
        //transform.Translate(dif.normalized * movementSpeed * movementMultiplier * Time.deltaTime, Space.World);
        jumpHandler(movePoint, jumpDist);
    }

    private void jumpHandler(Vector3 movePoint, float dist) {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            if (jumpCounter == 0) {
                jumpCounter = (isChasing) ? 0 : jumpCooldown;
                jumpOffsetCounter = jumpOffset;
                jumpStartPos = transform.position;
                anim.SetBool("jump", true);
            } else {
                jumpCounter--;
                if(makeSound) {
                    makeLandingSound();
                    makeSound = false;
                }
            }
        } 
        // If has landed
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")) {
            if (jumpOffsetCounter == 0) {                
                anim.SetBool("jump", false);
                Vector3 dir = movePoint - jumpStartPos;
                float distance = Mathf.Min(dir.magnitude, dist);
                transform.Translate(dir.normalized * distance * Time.deltaTime, Space.World);
                makeSound = true;
            } else {
                jumpOffsetCounter--;
            }
        }
    }

    private void makeLandingSound() {
		onBlastHit(transform.position, 750, .2f);
    }

    private List<MazeCell> tryDiagonal(List<MazeCell> path) {
        int tilesLeft = path.Count;
        //If there are only two tiles there are no chances for diagonal skips
        if (tilesLeft < 2) return path;
        int removed = 0;
        for (int i = 0; i < tilesLeft - 1 - removed; ++i) {
            RaycastHit hit;
            Vector3 dir = path[i + 1].transform.position - transform.position;
            if (Physics.Raycast(transform.position, dir.normalized, out hit, dir.magnitude)) {
                if (hit.transform.name.Contains("Wall"))
                    break;
            }
            else {
                Debug.LogWarning("Warning: Diagonal check ray collided with nothing.", transform);
            }
            path.RemoveAt(i);
            removed++;
        }
        return path;
    }

    private void tryGrabPlayer() {
        Vector3 dif = playerPos - transform.position;
        dif.y = 0;
        print(dif.magnitude);
        if (dif.magnitude <= grabRange) {
            gameManager.LostGame();
        }
    }
}
