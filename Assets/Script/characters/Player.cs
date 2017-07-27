using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (PersItem))]
[RequireComponent (typeof (Movement))]
[RequireComponent (typeof (Fighter))]
[RequireComponent (typeof (Attackable))]
[RequireComponent (typeof (ReturnToCheckpoint))]
public class Player : MonoBehaviour {

	// Movement 
	public Vector2 startPosition = new Vector2 (-4.0f, -3f);
	public float jumpHeight = 4.0f;
	public float timeToJumpApex = .4f;
	public string playerName = "Player 1";
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 8.0f;

	float gravity;
	float jumpVelocity;
	Vector2 velocity;
	Vector2 jumpVector;
	float velocityXSmoothing;
	//-------------------
	bool attemptingInteraction = false;
	Movement controller;
	Attackable attackable;
	Animator anim;
	GameManager gameManager;

	public bool canDoubleJump = true;

	float inputX = 0.0f;
	float inputY = 0.0f;
	bool isJump;
	float jumpPersist = 0.0f;
	float timeSinceLastDash = 0.0f;

	public float mistimedKBRatio = 1.0f;
	public float mistimedDamageRatio = 1.0f;
	public float mistimedStunRatio = 1.0f;


	public AudioClip Slash;
	public AudioClip DelayedSlash;
	public AudioClip ShortDelayedSlash;
	public AudioClip MultiSlash;
	public AudioClip FailedReflect;
	public AudioClip SuccessfulReflect;
	public bool autonomy = true;

	internal void Start()  {
		//Object.DontDestroyOnLoad (this);
		anim = GetComponent<Animator> ();
		controller = GetComponent<Movement> ();
		attackable = GetComponent<Attackable> ();
		Reset ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		controller.setGravityScale (gravity * (1.0f/60f));
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		jumpVector = new Vector2 (0f, jumpVelocity);
		gameManager = FindObjectOfType<GameManager> ();
	}

	public void Reset() {
		//GetComponent<ReturnToCheckpoint>().resetPos();
		attackable.resetHealth ();
		attackable.energy = 0.0f;
		attackable.alive = true;
	}
	public void onHitConfirm(GameObject otherObj) {
		Fighter mF = otherObj.GetComponent<Fighter> ();
		if (GetComponent<Fighter>().currentAttackName != "super") {
			float increase = 3.0f;
			increase += (3f * Mathf.Min(10f,mF.hitCombo));
			if (!controller.onGround) {
				increase *= 1.5f;
			}
			GameObject.FindGameObjectWithTag ("ComboCount").GetComponent<Text> ().text = "Last Combo: " + mF.hitCombo.ToString ();
			attackable.modifyEnergy (increase);
		}
	}

	internal void Update() {
		timeSinceLastDash += Time.deltaTime;
		/*
		if (lastHealth > GetComponent<Attackable> ().health) {
					Debug.Log ("Reset");
			FindObjectOfType<PlayerCursor> ().timeSinceLastHit = 0.0f;
		}
		lastHealth = GetComponent<Attackable> ().health;*/

		if (controller.onGround) {canDoubleJump = true;}
		inputX = 0.0f;
		inputY = 0.0f;

		if (controller.canMove && autonomy) {
			inputY = Input.GetAxis ("Vertical");

			/*if (Input.GetKeyDown (downKey)) {
				attemptingInteraction = true;
			} else {
				attemptingInteraction = false;
			}*/
			//Movement controls
			inputX = Input.GetAxis("Horizontal");
			if (inputX < 0.0f) { 
				anim.SetBool ("tryingToMove", true);
				controller.setFacingLeft (true);
			} else if (inputX > 0.0f) { 
				anim.SetBool ("tryingToMove", true);
				controller.setFacingLeft (false);
			}
			//Attack/Reflect/Guard Animations
			if (Input.GetButtonDown("Attack")) {
				
				if (inputY < -0.9f) {
					if (controller.onGround) {
						gameObject.GetComponent<Fighter> ().tryAttack ("down");
						AudioSource.PlayClipAtPoint (DelayedSlash, gameObject.transform.position);
						//attackable.modifyEnergy (100f);
					} else {
						gameObject.GetComponent<Fighter> ().tryAttack ("airdown");
						AudioSource.PlayClipAtPoint (ShortDelayedSlash, gameObject.transform.position);
					}
				} else if (inputY > 0.9f) {
					if (controller.onGround) {
						gameObject.GetComponent<Fighter> ().tryAttack ("up");
						AudioSource.PlayClipAtPoint (ShortDelayedSlash, gameObject.transform.position);
					} else {
						gameObject.GetComponent<Fighter> ().tryAttack ("airup");
						AudioSource.PlayClipAtPoint (MultiSlash, gameObject.transform.position);
					}
				}else if (Mathf.Abs(inputX) > 0.9f) {
					AudioSource.PlayClipAtPoint (Slash, gameObject.transform.position);
					gameObject.GetComponent<Fighter> ().tryAttack ("dash");
				} else {
					AudioSource.PlayClipAtPoint (Slash, gameObject.transform.position);
					gameObject.GetComponent<Fighter> ().tryAttack ("attack");
				}
			}
			if (Input.GetButtonDown("Super")) {
				if (attackable.energy >= 100.0f){
					gameObject.GetComponent<Fighter> ().tryAttack ("super");
					AudioSource.PlayClipAtPoint (MultiSlash, gameObject.transform.position);
				}
			}
			if (Input.GetButtonDown("Special")) {
				if (inputY < -0.9f ) {
					gameObject.GetComponent<Fighter> ().tryAttack ("reflect");
					AudioSource.PlayClipAtPoint (FailedReflect, gameObject.transform.position);
				} else {
					gameObject.GetComponent<Fighter> ().tryAttack ("guard");
				}
			}
			if (Input.GetButtonDown("Interact")) {
				Debug.Log ("Attempting interaction from player");
				GetComponent<Character> ().playerInteraction ();
			}

				
			if (Input.GetButtonDown("Jump")) {
				if (inputY < -0.9f) {
					GetComponent<Movement>().setDropTime(1.0f);
				}
				else if (controller.collisions.below) {
					//velocity.y = jumpVelocity;
					//controller.velocity.y = jumpVelocity * Time.deltaTime;
					controller.addSelfForce (jumpVector, 0f);
					jumpPersist = 0.2f;
					//gameManager.soundfx.gameObject.transform.Find ("P1Jump").GetComponent<AudioSource> ().Play ();
					isJump = true;
				} else if (canDoubleJump) {
					velocity.y = jumpVelocity;
					isJump = false;
					controller.addSelfForce (jumpVector, 0f);
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
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		Vector2 input = new Vector2 (inputX, inputY);
		controller.Move (velocity, input);
		if (!attackable.alive) {
			Reset ();
		}
		anim.SetBool ("grounded", controller.onGround);
		anim.SetBool ("tryingToMove", false);
		if (inputX != 0.0f) {
			anim.SetBool ("tryingToMove", true);
		}		
	}
}
