using UnityEngine;
using System;
using System.Collections;

public class LightTrigger : MonoBehaviour 
{
	public event Action LightZoneEnter;
	public event Action DarkZoneEnter;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
			DarkZoneEnter();		
	}

	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
			LightZoneEnter();
	}
}
