using UnityEngine;
using System.Collections;

public class OverviewCameraScript : MonoBehaviour {

	public GameObject shaderCtrlObj;
	public ShaderController shaderCtrl;
	public FirstPersonController player;

	public bool rotate = true;
	public float rotationSpeed = 3;
	public float moveSpeed = 4;
	[RangeAttribute(1,100)]
	public float radius = 5;
	private Vector3 rotationPoint;

	private const int MAX_CIRCLES = 120; // Maximum circles allowed at once
	// private Color[] colorsArray = new Color[MAX_CIRCLES];

	private Camera m_camera;
	
	void Start() {
		shaderCtrlObj = GameObject.FindGameObjectWithTag("ShaderController");
		shaderCtrl = (ShaderController) shaderCtrlObj.GetComponent(typeof(ShaderController));

		m_camera = GetComponent<Camera>();
		m_camera.backgroundColor = new Color(.266f, .486f, .482f, 1);

		player = FindObjectOfType<FirstPersonController> ();

		rotationPoint = new Vector3(-24, 0, -24);

		// for(int i = 0; i < colorsArray.Length; i++) {
		// 	colorsArray[i] = Color.HSVToRGB(.2f, 0.9f, .8f);;
		// }
		//transform.position = (transform.position - rotationPoint).normalized * radius + rotationPoint;
	}

	void Awake() {
		//transform.position = player.transform.position + new Vector3(0,10f,0);
	}

	void FixedUpdate() {
		rotationPoint = player.transform.position;
		transform.RotateAround(rotationPoint, Vector3.up, rotationSpeed * Time.deltaTime);
		//transform.position = (transform.position - rotationPoint).normalized * radius + rotationPoint;
		Vector3 desiredPosition = (transform.position - rotationPoint).normalized * radius + rotationPoint;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * moveSpeed);
	}

	void OnPreRender() {
		for(int i = 0; i < shaderCtrl.r.sharedMaterials.Length; ++i)
        {
			shaderCtrl.r.sharedMaterials[i].SetColor("_DefaultColor", new Color(.75f, .35f*i, .4f, 1));
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
}
