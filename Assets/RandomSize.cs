﻿using UnityEngine;
using System.Collections;

public class RandomSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.localScale = Vector3.one * Random.Range(1f,2f);
	
	}
	
	
}
