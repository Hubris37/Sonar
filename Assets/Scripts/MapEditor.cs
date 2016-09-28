using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
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
#endif