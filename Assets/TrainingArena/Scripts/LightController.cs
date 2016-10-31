using UnityEngine;
using System;
using System.Collections;

// Controlls the directional light and skybox.
public class LightController : MonoBehaviour 
{
	public event Action LightsAreOut;
	public Light dirLight;

	private float lightIntens = 1;
	void Awake()
	{
		RenderSettings.skybox.SetFloat("_Exposure", 1);
	}

	void Start()
	{
		
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			StartCoroutine(FadeOut());
		}
	}
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			StopAllCoroutines();
			dirLight.intensity = 1;
			RenderSettings.skybox.SetFloat("_Exposure", 1);
			lightIntens = 1f;
		}
	}

	// Fade out DirLight and skybox
	public IEnumerator FadeOut()
	{
		while(lightIntens > 0)
		{
			lightIntens -= 0.005f;
			RenderSettings.skybox.SetFloat("_Exposure", lightIntens);
			dirLight.intensity = lightIntens;
			yield return new WaitForSeconds(0.01f);
		}
		LightsAreOut();
		yield return null;
	}

	public void TurnOff()
	{
		RenderSettings.skybox.SetFloat("_Exposure", 0);
		dirLight.intensity = 0;
		LightsAreOut();
	}

	// Prevent skybox from having exposure 0 when exiting playmode
	void OnApplicationQuit()
	{
		RenderSettings.skybox.SetFloat("_Exposure", 1);		
	}

	// When the scene is changed to movementTest
	void OnDestroy()
	{
		RenderSettings.skybox.SetFloat("_Exposure", 1);		
	}

}
