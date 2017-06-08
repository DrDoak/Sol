using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

	// Use this for initialization
	List<Attackable> collidedObjs = new List<Attackable> (); 
	List<float> collidedTimes = new List<float>();
	public bool fixedKnockback = true;
	public Vector2 knockback = new Vector2(0.0f,60.0f);
	public float damage = 0.0f;

	void Update() {
		for (int i = collidedObjs.Count - 1; i >= 0; i--) {
			collidedTimes [i] = collidedTimes [i] - Time.deltaTime;
			if (collidedTimes [i] < 0f) {
				collidedObjs.RemoveAt (i);
				collidedTimes.RemoveAt (i);
			}
		}
	}

	internal void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<Attackable>() &&
			!collidedObjs.Contains (other.gameObject.GetComponent<Attackable> ())) {
			Attackable otherObj = other.gameObject.GetComponent<Attackable> ();
			float counterF = (other.gameObject.GetComponent<Movement> ().velocity.y * (1/Time.deltaTime));
			if (fixedKnockback) {

				//Debug.Log (counterF);
				if (counterF > 0.0f) {
					counterF = 0.0f;
				}
				counterF = 0.0f;
			//	Debug.Log (counterF);
				otherObj.addToVelocity (new Vector2(knockback.x, knockback.y - counterF));
			} else {
				Vector3 otherPos = other.gameObject.transform.position;
				float angle = Mathf.Atan2 (transform.position.y - otherPos.y, transform.position.x - otherPos.x); //*180.0f / Mathf.PI;
				float magnitude = knockback.magnitude;
				float forceX = Mathf.Cos (angle) * magnitude;
				float forceY = Mathf.Sin (angle) * magnitude;
				Vector2 force = new Vector2 (-forceX, -forceY);
				if (counterF < 0) {
					force.y = force.y - counterF;
				}
				otherObj.addToVelocity (force);
			}
			collidedObjs.Add (otherObj);
			collidedTimes.Add (0.2f);
		}
	}
}
