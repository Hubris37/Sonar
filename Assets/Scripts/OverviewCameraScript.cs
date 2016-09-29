using UnityEngine;
using System.Collections;

public class OverviewCameraScript : MonoBehaviour {

	public GameObject shaderCtrlObj;
	public ShaderController shaderCtrl;

	public bool rotate = true;
	[RangeAttribute(1,100)]
	public float rotationSpeed = 3;
	private Vector3 rotationPoint;

	private const int MAX_CIRCLES = 500; // Maximum circles allowed at once
	private Color[] colorsArray = new Color[MAX_CIRCLES];

	Camera camera;
	
	void Start() {
		shaderCtrlObj = GameObject.FindGameObjectWithTag("ShaderController");
		shaderCtrl = (ShaderController) shaderCtrlObj.GetComponent(typeof(ShaderController));

		camera = GetComponent<Camera>();
		camera.backgroundColor = new Color(.266f, .486f, .482f, 1);

		rotationPoint = new Vector3(-24, 0, -24);

		for(int i = 0; i < colorsArray.Length; i++) {
			colorsArray[i] = Color.HSVToRGB(.2f, 0.9f, .8f);;
		}
	}
	
	// Update is called once per frame
	void Update() {
		transform.RotateAround(rotationPoint, Vector3.up, rotationSpeed * Time.deltaTime);
	}

	void OnPreRender() {
		for(int i = 0; i < shaderCtrl.r.sharedMaterials.Length; ++i)
        {
			shaderCtrl.r.sharedMaterials[i].SetColor("_DefaultColor", new Color(.75f, .21f, .4f, 1));
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
