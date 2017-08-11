using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (BoxCollider2D))]
public class Movement : MonoBehaviour {

	public LayerMask collisionMask;

	const float skinWidth = .015f;
	int horizontalRayCount = 4;
	int verticalRayCount = 4;

	public Vector2 playerInput = Vector2.zero;
	public Vector2 accumulatedVelocity = Vector2.zero;
	public bool isGravity = true;
	public float gravityScale = -1.0f;
	public float speed;
	public bool facingLeft = false;
	public bool canMove = true;
	float terminalVelocity = -0.5f;

	float maxClimbAngle = 80;

	float horizontalRaySpacing;
	float verticalRaySpacing;
	public Vector2 velocity;
	public string falling;
	BoxCollider2D bCollider;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;
	SpriteRenderer sprite;
	List<Vector2> CharForces = new List<Vector2>();
	List<float> timeForces = new List<float>();
	public bool onGround = true;
	Vector2 playerForce = new Vector2();
	public float dropThruTime = 0.0f;
	Vector2 spawnPos;
	bool resetPos = false;
	Animator anim;

	void Start() {
		bCollider = GetComponent<BoxCollider2D> ();
		sprite = GetComponent<SpriteRenderer> ();
		CalculateRaySpacing ();
		canMove = true;
		setFacingLeft (facingLeft);
		doSpawnStuff ();
		anim = GetComponent<Animator> ();
	}
	void doSpawnStuff() {
		if (resetPos) {
			if (GetComponent<ReturnToCheckpoint> ()) {
				ReturnToCheckpoint rc = GetComponent<ReturnToCheckpoint> ();
				rc.setCheckpoint (spawnPos);
				rc.resetPos ();
			} else {
				transform.position = new Vector3 (spawnPos.x, spawnPos.y, transform.position.z);
				GetComponent<Movement> ().accumulatedVelocity = Vector2.zero;
			}
			resetPos = false;
		}
	}
	public void setSpawnPos(Vector2 sp) {
		resetPos = true;
		spawnPos = sp;
	}

	public void Move(Vector2 velocity) {
		//Move (velocity, Vector2.zero);
	}

	public void setGravityScale(float gravScale) {
		gravityScale = gravScale;
	}

	void Update() {
		anim.SetBool ("grounded", onGround);
	}

	void FixedUpdate() {
		//Debug.Log (Time.deltaTime);
		dropThruTime = Mathf.Max(0f,dropThruTime - Time.fixedDeltaTime);
		if (Mathf.Abs(accumulatedVelocity.x) > 0.3f) {
			if (collisions.below) {
				accumulatedVelocity.x *= (1.0f - Time.fixedDeltaTime * 2.0f);
			} else {
				accumulatedVelocity.x *= (1.0f - Time.fixedDeltaTime * 3.0f);
			}
		} else {
			accumulatedVelocity.x = 0f;
		}
		processMovement ();
	}

	void processMovement() {
		if (!canMove) {
			playerInput = Vector2.zero;
		}
		if (collisions.above || collisions.below){
			velocity.y = 0.0f;
		}  
		playerForce = playerForce * Time.fixedDeltaTime;
		velocity.x = playerForce.x;
		bool Yf = false;

		for (int i = CharForces.Count - 1; i >= 0; i--) {
			Vector2 selfVec = CharForces [i];
			if (selfVec.y != 0f) {
				velocity.y = 0f;
				Yf = true;
			}
			if (timeForces [i] < Time.fixedDeltaTime) {
				velocity += (selfVec * Time.fixedDeltaTime);
			} else {
				velocity += (selfVec * Time.fixedDeltaTime);
			}
			timeForces [i] = timeForces [i] - Time.fixedDeltaTime;
			if (timeForces [i] < 0f) {
				CharForces.RemoveAt (i);
				timeForces.RemoveAt (i);
			}
		}
		if (velocity.y > terminalVelocity){
			if (Yf || !collisions.below) {
				velocity.y += gravityScale * Time.fixedDeltaTime;
			} else if (collisions.below) { //force player to stick to slopes
				velocity.y += gravityScale * Time.fixedDeltaTime * 6f;
			}
		}
		velocity.x += (accumulatedVelocity.x * Time.fixedDeltaTime);
		UpdateRaycastOrigins ();
		collisions.Reset ();

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		transform.Translate (velocity);
		speed = Mathf.Abs(velocity.x);
	}

	public void addToVelocity(Vector2 veloc )
	{
		accumulatedVelocity.x += veloc.x;
		addSelfForce (new Vector2 (0f, veloc.y), 0f);
	}
	public void addSelfForce(Vector2 force, float duration) {
		CharForces.Add (force);
		timeForces.Add (duration);
	}

	public void Move(Vector2 veloc, Vector2 input) {
		playerInput = input;
		playerForce = veloc;
	}
	bool handleStairs(RaycastHit2D hit,Vector2 vel) {
		if (hit.collider.gameObject.GetComponent<JumpThru> ()) {
			if (dropThruTime > 0f) {
				return true;
			}
			if (hit.collider.gameObject.GetComponent<EdgeCollider2D> ()) {
				EdgeCollider2D ec = hit.collider.gameObject.GetComponent<EdgeCollider2D> ();
				Vector2[] points = ec.points;
				Vector2 leftPoint = Vector2.zero;
				bool foundLeft = false;
				Vector2 rightPoint = Vector2.zero;
				bool foundRight = false;
				foreach (Vector2 p in points) {
					float xDiff = (ec.transform.position.x + p.x) - transform.position.x;
					if (xDiff < -0.01f) {
						if (foundRight) {
							float yDiff = p.y - rightPoint.y;
							if (yDiff > 0.01f) {
								if (vel.x > 0f) {
									return true;
								} else {
									return false;
								}
							} else if (yDiff < -0.01f) {
								if (vel.x > 0f) {
									return false;
								} else {
									return true;
								}
							}
						} else {
							if (Vector2.Equals(Vector2.zero,leftPoint)) {
								leftPoint = p;
								foundLeft = true;
							}
						}
					} else if (xDiff > 0.01f) {
						if (foundLeft) {
							float yDiff = p.y - leftPoint.y;
							if (yDiff > 0.01f) {
								if (vel.x < 0f) {
									return true;
								} else {
									return false;
								}
							} else if (yDiff < -0.01f) {
								if (vel.x < 0f) {
									return false;
								} else {
									return true;
								}
							}
						} else {
							if (Vector2.Equals(Vector2.zero,rightPoint)) {
								rightPoint = p;
								foundRight = true;
							}
						}
					}
				}
			} else {
				return true;
			}
		}
		return false;
	}
	void HorizontalCollisions(ref Vector2 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			if (hit) {
				Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);
			} else {
				Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.green);
			}

			if (hit && !hit.collider.isTrigger) {

				if (handleStairs(hit,velocity) ){
					
				} else {
					float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

					if (i == 0 && slopeAngle <= maxClimbAngle) {
						float distanceToSlopeStart = 0;
						if (slopeAngle != collisions.slopeAngleOld) {
							distanceToSlopeStart = hit.distance - skinWidth;
							velocity.x -= distanceToSlopeStart * directionX;
						}
						ClimbSlope (ref velocity, slopeAngle);
						velocity.x += distanceToSlopeStart * directionX;
					}

					if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
						velocity.x = (hit.distance - skinWidth) * directionX;
						rayLength = hit.distance;

						if (collisions.climbingSlope) {
							velocity.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
						}

						collisions.left = directionX == -1;
						collisions.right = directionX == 1;
					}
				}
			}
		}

	}
	
	void VerticalCollisions(ref Vector2 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			if (hit && !hit.collider.isTrigger) {
				if (hit.collider.gameObject.GetComponent<JumpThru> () && ( velocity.y > 0 || dropThruTime > 0f)){ //|| handleStairs(hit,velocity))){
				} else {
					velocity.y = (hit.distance - skinWidth) * directionY;
					rayLength = hit.distance;
					if (collisions.climbingSlope) {
						velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
					}

					collisions.below = directionY == -1;
					collisions.above = directionY == 1;
				}
			}
		}
		falling = "none";
		onGround = false;
		rayLength = rayLength + 0.1f;
		if (true) {
			bool collide = false;
			bool started = false;
			rayLength = 0.3f;
			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = raycastOrigins.bottomLeft; //true ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * -1f, rayLength, collisionMask);
				if (hit && hit.collider.gameObject.GetComponent<JumpThru> () && (dropThruTime > 0f )) {
				} else {
					if (hit && !hit.collider.isTrigger) {
						Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);
						onGround = true;
						if ( started && !collide) {
							falling = "left";
						}
						collide = true;
					}  else {
						Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.green);
						if (started && collide) {
							falling = "right";
						}
					}
				}
				started = true;
			}
		}
	}
	public void setDropTime(float tm) {
		dropThruTime = tm;
	}
	void ClimbSlope(ref Vector2 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void UpdateRaycastOrigins() {
		Bounds bounds = bCollider.bounds;
	
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x , bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing() {
		Bounds bounds = bCollider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public float slopeAngle, slopeAngleOld;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
	public void setFacingLeft(bool left) {
		facingLeft = left;
		if (sprite) {
			if (facingLeft) {
				sprite.flipX = true;
			} else {
				sprite.flipX = false;
			}
		}
		if (GetComponent<Character> ()) {
			GetComponent<Character> ().facingLeft = facingLeft;
		}
	}

}
