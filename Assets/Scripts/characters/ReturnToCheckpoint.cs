using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToCheckpoint : MonoBehaviour {

	public Vector2 lastCheckpoint = Vector2.zero;
	bool foundCheckpoint;
	// Use this for initialization
	void Start () {
		if (!foundCheckpoint) {
			setCheckpoint (transform.position);
		}
	}
	void Update () {}

	public void setCheckpoint(Vector2 newPt) {
		foundCheckpoint = true;
		lastCheckpoint = newPt;
	}
	public void setCheckpoint(Vector3 newPt) {
		foundCheckpoint = true;
		lastCheckpoint = new Vector2 (newPt.x, newPt.y);
	}
	public void resetPos() {
		if (foundCheckpoint) {
			transform.position = lastCheckpoint;
			GetComponent<Movement>().accumulatedVelocity = Vector2.zero;
		}
	}
}
