using UnityEngine;
using System.Collections;

public class ChairSpawner : MonoBehaviour {

	public GameObject chairPrefab;

	// Use this for initialization
	void Start () {
		int chairs = Random.Range(1,5);

		for(int i = 0; i < chairs; ++i) {
			GameObject chair = Instantiate(chairPrefab);
			chair.transform.parent = transform;
			float xrad = 1.5f*Mathf.Sin(Mathf.Deg2Rad*i*90);
			float zrad = 1.5f*Mathf.Cos(Mathf.Deg2Rad*i*90);
			chair.transform.position = transform.position + new Vector3 (xrad,1f,zrad);
			chair.transform.LookAt(transform.position + Vector3.up);
		}
	}
}
