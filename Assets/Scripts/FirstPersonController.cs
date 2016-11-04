using UnityEngine;
using System.Collections;
using System;
using UnityEngine.VR;

public class FirstPersonController : MonoBehaviour {

	public event Action OnGoalTouch;
	
	public enum ControllMode {FPSControlls, CarControlls};
	public ControllMode inputMode;
	public float mouseSensitivityX = 3.5f;
	public float mouseSensitivityY = 3.5f;
	public float walkSpeed = 0.5f;
	public float turnSpeed = 2f;
	public GameObject goal;

	public bool freezeMovement = false;
	public bool lockUpDownMovement = true;
	
	//Transform cameraT; // never used
	//Camera cam; // Never used
	private Rigidbody myRigidBody;
	private float verticalLookRotation;
	private Vector3 moveAmount;
	private Vector3 smoothMoveVelocity;
    private Animator anim;

	// Use this for initialization
	void Start () {
		//cam = Camera.main;
		//cameraT = Camera.main.transform;
		myRigidBody = GetComponent<Rigidbody>();
		myRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        anim = GetComponent<Animator>();

		VRSettings.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		RaycastHit hit;
		Vector3 fwd = cameraT.TransformDirection(Vector3.forward);
        
        if (Physics.Raycast(transform.position, fwd, out hit)) {
            cam.farClipPlane = Mathf.MoveTowards(cam.farClipPlane,hit.distance*2f, Time.deltaTime*5f);
            
            // Do something with the object that was hit by the raycast.
        }
		/*
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-60,60);
		transform.Rotate(Vector3.right * Input.GetAxis("Mouse Y")* mouseSensitivityY);
		cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		Vector3 targetMoveAmount = moveDir * walkSpeed;
		
		*/


		float h = 0, vH = 0, vV = 0;

		if(inputMode == ControllMode.FPSControlls)
		{
			h = Input.GetAxis("RJoystickHorizontal") * Time.fixedDeltaTime * turnSpeed;
			vH = Input.GetAxis("LJoystickHorizontal") * Time.fixedDeltaTime; // Positiv joystick ner??
			vV = Input.GetAxis("LJoystickVertical") * Time.fixedDeltaTime;
		}
		else if(inputMode == ControllMode.CarControlls)
		{
			h = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * turnSpeed;
			vV = Input.GetAxis("Accelerate") * Time.fixedDeltaTime;
			vH = 0;
		}

		if (!freezeMovement) {
			transform.Rotate (h * Vector3.up);
		}

		//transform.Rotate (v * Vector3.right);
		//myRigidBody.AddRelativeTorque (h * Vector3.back );
		//myRigidBody.AddRelativeTorque (v * Vector3.right);

		Vector3 targetMoveAmount = ((vV * Vector3.back) + (vH * Vector3.right)) * walkSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
	}

	void FixedUpdate() {
		
		if(!freezeMovement)
		{
            //myRigidBody.AddRelativeForce (moveAmount, ForceMode.Impulse);
            if (anim != null) {
                anim.SetBool("walking", (moveAmount.magnitude > 0.001f));
            }

			// Move Player
			myRigidBody.MovePosition(myRigidBody.position + transform.TransformDirection(moveAmount));
			
			if (Input.GetButton ("Fire1") && !lockUpDownMovement) {
				myRigidBody.AddRelativeForce (Vector3.up * 5f);
			}

			if (Input.GetButton ("Fire2") && !lockUpDownMovement) {
				myRigidBody.AddRelativeForce (Vector3.up * -5f);
			}
		}
	}

	void OnTriggerEnter(Collider triggerCollider) {
		//print (triggerCollider.gameObject.name);
		if (triggerCollider.gameObject == goal) {
			OnGoalTouch ();
		}
	}
		
}
