using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {

	public float mouseSensitivityX = 3.5f;
	public float mouseSensitivityY = 3.5f;
	public float walkSpeed = 4;

	Transform cameraT;
	Rigidbody myRigidBody;
	float verticalLookRotation;

	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;

	// Use this for initialization
	void Start () {
		cameraT = Camera.main.transform;
		myRigidBody = GetComponent<Rigidbody>();
		myRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-60,60);
		cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		Vector3 targetMoveAmount = moveDir * walkSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
	}

	void FixedUpdate() {
		myRigidBody.MovePosition(myRigidBody.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}
}
