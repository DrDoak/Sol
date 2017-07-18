using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Movement))]
public class NPCMovement : MonoBehaviour {

	public Player followObj;
	public float bottomOfTheWorld = -10.0f;
	Movement movement;
	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	public float jumpHeight = 4.0f;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 8.0f;

	bool targetSet = false;
	bool targetObj = false;
	Vector3 targetPoint;
	public float minDistance = 1.0f;
	public float abandonDistance = 10.0f;
	float inputX = 0.0f;
	float inputY = 0.0f;
	Animator anim;


	void Start () {
		movement = GetComponent<Movement> ();
		anim = GetComponent<Animator> ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		movement.setGravityScale(gravity * (1.0f/60f));
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}

	public void moveToPoint(Vector3 point) {
		inputX = 0.0f;
		inputY = 0.0f;

		float dist = Vector3.Distance (transform.position, point);
		if (dist > abandonDistance || dist < minDistance) {
			endTarget ();
		} else {
			if (movement.canMove) {
				if (point.x > transform.position.x) {
					if (dist > minDistance)
						inputX = 1.0f;
					movement.setFacingLeft (false);

				} else {
					if (dist > minDistance)
						inputX = -1.0f;
					movement.setFacingLeft (true);
				}
			}
		}
		float targetVelocityX = inputX * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (movement.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		Vector2 input = new Vector2 (inputX, inputY);

		if (movement.canMove && (movement.falling == "left" || movement.falling == "right") && movement.collisions.below) {
			movement.addSelfForce (new Vector2 (0f, jumpVelocity), 0f);
		}
		movement.Move (velocity, input);
		anim.SetBool ("grounded", movement.onGround);
		anim.SetBool ("tryingToMove", false);
		if (inputX != 0.0f) {
			anim.SetBool ("tryingToMove", true);
		}
	}

	void Update () {
		if (targetSet) {
			if (targetObj) {
				if (followObj == null) {
					endTarget ();
					return;
				}
				targetPoint = followObj.transform.position;
			}
			moveToPoint (targetPoint);
		}
	}
	public void orientToTarget() {
	}
		
	public void setTargetPoint(Vector3 point, float proximity) {
		setTargetPoint (point, proximity, float.MaxValue);
	}
	public void setTargetPoint(Vector3 point, float proximity,float max) {
		targetPoint = point;
		minDistance = proximity;
		abandonDistance = max;
		targetSet = true;
	}

	void setTarget(Player target) {
		targetObj = true;
		targetSet = true;
		followObj = target;
	}
	void endTarget() {
		targetSet = false;
		targetObj = false;
		followObj = null;
		minDistance = 0.2f;
	}
}
