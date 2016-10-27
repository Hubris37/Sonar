using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeCell : MonoBehaviour {

    public IntVector2 coordinates;
    private int initializedEdgeCount;
    public MazeRoom room;
    private GameObject decor;
    public MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];
    public List<MazeCell> wallBetween = new List<MazeCell>();
    public bool AISpawnable = true;

    public MazeCellEdge GetEdge(MazeDirection direction) {
        return edges[(int)direction];
    }

    public List<MazeCell> getNeighbours() {
        List<MazeCell> neighbours = new List<MazeCell>();
        for (int i = 0; i < MazeDirections.Count; ++i) {
            if (!wallBetween.Contains(edges[i].otherCell) && !wallBetween.Contains(edges[i].cell)) {
                if (edges[i].otherCell == this)
                    neighbours.Add(edges[i].cell);
                else
                    neighbours.Add(edges[i].otherCell);
            }
        }
        return neighbours;
    }

	public void Initialize(MazeRoom room, bool decoration, bool useShaderMaterials) {
        room.Add(this);
        if (decoration && room.settings.Decor.Length > 0){
            decor = Instantiate(room.settings.Decor[Random.Range(0,room.settings.Decor.Length)],transform.position,Quaternion.identity) as GameObject;
            decor.transform.parent = transform;
            AISpawnable = false;
        }
		if(useShaderMaterials) {
        	transform.GetChild(0).GetComponent<Renderer>().material = room.settings.floorMaterial;
		}
    }

    public void SetEdge(MazeDirection direction, MazeCellEdge edge) {
        edges[(int)direction] = edge;
        initializedEdgeCount += 1;
    }

    public bool IsFullyInitialized {
        get {
            return initializedEdgeCount == MazeDirections.Count;
        }
    }

    public MazeDirection RandomUninitializedDirection {
        get {
            int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
            for (int i = 0; i < MazeDirections.Count; i++) {
                if (edges[i] == null) {
                    if (skips == 0) {
                        return (MazeDirection)i;
                    }
                    skips -= 1;
                }
            }
            throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
        }
    }

    public void Show () {
        if(!isActiveAndEnabled) {
            gameObject.SetActive(true);
            foreach(MazeCellEdge edge in edges) {
                if(edge is MazeDoor && edge.otherCell.isActiveAndEnabled) {
                    //edge.gameObject.SetActive(false);
                    MazeDoor door = edge as MazeDoor;

                    for (int i = 0; i < door.transform.childCount; i++) {
                        Transform child = door.transform.GetChild(i);
                        if (child == door.hinge) {
                            child.gameObject.SetActive(false);
                        }
                    }
                }
                if(edge is MazeWall && wallBetween.Contains(edge.otherCell)){
                    //edge.otherCell.gameObject.SetActive(true);
                    //MazeCellEdge e = edge.otherCell.GetEdge(MazeDirections.GetOpposite(edge.direction)); // never used
                    
                    //e.gameObject.SetActive(false);
                }
            }
        }
	}

	public void Hide () {
		gameObject.SetActive(false);
	}
}
