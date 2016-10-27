using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {

	public IntVector2 size;
	[Range(1f,4f)]
	public float cellScale;
	public int seed;
	public bool hideRooms = true;

    private MazeCell[,] cells;
	private List<MazeRoom> rooms = new List<MazeRoom>();
	public MazeRoomSettings[] roomSettings;

	public IntVector2 playerCoordinates;

	[HeaderAttribute("Prefabs")]
    public MazeCell cellPrefab;
    public MazePassage passagePrefab;
	public MazeWall wallPrefab;
	public MazeDoor[] doorPrefab;

	[Range(0f, 1f)]
	public float doorProbability;

	public MazeCell GetCell (IntVector2 coordinates) {
		return cells [coordinates.x, coordinates.z];
	}

    public MazeCell GetCell(Vector3 worldPosition) {
		int xCell = Mathf.FloorToInt(worldPosition.x / cellScale + size.x/2f - 0.5f);
		int zCell = Mathf.FloorToInt(worldPosition.z / cellScale + size.z/2f - 0.5f);
        return cells[xCell, zCell];
    }

    public void Generate () {

		Random.InitState (seed);

		string holderName = "Generated Maze";
		if(transform.FindChild (holderName)) {
			DestroyImmediate(transform.FindChild(holderName).gameObject);
		}

		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;
		mapHolder.localScale *= cellScale;

		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell> ();
		rooms = new List<MazeRoom>();
		
		DoFirstGenerationStep (activeCells, mapHolder);
		while (activeCells.Count > 0) {
			DoNextGenerationStep (activeCells, mapHolder);
		}

		if(hideRooms) {
			for (int i = 0; i < rooms.Count; i++) {
				rooms[i].Hide();
			}
		}
		
	}

	private void DoFirstGenerationStep (List<MazeCell> activeCells, Transform mapHolder) {
		MazeCell newCell = CreateCell(RandomCoordinates, mapHolder);
		newCell.Initialize(CreateRoom(-1),false);
		activeCells.Add(newCell);
	}

	private void DoNextGenerationStep (List<MazeCell> activeCells, Transform mapHolder) {
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
                neighbor = CreateCell(coordinates, mapHolder);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
			else if (currentCell.room.settingsIndex == neighbor.room.settingsIndex) {
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
		bool createDecor = Random.value < cell.room.settings.decorProbability ? true : false;
		if(otherCell.coordinates.x == playerCoordinates.x && otherCell.coordinates.z == playerCoordinates.z) {
			createDecor = false;
		}

		MazePassage prefab = Random.value < doorProbability ? doorPrefab[Random.Range(0,doorPrefab.Length)] : passagePrefab;
		MazePassage passage = Instantiate(prefab,cell.transform.position,direction.ToRotation()) as MazePassage;

        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(prefab) as MazePassage;
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
		if (cell.room != otherCell.room) {
			MazeRoom roomToAssimilate = otherCell.room;
			cell.room.Assimilate(roomToAssimilate);
			rooms.Remove(roomToAssimilate);
			DestroyImmediate(roomToAssimilate);
		}
	}

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
		MazeWall wall = Instantiate(wallPrefab) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
		
		bool createDecor = Random.value < cell.room.settings.decorProbability ? true : false;
		if (createDecor && cell.room.settings.WallDecor.Length > 0){
			GameObject decor = Instantiate(cell.room.settings.WallDecor[Random.Range(0,cell.room.settings.WallDecor.Length-1)],wall.transform.position,wall.transform.rotation) as GameObject;
			decor.transform.parent = wall.transform;
		}

        if (otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MazeWall;
            cell.wallBetween.Add(otherCell);
            otherCell.wallBetween.Add(cell);
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }


    public MazeCell CreateCell (IntVector2 coordinates, Transform mapHolder) {
		MazeCell newCell = Instantiate (cellPrefab) as MazeCell;
		cells [coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Maze Cell" + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = mapHolder.transform;
		newCell.transform.localScale *= cellScale;
		//newCell.transform.localPosition = new Vector3(coordinates.x - size.x + 0.5f, 0f, coordinates.z - size.z + 0.5f);
		newCell.transform.localPosition = 
			new Vector3(coordinates.x - size.x / 2f + 0.5f, 0f, coordinates.z - size.z / 2f + 0.5f);

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