using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WiimoteApi;

public class WiiMoteController : MonoBehaviour {
	static bool[] leds1 = new bool[4] { true, false, false, false };
	static bool[] leds2 = new bool[4] { false, true, false, false };
	static bool[] leds3 = new bool[4] { false, false, true, false };
	static bool[] leds4 = new bool[4] { false, false, false, true };
	static bool[] leds5 = new bool[4] { true, true, false, false };
	static bool[] leds6 = new bool[4] { true, true, true, false };
	static bool[] leds7 = new bool[4] { true, true, true, true };
	static bool[] leds8 = new bool[4] { false, true, false, false };
	static bool[][] ledsList = new bool[][] { leds1, leds2, leds3, leds4, leds5, leds6, leds7, leds8 };

	// public bool blink = false;
	public float basePitch = 200;
	public float audioLevel = 0.01f;
	public Camera cam;
	public GameObject facePrefab;
	public GameObject projectilePrefab;
	public GameObject pointerPrefab;

	protected FirstPersonController player;

	private int ledCount = 0;
	private float prevTime = 0;
	private List<float> prevSoundTimes = new List<float>();
	private List<GameObject> faces = new List<GameObject>();
	private List<Animator> animators = new List<Animator>();
	private List<float> pitches = new List<float>();
	private List<GameObject> pointers = new List<GameObject>();
	private Vector3 screenPoint;
	private GameObject[] projectiles = new GameObject[100];
	private Rigidbody[] projectilesRB = new Rigidbody[100];
	private int projectileNum = 0;

	public delegate void SoundBlastHit(Vector3 hitPos, float pitchVal, float dbVal);
	public static event SoundBlastHit onBlastHit;

	// Use this for initialization
	void Start () {
		if(cam == null) {
			cam = Camera.main;
		}
		player = FindObjectOfType<FirstPersonController>();

		Vector3 pScale = new Vector3(0.7f,0.7f,0.7f);
		for(int i = 0; i < projectiles.Length; ++i) {
			GameObject p = Instantiate(projectilePrefab);
			p.transform.parent = gameObject.transform;
			p.transform.localScale = pScale;
			p.AddComponent<AddForce>();
			p.transform.position = new Vector3(50*i, -1000, 0);
			projectilesRB[i] = p.GetComponent<Rigidbody>();
			projectilesRB[i].mass = 0.25f;
			projectilesRB[i].useGravity = false;
			projectiles[i] = p;
		}

		GameManager.isReborn += ClearProjectiles;
		InitWiimotes();
	}
	
	// Update is called once per frame
	void Update () {
		int i = 0; // Wiimote counter
		foreach(Wiimote remote in WiimoteManager.Wiimotes) {
			int ret;
			do {
				ret = remote.ReadWiimoteData();
			} while (ret > 0); // ReadWiimoteData() returns 0 when nothing is left to read.  So by doing this we continue to
								// update the Wiimote until it is "up to date."

			screenPoint = cam.WorldToScreenPoint(faces[i].transform.position);

			float[] pointer = remote.Ir.GetPointingPosition();
			// Move mask pointer
			if(pointer[0] > 0 && pointer[1] > 0) {
				// Change pointer position
				Vector3 pointScreenPoint = new Vector3(pointer[0]*Screen.width, pointer[1]*Screen.height, screenPoint.z);
				Vector3 pointPosition = cam.ScreenToWorldPoint(pointScreenPoint);
				pointers[i].transform.position = new Vector3(pointPosition.x, 8.5f, pointPosition.z);

				// Change face position
				Vector3 currentPos = faces[i].transform.position;
				Vector3 targetPos = pointers[i].transform.position;
				faces[i].transform.position = Vector3.MoveTowards(currentPos, targetPos, Vector3.Distance(currentPos, targetPos)*Time.deltaTime);
				faces[i].transform.position = new Vector3(faces[i].transform.position.x, 9, faces[i].transform.position.z);

				// Fix lookat
				pointers[i].transform.forward = cam.transform.forward; // Billboard pointer
				RaycastHit hit;
				if (Physics.Raycast(pointers[i].transform.position, pointers[i].transform.forward, out hit, 20)) {
					faces[i].transform.LookAt(hit.point);
				}

				// Shoot projectiles and make sounds
				if(remote.Button.a || remote.Button.b) {
					animators[i].SetBool("IsOpen", true);

					if(Time.time - prevSoundTimes[i] > 0.3f) {
						prevSoundTimes[i] = Time.time;

						// Create projectiles
						GameObject p = projectiles[projectileNum];
						p.transform.position = faces[i].transform.position;
						
						Rigidbody pRB = projectilesRB[projectileNum];
						pRB.useGravity = true;
						pRB.velocity = Vector3.zero;
						pRB.AddForce(faces[i].transform.forward * 256);

						projectileNum = (projectileNum+1) % projectiles.Length;

						// Create sound
						onBlastHit(faces[i].transform.position, pitches[i], audioLevel);
						// RaycastHit hit;
						// if (Physics.Raycast(faces[i].transform.position, faces[i].transform.forward, out hit, 20)) {
						// 	onBlastHit(hit.point, pitches[i], audioLevel);
						// }
					}
				} else {
					animators[i].SetBool("IsOpen", false);
				}
			}

			// Changing LEDs in a fun pattern: (*-*-) <-> (-*-*) 
			// if(blink) {
			// 	if(Time.time - prevTime > .5f) {
			// 		prevTime = Time.time;
			// 		ledCount += 1;
			// 		int a = (ledCount+1)%2;
			// 		int b = (ledCount+0)%2;
			// 		int c = (ledCount+1)%2;
			// 		int d = (ledCount+0)%2;
			// 		remote.SendPlayerLED(leds1[a], leds1[b], leds1[c], leds1[d]);
			// 	}
			// }
			++i;
		}
	}

	void OnApplicationQuit() {
		FinishedWithWiimotes();
	}

	void InitWiimotes() {
		WiimoteManager.FindWiimotes(); // Poll native bluetooth drivers to find Wiimotes

		int i = 0;
		foreach(Wiimote remote in WiimoteManager.Wiimotes) {
			SetWiimoteLeds(remote, i);
			remote.SetupIRCamera(IRDataType.BASIC); // Basic IR dot position data
			GameObject face = Instantiate(facePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			faces.Add(face);
			animators.Add(face.GetComponent<Animator>());
			pitches.Add(basePitch + i*500);
			prevSoundTimes.Add(0);

			GameObject pointer = Instantiate(pointerPrefab);

			pointer.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(i*0.15f%1, 0.7f, 1);
			pointers.Add(pointer);
			++i;
		}
	}

	void SetWiimoteLeds(Wiimote remote, int i) {
		remote.SendPlayerLED(
			ledsList[i%ledsList.Length][0],
			ledsList[i%ledsList.Length][1],
			ledsList[i%ledsList.Length][2],
			ledsList[i%ledsList.Length][3]
		);
	}

	void FinishedWithWiimotes() {
		foreach(Wiimote remote in WiimoteManager.Wiimotes) {
			WiimoteManager.Cleanup(remote);
		}
	}

	void ClearProjectiles() {
		for(int i = 0; i < projectiles.Length; ++i) {
			projectiles[i].transform.position = new Vector3(50*i, -1000, 0);
			projectilesRB[i].useGravity = false;
			projectilesRB[i].velocity = Vector3.zero;
 			projectilesRB[i].angularVelocity = Vector3.zero;
		}
	}

	void OnDestroy()
	{
		GameManager.isReborn -= ClearProjectiles;
	}
}
