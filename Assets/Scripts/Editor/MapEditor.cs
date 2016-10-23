﻿using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof (Maze))]
public class MapEditor : Editor {

	public override void OnInspectorGUI() 
	{
		Maze MazeGenerator = target as Maze;
		
		if (DrawDefaultInspector ()) {
			MazeGenerator.Generate(0);
		}

		if(GUILayout.Button ("Generate Maze")) {
			MazeGenerator.Generate(0);
		}

	}

}
#endif