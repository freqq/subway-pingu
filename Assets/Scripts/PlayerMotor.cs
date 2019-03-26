using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour {
	private const float LANE_DISTANCE = 2.5f;
	private const float TURN_SPEED = 0.5f;

	//
	private bool isRunning = false;

	//animation
	private Animator anim;

	//movement
	private CharacterController controller;
	private int desiredLine = 1; // 0 - letf, 1 - middle, 2 right
	private float jumpForce = 5.0f;
	private float gravity = 12.0f;
	private float verticalVelocity;

	//speed modifier
	private float originalSpeed = 7.0f;
	private float speed;
	private float speedInscreaseLastTick;
	private float speedInscreaseTime = 2.5f;
	private float speedInscreaseAmount = 0.1f;


	private void Start(){
		speed = originalSpeed;
		controller = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
	}

	private void Update(){
		if (!isRunning)
			return;

		if (Time.time - speedInscreaseLastTick > speedInscreaseTime) {
			speedInscreaseLastTick = Time.time;
			speed += speedInscreaseAmount;

			ManagerGame.Instance.updateModifier (speed - originalSpeed);
		}

		//gather the inputs on which lane we should be
		if (MobileInput.Instance.SwipeLeft)
			MoveLane (false);
		if (MobileInput.Instance.SwipeRight)
			MoveLane (true);

		//calculate where we should be in the future
		Vector3 targetPosition = transform.position.z * Vector3.forward;
		if (desiredLine == 0)
			targetPosition += Vector3.left * LANE_DISTANCE;
		else if (desiredLine == 2)
			targetPosition += Vector3.right * LANE_DISTANCE;

		//lets calcualte our move delta
		Vector3 moveVector = Vector3.zero;
		moveVector.x = (targetPosition - transform.position).normalized.x * speed;

		bool IsGrounded = isGrounded ();

		//calculate y
		if (IsGrounded) {
			verticalVelocity = -0.1f;
			anim.SetBool ("Grounded", IsGrounded);
			if (MobileInput.Instance.SwipeUp) {
				//jump
				anim.SetTrigger ("Jump");
				verticalVelocity = jumpForce;
			} else if (MobileInput.Instance.SwipeDown) {
				//slide
				StartSliding();
				Invoke ("StopSliding", 1.0f);
			}
		} else {
			verticalVelocity -= (gravity * Time.deltaTime);

			//fast falling mechanic
			if (MobileInput.Instance.SwipeDown) {
				verticalVelocity = -jumpForce;
			}
		}

		moveVector.y = verticalVelocity;
		moveVector.z = speed;

		//move the pengu
		controller.Move(moveVector * Time.deltaTime);

		//rotate the pingu to where is he going
		Vector3 dir = controller.velocity;
		if (dir != Vector3.zero) {
			dir.y = 0;
			transform.forward = Vector3.Lerp (transform.forward, dir, TURN_SPEED);
		}
	}

	private void MoveLane(bool goingRight){
		desiredLine += (goingRight) ? 1 : -1;
		desiredLine = Mathf.Clamp (desiredLine, 0, 2);

	}

	private bool isGrounded(){
		Ray groundRay = new Ray (
			                new Vector3 (
				                controller.bounds.center.x,
				                (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f,
				                controller.bounds.center.z),
			                Vector3.down);
		Debug.DrawRay (groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

		return (Physics.Raycast (groundRay, 0.2f + 0.1f));
	}

	public void StartGame(){
		isRunning = true;
		anim.SetTrigger ("StartRunning");
	}

	private void StartSliding(){
		anim.SetBool ("Sliding", true);
		controller.height /= 2;
		controller.center = new Vector3 (controller.center.x, controller.center.y / 2, controller.center.z);
	}

	private void StopSliding(){
		anim.SetBool ("Sliding", false);
		controller.height *= 2;
		controller.center = new Vector3 (controller.center.x, controller.center.y * 2, controller.center.z);

	}

	private void Crash(){
		anim.SetTrigger ("Death");
		isRunning = false;
        ManagerGame.Instance.OnDeath();
	}

	private void OnControllerColliderHit(ControllerColliderHit hit){
		switch (hit.gameObject.tag) {
			case "Obstacle":
				Crash ();
				break;						
		}
	}
}
