using UnityEngine;
using System.Collections;
using WiimoteApi;

public class WiiMoteController : MonoBehaviour {
	 bool[] leds = new bool[] {true, false, false, false};
	 int count = 0;
	 bool blink = false;
	 float prevTime = 0;
	 public Camera cam;

	 private Vector3 screenPoint;

	// Use this for initialization
	void Start () {
		if(cam == null) {
			cam = Camera.main;
		}
		InitWiimotes();
	}
	
	// Update is called once per frame
	void Update () {
		foreach(Wiimote remote in WiimoteManager.Wiimotes) {
			int ret;
			do {
				ret = remote.ReadWiimoteData();
			} while (ret > 0); // ReadWiimoteData() returns 0 when nothing is left to read.  So by doing this we continue to
									// update the Wiimote until it is "up to date."

			screenPoint = cam.WorldToScreenPoint(transform.position);

			float[] pointer = remote.Ir.GetPointingPosition();
			if(pointer[0] > 0 && pointer[1] > 0) {
				Vector3 curScreenPoint = new Vector3(pointer[0]*Screen.width, pointer[1]*Screen.height, screenPoint.z);
				Vector3 curPosition = cam.ScreenToWorldPoint(curScreenPoint);
				transform.position = curPosition;



				// float distance = transform.position.y - cam.transform.position.y;
				// Vector3 pos = new Vector3(pointer[0]*Screen.width, distance, pointer[1]*Screen.height);
				// transform.position = cam.ScreenToWorldPoint(pos);




				// transform.position = new Vector3(
				// 	Mathf.Clamp(transform.position.x + (pointer[0]-.5f), -5, 5),
				// 	transform.position.y,
				// 	Mathf.Clamp(transform.position.z + (pointer[1]-.5f), -5, 5)
				// 	);
			}
		}

		if(blink) {
			if(Time.time - prevTime > .5f) {
				prevTime = Time.time;
				count += 1;
				foreach(Wiimote remote in WiimoteManager.Wiimotes) {
					int a = (count+1)%2;
					int b = (count+0)%2;
					int c = (count+1)%2;
					int d = (count+0)%2;
					remote.SendPlayerLED(leds[a], leds[b], leds[c], leds[d]);
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
		foreach(Wiimote remote in WiimoteManager.Wiimotes) {
			remote.SendPlayerLED(true, false, false, false);
			remote.SetupIRCamera(IRDataType.BASIC); // Basic IR dot position data
		}
	}
	void FinishedWithWiimotes() {
		foreach(Wiimote remote in WiimoteManager.Wiimotes) {
			WiimoteManager.Cleanup(remote);
		}
	}
}
