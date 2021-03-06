﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

//Governs player input
[RequireComponent (typeof (PersItem))]
[RequireComponent (typeof (Movement))]
[RequireComponent (typeof (Fighter))]
[RequireComponent (typeof (Attackable))]
[RequireComponent (typeof (ReturnToCheckpoint))]
public class Playable : MonoBehaviour {

	// Movement 
	public bool IsCurrentPlayer = false;
	public Vector2 startPosition = new Vector2 (-4.0f, -3f);
	public float jumpHeight = 4.0f;
	public float timeToJumpApex = .4f;

	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 8.0f;
	public bool CharacterSelect = false;

	float gravity;
	float jumpVelocity;
	Vector2 velocity;
	Vector2 jumpVector;
	float velocityXSmoothing;
	//-------------------
	bool attemptingInteraction = false;
	Movement movement;
	Attackable attackable;
	AnimatorSprite m_anim;
	GameManager gameManager;

	public bool canDoubleJump = true;

	float inputX = 0.0f;
	float inputY = 0.0f;
	//bool isJump;
	float jumpPersist = 0.0f;
	float timeSinceLastDash = 0.0f;

	string[] AirDown = new string[] {"air_down","down", "air","attack"};
	string[] AirSide = new string[] {"air_side","side", "air","attack"};
	string[] AirUp = new string[] {"air_up","up", "air","attack"};

	public bool autonomy = true;

	bool targetSet = false;
	bool targetObj = false;
	Vector3 targetPoint;
	public float minDistance = 1.0f;
	public float abandonDistance = 10.0f;
	public Playable followObj;

	internal void Start()  {
		m_anim = GetComponent<AnimatorSprite> ();
		movement = GetComponent<Movement> ();
		attackable = GetComponent<Attackable> ();
		Reset ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		movement.setGravityScale (gravity * (1.0f/60f));
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		jumpVector = new Vector2 (0f, jumpVelocity);
		gameManager = FindObjectOfType<GameManager> ();
		if (CharacterSelect == true) {
			GameManager.StartCharacterSelect ();
		}
	}

	public void Reset() {
		//GetComponent<ReturnToCheckpoint>().resetPos();
		attackable.energy = 0.0f;
		attackable.alive = true;
	}
	public void onHitConfirm(GameObject otherObj) {
		Fighter mF = otherObj.GetComponent<Fighter> ();
		if (mF && GetComponent<Fighter>().currentAttackName != "super") {
			float increase = 3.0f;
			increase += (3f * Mathf.Min(10f,mF.hitCombo));
			if (!movement.onGround) {
				increase *= 1.5f;
			}
			GameObject.FindGameObjectWithTag ("ComboCount").GetComponent<TextMeshProUGUI> ().text = "Last Combo: " + mF.hitCombo.ToString ();
			attackable.modifyEnergy (increase);
		}
	}
	void Update() {
		if (IsCurrentPlayer) {
			playerMovement ();
		} else {
			npcMovement ();
		}
	}

	internal void playerMovement() {
		if (movement.onGround) {canDoubleJump = true;}
		inputX = 0.0f;
		inputY = 0.0f;
		if (!autonomy && movement.canMove && targetSet) {
			if (targetObj) {
				if (followObj == null) {
					endTarget ();
					return;
				}
				targetPoint = followObj.transform.position;
			}
			moveToPoint (targetPoint);
		}else if (movement.canMove && autonomy) {
			
			if (Input.GetButtonDown("Menu")) {
				GameManager.StartCharacterSelect ();
				//gameManager.toggleMenu ();
			}
			inputY = Input.GetAxis ("Vertical");

			//Movement controls
			inputX = Input.GetAxis("Horizontal");
			attackInput (inputX, inputY);

			if (Input.GetButtonDown("Interact")) {
				GetComponent<Character> ().playerInteraction ();
			}
							
			if (Input.GetButtonDown("Jump")) {
				if (inputY < -0.9f) {
					GetComponent<Movement>().setDropTime(1.0f);
				}
				else if (movement.collisions.below) {
					//velocity.y = jumpVelocity;
					//controller.velocity.y = jumpVelocity * Time.deltaTime;
					movement.addSelfForce (jumpVector, 0f);
					jumpPersist = 0.2f;
					//gameManager.soundfx.gameObject.transform.Find ("P1Jump").GetComponent<AudioSource> ().Play ();
					//isJump = true;
				} else if (canDoubleJump) {
					velocity.y = jumpVelocity;
					//isJump = false;
					movement.addSelfForce (jumpVector, 0f);
					//gameManager.soundfx.gameObject.transform.Find ("P1Jump").GetComponent<AudioSource> ().Play ();
					canDoubleJump = false;
				}
			}
			/*if (Input.GetKey (jumpKey) && isJump && controller.velocity.y > 0f) {
				controller.setGravityScale (gravity * 0.8f);
			}*/
		}
		//Movement logic
		float targetVelocityX = inputX * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (movement.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		Vector2 input = new Vector2 (inputX, inputY);
		movement.Move (velocity, input);
		if (!attackable.alive)
			Reset ();
		movement.AttemptingMovement = (inputX != 0.0f);
	}

	void npcMovement () {
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
		movement.AttemptingMovement = (inputX != 0.0f);
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

	void setTarget(Playable target) {
		targetObj = true;
		targetSet = true;
		followObj = target;
	}
	public void endTarget() {
		targetSet = false;
		targetObj = false;
		followObj = null;
		minDistance = 0.2f;
	}
	void attackInput(float inputX, float inputY) {
		if (Input.GetButtonDown ("Sheath")) {
			gameObject.GetComponent<Fighter> ().ToggleSheath ();
		}
		if (Input.GetButtonDown("Attack")) {
			if (inputY < -0.9f) {
				if (movement.onGround) {
					gameObject.GetComponent<Fighter> ().tryAttack ("down");
					//attackable.modifyEnergy (100f);
				} else {
					gameObject.GetComponent<Fighter> ().tryAttack (AirDown);
				}
			} else if (inputY > 0.9f) {
				if (movement.onGround) {
					gameObject.GetComponent<Fighter> ().tryAttack ("up");
				} else {
					gameObject.GetComponent<Fighter> ().tryAttack (AirUp);
				}
			}else if (Mathf.Abs(inputX) > 0.9f) {
				if (movement.onGround) {
					gameObject.GetComponent<Fighter> ().tryAttack ("side");
				} else {
					gameObject.GetComponent<Fighter> ().tryAttack (AirSide);
				}
			} else {
				gameObject.GetComponent<Fighter> ().tryAttack ("attack");
			}
		}
		if (Input.GetButtonDown("Super")) {
			if (attackable.energy >= 100.0f){
				gameObject.GetComponent<Fighter> ().tryAttack ("super");
			}
			//FindObjectOfType<KNManager> ().createList (GetComponent<Character> ());
		}
		if (Input.GetButtonDown("Special")) {
			if (inputY < -0.9f ) {
				gameObject.GetComponent<Fighter> ().tryAttack ("spc_down");
			} else {
				gameObject.GetComponent<Fighter> ().tryAttack ("spc");
			}
		}
	}
}