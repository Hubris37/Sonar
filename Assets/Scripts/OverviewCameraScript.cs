﻿using UnityEngine;
using System.Collections;

public class OverviewCameraScript : MonoBehaviour {

	public GameObject shaderCtrlObj;
	public ShaderController shaderCtrl;
	public FirstPersonController player;
	public GameObject goal;

	public bool rotate = true;
	float rotationSpeed = 3;
	public float moveSpeed = 4;
	[RangeAttribute(1,100)]
	public float radius = 20;
	private Vector3 rotationPoint;
	public float timeUntilColorChange;
	float timeBetweenColor = 1f;
	float curCol = 0;
	Vector3 firstPos;

	private const int MAX_CIRCLES = 120; // Maximum circles allowed at once
	// private Color[] colorsArray = new Color[MAX_CIRCLES];

	private Camera m_camera;
	
	void Start() {
		firstPos = transform.position;
		//GameManager.isReborn += ResetPos;
		shaderCtrlObj = GameObject.FindGameObjectWithTag("ShaderController");
		goal = GameObject.FindGameObjectWithTag("Goal");
		shaderCtrl = (ShaderController) shaderCtrlObj.GetComponent(typeof(ShaderController));

		m_camera = GetComponent<Camera>();
		m_camera.backgroundColor = new Color(.266f, .286f, .282f, 1);

		player = FindObjectOfType<FirstPersonController> ();


		rotationPoint = new Vector3(0, 0, 0);


		// for(int i = 0; i < colorsArray.Length; i++) {
		// 	colorsArray[i] = Color.HSVToRGB(.2f, 0.9f, .8f);;
		// }
		//transform.position = (transform.position - rotationPoint).normalized * radius + rotationPoint;
	}

	void Awake() {
		//transform.position = player.transform.position + new Vector3(0,10f,0);
	}

	void Update() {
		if (Time.time > timeUntilColorChange) {
			curCol = 0.6f + Mathf.Sin(Time.time*0.02f)*0.2f;
			curCol %= 1;
			m_camera.backgroundColor = Color.HSVToRGB (curCol, 0.6f, 0.3f);
			timeUntilColorChange = Time.time+timeBetweenColor;
		}
	}


	void FixedUpdate() {
		//rotationPoint = player.transform.position;
		rotationPoint = (player.transform.position + goal.transform.position) / 2;
		float dist = (rotationPoint.magnitude+10)/10;
		m_camera.orthographicSize = Mathf.Clamp(20/dist,12,30);

		Debug.DrawLine (rotationPoint, player.transform.position);

		//transform.position = /*(transform.position - rotationPoint).normalized * radius +*/ rotationPoint + Vector3.up * 10;
		//rotationPoint *= Vector3.down * 10f;
		transform.RotateAround(rotationPoint, Vector3.up, rotationSpeed * Time.deltaTime);
		transform.position = /*(transform.position - rotationPoint).normalized * radius + */rotationPoint;

        //transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * moveSpeed);
		//Vector3 desiredPosition
		//transform.position = new Vector3(transform.position.x, 5, transform.position.z);
		//transform.Translate(Vector3.right * Time.deltaTime * rotationSpeed);
		//transform.LookAt (rotationPoint);
	}

	void OnPreRender() {
		for(int i = 0; i < shaderCtrl.r.sharedMaterials.Length; ++i)
        {
			float col1 =  (0.2f + 0.32f * i) % 1;
			float col2 = (0.22f * i) % 1;
			shaderCtrl.r.sharedMaterials[i].SetColor("_DefaultColor", new Color(col2, col1, .4f, 1));
			shaderCtrl.r.sharedMaterials[i].SetFloat("_WallO", .7f);
			shaderCtrl.r.sharedMaterials[i].SetFloat("_UseDepth", 0);
			// shaderCtrl.r.sharedMaterials[i].SetColorArray("_Color", colorsArray); // Sets all circle colors, IS NOT REVERSED
		}
		// shaderCtrl.r.sharedMaterial.shader = Shader.Find("Standard");
        // _default = material.GetColor(colorPropertyName);
        // material.SetColor(colorPropertyName, myColor);
    }

    void OnPostRender() {
		for(int i = 0; i < shaderCtrl.r.sharedMaterials.Length; ++i)
        {
			shaderCtrl.r.sharedMaterials[i].SetColor("_DefaultColor", new Color(0, 0, 0, 0));
			shaderCtrl.r.sharedMaterials[i].SetFloat("_WallO", 0);
			shaderCtrl.r.sharedMaterials[i].SetFloat("_UseDepth", 1);
		}
        // shaderCtrl.r.sharedMaterial.shader = Shader.Find("Custom/Echolocation");
    }

	void ResetPos() {
		transform.position = firstPos;
	}
		
}
