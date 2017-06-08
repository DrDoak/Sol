using UnityEngine;
using System.Collections.Generic; //REMEMBER! In order to use lists! Make sure it is System.Collections.Generic instead of System.Collections
// Example of an object that can affect the physics of another object.
public class continuousHitbox : MonoBehaviour {
	List<Attackable> overlappingControl = new List<Attackable> (); 

	public float damage = 10.0f;
	public bool fixedKnockback = false;
	public Vector2 knockback = new Vector2(0.0f,4.0f);
	public float hitboxDuration = 1.0f;
	public bool timedHitbox = true;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {
		if (hitboxDuration > 0.0f || !timedHitbox) {
			foreach(Attackable cont in overlappingControl) {
				cont.damageObj (damage * Time.deltaTime);
				if (fixedKnockback) {
					cont.addToVelocity (knockback * Time.deltaTime);
				} else {
					Vector3 otherPos = cont.gameObject.transform.position;
					float angle = Mathf.Atan2 (transform.position.y - otherPos.y, transform.position.x - otherPos.x); //*180.0f / Mathf.PI;
					float magnitude = knockback.magnitude;
					float forceX = Mathf.Cos (angle) * magnitude;
					float forceY = Mathf.Sin (angle) * magnitude;
					Vector2 force = new Vector2 (-forceX, -forceY);
					cont.addToVelocity (force*Time.deltaTime);
				}
			}
			hitboxDuration = hitboxDuration - Time.deltaTime;
		} else {
			GameObject.Destroy (gameObject);
		}
	}
	public void setKnockback(Vector2 kb) {
		knockback = kb;
	}

	public void setFixedKnockback(bool fixedKB) {
		fixedKnockback = fixedKB;
	}

	public void setDamage(float dmg) {
		damage = dmg;
	}
	public void setHitboxDuration (float time) {
		hitboxDuration = time;
	}
	public void setScale(Vector2 scale) {
		transform.localScale = scale;
	}
	public void setTimedHitbox(bool timed) {
		timedHitbox = timed;
	}

	internal void OnTriggerEnter2D(Collider2D other) {
		//Debug.Log ("collision detected with Continuous hitbox");
		if (other.gameObject.GetComponent<Attackable>()) {
			overlappingControl.Add (other.gameObject.GetComponent<Attackable> ()); 
		}
	} 
	internal void OnTriggerExit2D(Collider2D other) {
		//Debug.Log ("Collision ended with Continuous hitbox");
		if (other.gameObject.GetComponent<Attackable> ()) {
			overlappingControl.Remove (other.gameObject.GetComponent<Attackable> ()); //Removes the object from the list
		}
	}
}
