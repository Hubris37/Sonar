using UnityEngine;
using System.Collections;

public class ChangeMaterial : MonoBehaviour 
{
	public LightTrigger lights;
	public Material echoMaterial;
	public Renderer[] objects;

	// Use this for initialization
	void Start () 
	{
		lights.LightsAreOut += SwapMaterial;		
	}
	
	private void SwapMaterial()
	{
		for(int i = 0; i < objects.Length; i++)
		{
			objects[i].material = echoMaterial;
		}
	}
}