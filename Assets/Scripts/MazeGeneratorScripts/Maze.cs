using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {

	public IntVector2 size;
    private MazeCell[,] cells;
	private List<MazeRoom> rooms = new List<MazeRoom>();
	public MazeRoomSettings[] roomSettings;

    public MazeCell cellPrefab;
    public MazePassage passagePrefab;
	public MazeDoor doorPrefab;
    public MazeWall[] wallPrefabs;

	[Range(0f, 1f)]
	public float doorProbability;
	[Range(0f, 1f)]
	public float decorProbability;

	public MazeCell GetCell (IntVector2 coordinates) {
		return cells [coordinates.x, coordinates.z];
	}

	public void Generate () {
		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell> ();
		DoFirstGenerationStep (activeCells);
		while (activeCells.Count > 0) {
			DoNextGenerationStep (activeCells);
		}
	}

	private void DoFirstGenerationStep (List<MazeCell> activeCells) {
		MazeCell newCell = CreateCell(RandomCoordinates);
		newCell.Initialize(CreateRoom(-1),false);
		activeCells.Add(newCell);
	}

	private void DoNextGenerationStep (List<MazeCell> activeCells) {
		int currentIndex = activeCells.Count - 1;
		MazeCell currentCell = activeCells [currentIndex];
        
        if (currentCell.IsFullyInitialized) {
            activeCells.RemoveAt(currentIndex);
            return;
        }

		MazeDirection direction = currentCell.RandomUninitializedDirection;
        IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2 ();
		if (ContainsCoordinates (coordinates)) {
            MazeCell neighbor = GetCell(coordinates);
            if (GetCell(coordinates) == null)
            {
                neighbor = CreateCell(coordinates);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
			else if (currentCell.room == neighbor.room) {
				CreatePassageInSameRoom(currentCell, neighbor, direction);
			}
            else {
                CreateWall(currentCell, neighbor, direction);
            }
		} else {
            CreateWall(currentCell, null, direction);
        }
	}

    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		bool createDecor = Random.value < decorProbability ? true : false;

		MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;
		MazePassage passage = Instantiate(prefab,cell.transform.position,direction.ToRotation()) as MazePassage;

        passage.Initialize(cell, otherCell, direction);
        //passage = Instantiate(prefab) as MazePassage;
		if (passage is MazeDoor) {
			otherCell.Initialize(CreateRoom(cell.room.settingsIndex),false);
		}
		else {
			otherCell.Initialize(cell.room,createDecor);
		}
        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

	private void CreatePassageInSameRoom (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
		MazeWall wall = Instantiate(wallPrefabs[Random.Range(0,wallPrefabs.Length)]) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
		//wall.transform.localScale = transform.localScale;
        if (otherCell != null)
        {
            //wall = Instantiate(wallPrefabs[Random.Range(0,wallPrefabs.Length)]) as MazeWall;
            cell.wallBetween.Add(otherCell);
            otherCell.wallBetween.Add(cell);
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }


    public MazeCell CreateCell (IntVector2 coordinates) {
		MazeCell newCell = Instantiate (cellPrefab) as MazeCell;
		cells [coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Maze Cell" + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		//newCell.transform.localScale = transform.localScale;
		newCell.transform.localPosition = 
			new Vector3(coordinates.x - size.x * 1f + 0.5f, 0f, coordinates.z - size.z * 1f + 0.5f);
		return newCell;
	}

	private MazeRoom CreateRoom (int indexToExclude) {
		MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
		newRoom.settingsIndex = Random.Range(0, roomSettings.Length);
		if (newRoom.settingsIndex == indexToExclude) {
			newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;
		}
		newRoom.settings = roomSettings[newRoom.settingsIndex];
		rooms.Add(newRoom);
		return newRoom;
	}

	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2 (Random.Range (0, size.x), Random.Range (0, size.z));
		}
	}

	public bool ContainsCoordinates (IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}

    public List<MazeCell> getCellList() {
        List<MazeCell> cellList = new List<MazeCell>();
        foreach (MazeCell c in cells) {
            cellList.Add(c);
        }
        return cellList;
    }

    public List<MazeRoom> getRooms() {
        return rooms;
    }

}