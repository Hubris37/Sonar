﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour {

    private Maze maze;

    private bool moving = false;
    private List<MazeCell> movementPath;
    public MazeCell currentPositionCell;
    public float movementSpeed = 1.4f;
    public float chasingSpeedMultiplier = 1.2f;
    public float aggroRange = 1.0f;

    private Vector3 playerPos;
    private float playerNoise;

    private GameManager gameManager;

    public bool patrolsRoom;
    private bool isChasing = false;

    // Use this for initialization
    void Start() {
        movementPath = new List<MazeCell>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void initializeAI(Maze m) {
        maze = m;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
            currentPositionCell = hit.transform.parent.transform.gameObject.GetComponent<MazeCell>();
        }
        else {
            Debug.LogError("Error: Could not find MazeCell for AI.", transform);
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
            isChasing = ((playerPos - transform.position).magnitude < detectionRadius) ? true : false;
        }
        // Check if in line of sight
        if (isChasing) {
            Vector3 dir = playerPos - transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dir.normalized, out hit, dir.magnitude)) {
                if (hit.transform.name.Contains("Wall"))
                    isChasing = false;
            }
        }
    }

    private void findPath() {
        if (movementPath.Count == 0) {
            if (patrolsRoom) {
                // Get a random cell in the current room
                List<MazeCell> roomCells = currentPositionCell.room.getCells();
                movementPath = aStar(findTargetPosition(), roomCells);
            }
        }
    }

    private MazeCell findTargetPosition() {
        // return roomCells[Random.Range(0, roomCells.Count)];
        MazeCell target = currentPositionCell;
        float maxDist = 0;
        foreach (MazeCell m in currentPositionCell.room.getCells()) {
            target = (heuristicCost(currentPositionCell, target) < heuristicCost(m, target)) ? m : target;
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
        Debug.Log("Error: AI could not find a path.", transform);
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
        Vector3 dif;
        float movementMultiplier = 1.0f;
        // If chasing player, move towards him/her
        if (isChasing) {
            dif = playerPos - transform.position;
            dif.y = 0;
            movementMultiplier = chasingSpeedMultiplier;
        }
        else {
            // Else, move on calculated path
            int tilesLeft = movementPath.Count;
            if (tilesLeft == 0) return;
            movementPath = tryDiagonal(movementPath);
            float thresh = 0.5f;
            dif = movementPath[0].transform.position - transform.position;
            dif.y = 0;
            if (dif.magnitude < thresh) {
                currentPositionCell = movementPath[0];
                movementPath.RemoveAt(0);
                if (movementPath.Count == 0) return;
            }
        }
        transform.Translate(dif.normalized * movementSpeed * movementMultiplier * Time.deltaTime);
    }

    private List<MazeCell> tryDiagonal(List<MazeCell> path) {
        int tilesLeft = path.Count;
        //If there are only two tiles there are no chances for diagonal skips
        if (tilesLeft < 2) return path;
        int removed = 0;
        for (int i = 0; i < tilesLeft-1-removed; ++i) {
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

    }
}
