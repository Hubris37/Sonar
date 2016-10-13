using UnityEngine;
using System.Collections;

public class GramophoneSpawner : MonoBehaviour 
{
	public GameObject gramophone;
	public LightTrigger lightTrigger;

	// Use this for initialization
	void Start () 
	{
		lightTrigger.LightsAreOut += SpawnGramophone; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void SpawnGramophone()
	{
		Vector3 offset = new Vector3(0,0.1f,0);
		GameObject g = Instantiate(gramophone, transform.position + offset, Quaternion.identity) as GameObject;
		g.transform.parent = gameObject.transform;
	}
}
