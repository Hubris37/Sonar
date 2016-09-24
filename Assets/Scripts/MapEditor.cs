using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (GameManager))]
public class MapEditor : Editor {

	public override void OnInspectorGUI() {
		GameManager gameMan = (GameManager)target;

		DrawDefaultInspector();

		if(GUILayout.Button ("Generate")) {
			gameMan.GenerateMaze();
		}

	}

}
