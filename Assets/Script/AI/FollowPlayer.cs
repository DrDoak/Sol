using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Movement))]
public class FollowPlayer : MonoBehaviour {

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
	public bool targetSet = true;
	public float minDistance = 1.0f;
	public float maxDistance = 10.0f;
	float inputX = 0.0f;
	float inputY = 0.0f;
	Animator anim;


	void Start () {
		movement = GetComponent<Movement> ();
		anim = GetComponent<Animator> ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		movement.setGravityScale(gravity * (1.0f/60f));
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		setTarget(FindObjectOfType<Player> ());
	}

	void setTarget(Player target) {
		targetSet = true;
		followObj = target;
	}

	public void moveToPlayer() {
		inputX = 0.0f;
		inputY = 0.0f;

		if (followObj == null)
			return;
		float dist = Vector3.Distance (transform.position, followObj.transform.position);
		if (movement.canMove && dist < maxDistance) {

			if (followObj.transform.position.x > transform.position.x) {
				if ( dist > minDistance)
					inputX = 1.0f;
				movement.setFacingLeft (false);

			} else {
				if ( dist > minDistance)
					inputX = -1.0f;
				movement.setFacingLeft (true);
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

	// Update is called once per frame
	void Update () {
		moveToPlayer ();
	}
}
