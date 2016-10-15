using UnityEngine;
using System.Collections;

public class PostProcess : MonoBehaviour {

	public Shader depth, echolocation;
	public Material imgEffectMat;

	private Camera cam;

	void Start () {
		cam = GetComponent<Camera>();
		// cam.depthTextureMode = DepthTextureMode.Depth;
		// cam.SetReplacementShader(depth, "");
	}

	void Update() {
		
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		Graphics.Blit(source, destination, imgEffectMat);
	}
}
