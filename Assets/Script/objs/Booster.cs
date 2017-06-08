using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour {
	public bool fixedKnockback = true;
	public Vector2 knockback = new Vector2(50.0f,0.0f);
	public float damage = 0.0f;

	internal void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.GetComponent<Attackable>()) {
			Attackable otherObj = other.gameObject.GetComponent<Attackable> ();
			float counterF = (other.gameObject.GetComponent<Movement> ().velocity.x * (1/Time.deltaTime));
			if (fixedKnockback) {
				if (counterF > 0.0f) {
					counterF = 0.0f;
				} else {
					otherObj.addToVelocity (new Vector2(knockback.x, knockback.y - counterF));
				}
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
		}
	}
}
