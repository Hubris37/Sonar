using UnityEngine;
using System;
using System.Collections;

public class LightTrigger : MonoBehaviour 
{
	public event Action LightsAreOut;
	public Light dirLight;

	private float lightIntens = 1;

	void Start()
	{
		RenderSettings.skybox.SetFloat("_Exposure", 1);RenderSettings.skybox.SetFloat("_Exposure", 1);
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
