using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (BoxCollider2D))]
public class Movement : MonoBehaviour {

	public LayerMask collisionMask;

	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	public Vector2 playerInput = Vector2.zero;
	public Vector2 accumulatedVelocity = Vector2.zero;
	public bool isGravity = true;
	public float gravityScale = -1.0f;
	public float speed;
	public bool facingLeft = false;
	public bool canMove = true;
	public float terminalVelocity = -0.5f;

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

	void Start() {
		bCollider = GetComponent<BoxCollider2D> ();
		//bCollider = GetComponent<CapsuleCollider2D> ();
		sprite = GetComponent<SpriteRenderer> ();
		CalculateRaySpacing ();
		canMove = true;
		setFacingLeft (facingLeft);
	}

	public void Move(Vector2 velocity) {
		//Move (velocity, Vector2.zero);
	}

	public void setGravityScale(float gravScale) {
		gravityScale = gravScale;
	}

	void Update() {
	}

	void FixedUpdate() {
		//Debug.Log (Time.deltaTime);
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

	void HorizontalCollisions(ref Vector2 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

			if (hit && !hit.collider.isTrigger) {

				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxClimbAngle) {
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance-skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
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
		
			//Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);

			if (hit && !hit.collider.isTrigger) {
				velocity.y = (hit.distance - skinWidth) * directionY;
				/*if ((directionY > 0) && accumulatedVelocity.y > 0) {
					accumulatedVelocity.y = -(accumulatedVelocity.y * 0.7f);
				}*/
				rayLength = hit.distance;
				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
		falling = "none";
		onGround = false;
		rayLength = rayLength + 0.1f;
		if (true) { //directionY == -1) {
			bool collide = false;
			bool started = false;
			rayLength = 0.3f;
			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = raycastOrigins.bottomLeft; //true ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * -1f, rayLength, collisionMask);

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
				started = true;
			}
		}
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
	}

}
