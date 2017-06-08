using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public Vector3 projectileSpeed;
	public float rotationSpeed = 90.0f;
	public bool rotating = false;
	public bool penetrating = false;
	public float duration = 0.5f;
	public bool facingLeft = false;
	bool lastDirection = false;
	public GameObject creator;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += projectileSpeed * Time.deltaTime;
		if (rotating) {
			transform.Rotate (Vector3.forward * (rotationSpeed * Time.deltaTime));
		}
		duration = duration - Time.deltaTime;
		if (duration < 0.0f) {
			Destroy (gameObject);
		}

		if (lastDirection != facingLeft) {
			lastDirection = facingLeft;
			if (facingLeft) {
				SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer> ();
				sprite.flipX = true;
			} else {
				SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer> ();
				sprite.flipX = false;
			}
		}
	}
	internal void OnTriggerEnter2D(Collider2D other)
	{
		if (!GetComponentInChildren<hitbox> ()) {
			Debug.Log ("Projectile destroyed due to no hitbox");
			Destroy (gameObject);
		} else if (other.gameObject.GetComponent<Attackable> () && other.gameObject.GetComponent<Attackable> ().faction == GetComponentInChildren<hitbox> ().faction ||
			penetrating) {
		}else if (other.gameObject.GetComponent<hitbox> ()) { 
			
		} else {
			hitbox hb = GetComponentInChildren<hitbox> ();
			hb.OnTriggerEnter2D (other);
			Destroy (gameObject);
		}
	}
}
