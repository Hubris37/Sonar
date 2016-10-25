using UnityEngine;
using System.Collections;

public class PostProcess : MonoBehaviour {
	
	public Material imgEffectMat;

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		Graphics.Blit(source, destination, imgEffectMat);
	}
}
