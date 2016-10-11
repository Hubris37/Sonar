using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour 
{
	public TutorialGoal tutGoal;
	public string nextLvlName;
	[Header("Lights")] 	
	public Light dirLight;
	public float lowestIntensity = 0f;
	public float highestIntensity = 1f;
	public LightTrigger lightTrigger;
	public AudioClip switchOn;
	public AudioClip switchOff;

	private AudioSource aud;

	void Start()
	{
		tutGoal.OnGoalEnter += ChangeScene;
		lightTrigger.DarkZoneEnter += DarkZone;
		lightTrigger.LightZoneEnter += LightZone;
		aud = GetComponent<AudioSource>();
	}


	void LightZone()
	{
/*		StopCoroutine(DimLight(0));
		StartCoroutine(DimLight(highestIntensity));*/
		dirLight.intensity = highestIntensity;
		aud.clip = switchOn;
		aud.Play();
		
	}

	void DarkZone()
	{
		/*StopCoroutine(DimLight(0));
		StartCoroutine(DimLight(lowestIntensity));*/
		dirLight.intensity = lowestIntensity;
		aud.clip = switchOff;
		aud.Play();
	}

	private void ChangeScene()
	{
		if(NiceSceneTransition.instance != null)
		{
			NiceSceneTransition.instance.LoadScene(nextLvlName);
		} else {
			SceneManager.LoadScene(nextLvlName);
		}
	}

	IEnumerator DimLight(float dimTo)
	{
		while(dirLight.intensity != dimTo)
		{
			float curIntens = dirLight.intensity;
			dirLight.intensity = Mathf.Lerp(curIntens, dimTo, Time.deltaTime);
			yield return null;
		}
	}
}
