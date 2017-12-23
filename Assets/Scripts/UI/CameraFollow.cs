using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Movement target;
	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	public Vector2 focusAreaSize;
	public bool camConstrained;
	public Vector2 minVertex;
	public Vector2 maxVertex;
	Vector2 viewSize;

	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;

	bool lookAheadStopped;

	void Start() {
		initFunct ();
	}
	public void initFunct() {
		if (target != null) {
			viewSize.y = GetComponent<Camera> ().orthographicSize * 2f;
			viewSize.x = viewSize.y * GetComponent<Camera> ().aspect;
			focusArea = new FocusArea (target.GetComponent<Collider2D> ().bounds, focusAreaSize,viewSize);

		}
	}
	void Update() {
		if (target != null) {
			focusArea.Update (target.GetComponent<Collider2D> ().bounds,minVertex,maxVertex,camConstrained);
		}
		Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;


		if (focusArea.velocity.x != 0) {
			lookAheadDirX = Mathf.Sign (focusArea.velocity.x);
			if (Mathf.Sign(target.SelfInput.x) == Mathf.Sign(focusArea.velocity.x) && target.SelfInput.x != 0) {
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDstX;
			}
			else {
				if (!lookAheadStopped) {
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX)/4f;
				}
			}
		}

		currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

		focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;
		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos() {
		/*Gizmos.color = new Color (0, 0, 1, .1f);
		Gizmos.DrawCube (focusArea.centre, focusAreaSize);
		Gizmos.color = new Color (0, 1, 0, .4f);
		Gizmos.DrawCube (focusArea.centre, viewSize);*/

	}

	struct FocusArea {
		public Vector2 centre;
		public Vector2 velocity;
		float left,right;
		float top,bottom;
		Vector2 focusSize;

		Vector2 camSize;


		public FocusArea(Bounds targetBounds, Vector2 size,Vector2 largeCam) {
			focusSize = size;
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left+right)/2,(top + bottom)/2);
			camSize = largeCam;
		}

		public void Update(Bounds targetBounds,Vector2 minVertex, Vector2 maxVertex,bool camConstrained) {
			float shiftX = 0;

			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			bool shift = true;
			if (camConstrained) {
				float extra = camSize.x;
				if (left - (extra/2f) < minVertex.x && shiftX < 0f) {
					shift = false;
				}
				if (right + (extra/2f) > maxVertex.x && shiftX > 0f) {
					shift = false;
				}
			}
			if (shift) {
				left += shiftX;
				right += shiftX;
			}

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			shift = true;
			//Debug.Log (top);
			if (camConstrained) {
				float extra = camSize.y;
				//Debug.Log (extra);
				if (bottom - (extra/2f) < minVertex.y && shiftY < 0f) {
					shift = false;
				}
				if (top + (extra/2f) > maxVertex.y && shiftY > 0f) {
					shift = false;
				}
			}
			if (shift) {
				bottom += shiftY;
				top += shiftY;
			}
			centre = new Vector2((left+right)/2,(top +bottom)/2);
			velocity = new Vector2 (shiftX, shiftY);
		}
	}

}
