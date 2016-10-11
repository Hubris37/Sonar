using UnityEngine;
using System;

[Serializable]
public class MazeRoomSettings {

	public Material floorMaterial, wallMaterial;
	public GameObject[] Decor;
	public GameObject[] WallDecor;
	[Range(0, 1)]
	public float decorProbability;
}