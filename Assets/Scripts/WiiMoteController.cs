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

	int ledCount = 0;
	bool blink = false;
	float prevTime = 0;
	List<float> prevSoundTimes = new List<float>();
	List<GameObject> faces = new List<GameObject>();
	List<float> pitches = new List<float>();

	public float basePitch = 200;
	public float audioLevel = 0.5f;
	public Camera cam;

	private Vector3 screenPoint;

	public delegate void SoundBlastHit(Vector3 hitPos, float pitchVal, float dbVal);
	public static event SoundBlastHit onBlastHit;

	// Use this for initialization
	void Start () {
		if(cam == null) {
			cam = Camera.main;
		}
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
			if(pointer[0] > 0 && pointer[1] > 0) {
				Vector3 curScreenPoint = new Vector3(pointer[0]*Screen.width, pointer[1]*Screen.height, screenPoint.z);
				Vector3 curPosition = cam.ScreenToWorldPoint(curScreenPoint);
				faces[i].transform.position = new Vector3(curPosition.x, 5f, curPosition.z);



				// float distance = transform.position.y - cam.transform.position.y;
				// Vector3 pos = new Vector3(pointer[0]*Screen.width, distance, pointer[1]*Screen.height);
				// transform.position = cam.ScreenToWorldPoint(pos);




				// transform.position = new Vector3(
				// 	Mathf.Clamp(transform.position.x + (pointer[0]-.5f), -5, 5),
				// 	transform.position.y,
				// 	Mathf.Clamp(transform.position.z + (pointer[1]-.5f), -5, 5)
				// 	);

				if(Time.time - prevSoundTimes[i] > 0.5f && remote.Button.a) {
					prevSoundTimes[i] = Time.time;

					// Create spheres
					// GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					// sphere.transform.position = transform.position;
					// sphere.transform.localScale = new Vector3(0.7f,0.7f,0.7f);
					// Rigidbody sphereRB = sphere.AddComponent<Rigidbody>();
					// sphereRB.mass = 15;

					RaycastHit hit;
					if (Physics.Raycast(faces[i].transform.position, Vector3.down, out hit, 20)) {
						onBlastHit(hit.point, pitches[i], audioLevel);
					}
				}
			}
			++i;
		}

		// Changing LEDs in a fun pattern: (*-*-) <-> (-*-*) 
		if(blink) {
			if(Time.time - prevTime > .5f) {
				prevTime = Time.time;
				ledCount += 1;
				foreach(Wiimote remote in WiimoteManager.Wiimotes) {
					int a = (ledCount+1)%2;
					int b = (ledCount+0)%2;
					int c = (ledCount+1)%2;
					int d = (ledCount+0)%2;
					remote.SendPlayerLED(leds1[a], leds1[b], leds1[c], leds1[d]);
				}
			} else {

			}
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
			prevSoundTimes.Add(0);
			faces.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
			pitches.Add(basePitch + i*500);

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
}
