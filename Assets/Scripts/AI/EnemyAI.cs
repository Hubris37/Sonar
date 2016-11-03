using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyAI : MonoBehaviour {

    protected Maze maze;
    private List<MazeCell> mazeNodes;

    protected bool moving = false;
    public List<MazeCell> movementPath;
    protected MazeCell startCell;
    protected MazeCell currentPositionCell;

    [Header("Behaviour modifiers")]
    public float aggroRange = 1.0f;
    public float grabRange = 0.6f;
    public int patrolMinRoomSize = 4;
    public bool patrolsRoom = true;

    protected Vector3 playerPos;
    protected float playerNoise;
    protected Rigidbody rigid;

    protected GameManager gameManager;

    protected bool isAggroed = false;
    //public bool isInvestigating = false;
    protected bool chasingAround = false;
    protected Animator anim;

    public delegate void SoundBlastHit(Vector3 hitPos, float pitchVal, float dbVal);
	public static event SoundBlastHit onBlastHit;

    public AudioClip audioStartle;

    protected abstract void onAggro();
    protected abstract void onLoseAggro();
    public abstract void move();

    private float colliderCounter = 0;

    // Use this for initialization
    void Start() {
    }

    public void initializeAI(Maze m, MazeCell c = null) {
        maze = m;
        mazeNodes = maze.getCellList();
        currentPositionCell = c;
        if (currentPositionCell == null)
            findCurrentCell();
        startCell = currentPositionCell;
    /*    if (currentPositionCell.room.getCells().Count < patrolMinRoomSize) {
            patrolsRoom = false;
        } */
    }

    void OnCollisionStay(Collision other) {
        if (other.transform.name != "Quad" && colliderCounter <= 0) {
            colliderCounter = 1.0f;
            List<MazeCell> map = currentPositionCell.room.getCells();
            movementPath = aStar(map[Random.Range(0,map.Count)], map);
        }
        colliderCounter -= Time.deltaTime;
    }

    protected void findCurrentCell() {
      //  currentPositionCell = maze.GetCell(transform.position);
        // Tills vidare, gärna hitta korrekt formel för hitta MazeCell givet world pos istället
        RaycastHit hit;
        int layerMask = 1 << 11;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 5.0f, layerMask)) {
            currentPositionCell = hit.transform.parent.transform.gameObject.GetComponent<MazeCell>();
        }
        else {
            Debug.LogWarning("Error: Could not find current MazeCell of AI.", transform);
        }
    }

    protected List<MazeCell> pathToPlayer() {
            findCurrentCell();
        getPlayerInformation();
            return aStar(maze.GetCell(playerPos), mazeNodes);
    }

    // Update is called once per frame
    void Update() {
    }

    public void getPlayerInformation() {
        playerPos = gameManager.player.transform.position;
        playerNoise = gameManager.audioMeasure.DbValue;
    }

    protected void checkAggro(float addedRadius = 0f) {
        print(playerNoise);
        getPlayerInformation();
        if (!isAggroed) {
            float detectionRadius = aggroRange + Mathf.Max(0, playerNoise) + addedRadius;
            Vector3 dif = (playerPos - transform.position);
            dif.y = 0;
            if (dif.magnitude < detectionRadius && !wallBetweenPoint(playerPos)) {
                isAggroed = true;
                onAggro();
            }
        }
        else if (wallBetweenPoint(playerPos)) {
            isAggroed = false;
            onLoseAggro();
            findCurrentCell();
            movementPath = aStar(startCell, mazeNodes);
        }
    }
    

    protected bool wallBetweenPoint(Vector3 point) {
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        Vector3 dir = pos - point;
        RaycastHit hit;
        int layerMask = 1 << 9;
        if (Physics.Raycast(point, dir.normalized, out hit, dir.magnitude, layerMask)) {
            // Funkar inte för grandchildren
            if (hit.transform.name.Contains("Quad") || hit.transform.name.Contains("Gate") || hit.transform.name.Contains("Door")) {
                return true;
            }
        }
        return false;
    }

    protected bool straightLineToPlayer() {
        Vector3 pos = transform.position;
        Vector3 dir = pos - playerPos;
        RaycastHit hit;
        if (Physics.Raycast(playerPos, dir.normalized, out hit, dir.sqrMagnitude)) {
            // Funkar inte för grandchildren
            if (hit.transform == transform || hit.transform.IsChildOf(transform)) {
                return true;
            }
        }
        return false;
    }

    protected void findPath() {
        if (movementPath.Count == 0) {
            if (!isAggroed && Random.Range(0,1) > 0.9f) {
                rigid.velocity = Vector3.zero;
                anim.SetTrigger("seek");
            }
            //makeSniffingSound();
            List<MazeCell> map;
            if (patrolsRoom) {
                // Get a random cell in the current room
                map = currentPositionCell.room.getCells();
            }
            else {
                map = mazeNodes;
            }
            movementPath = aStar(findTargetPosition(map), map);
        }
    }

    protected MazeCell findTargetPosition(List<MazeCell> map) {
        if (!patrolsRoom)
            return map[Random.Range(0, map.Count)];
        
        return findMaxCostCell(currentPositionCell,map);
    }

    protected MazeCell findMaxCostCell(MazeCell start, List<MazeCell> map) {
        MazeCell furthest = start;
        foreach (MazeCell m in map) {
            furthest = (heuristicCost(start, furthest) < heuristicCost(start, m)) ? m : furthest;
        }
        return furthest;
    }

    protected List<MazeCell> aStar(MazeCell targetCell, List<MazeCell> map) {
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

    protected float getAnimationLength(string name) {
        RuntimeAnimatorController rac = anim.runtimeAnimatorController;
        foreach (AnimationClip ac in rac.animationClips) {
            if (ac.name == name) {
                return ac.length;
            }
        }
        Debug.LogError("getAnimationLength: Could not find animation with name " + name + ".");
        return 0;
    }



    protected void makeDetectionSound() {
        AudioManager.instance.PlaySound(audioStartle, transform.position);
        onBlastHit(transform.position, 1000, .25f);
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

    protected void tryGrabPlayer() {
        Vector3 dif = playerPos - transform.position;
        dif.y = 0;
        if (dif.magnitude <= grabRange) {
            gameManager.player.transform.position += new Vector3(40, 0, 40);
            gameManager.LostGame();
        }
    }

    protected void blastHit(Vector3 pos, float p, float db) {
        onBlastHit(pos, p, db);
    }

    protected void setCurrentPositionCell(MazeCell c) {
        currentPositionCell = c;
    }

    protected Vector3 getPathTargetPoint(Vector3 currentPos) {
        Vector3 dif, movePoint;
        int tilesLeft = movementPath.Count;
        if (tilesLeft == 0) return currentPos;
        float thresh = 0.5f;
        movePoint = movementPath[0].transform.position;
        dif = movePoint - currentPos;
        dif.y = 0;
        if (dif.magnitude < thresh) {
            currentPositionCell = movementPath[0];
            movementPath.RemoveAt(0);
            if (movementPath.Count == 0) return currentPos;
        }
        return movePoint;
    }

    protected void investigatePoint(Vector3 targetPoint, float aggroRadius) {
        MazeCell targetCell = maze.GetCell(targetPoint);
        if (!isAggroed && targetCell.room == currentPositionCell.room) {
            movementPath = aStar(maze.GetCell(targetPoint), mazeNodes);
            checkAggro(aggroRadius);
        }
    }

    protected void moveToFloor() {
        Vector3 pos = transform.position;
        pos.y = 0;
        rigid.velocity = new Vector3(0.001f, 0.0001f, 0.0001f);
        transform.Translate(currentPositionCell.transform.position);
    }
}
