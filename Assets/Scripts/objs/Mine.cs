using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour {

	// Use this for initialization
	public bool fixedKnockback = true;
	public Vector2 knockback = new Vector2(0.0f,60.0f);
	public float damage = 0.0f;
	public GameObject ExplosionPrefab;
	public Vector2 hitboxScale = new Vector2 (1.0f, 1.0f);
	public float hitboxDuration = 0.5f;
	public bool inPlay = true;
	public Vector2 respawnPos; 
	public float respawnTime = 10.0f;
	GameObject respawnObj;

	void Start(){

	}

	void Update(){
	}

	internal void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<Attackable>()) {
			Attackable otherObj = other.gameObject.GetComponent<Attackable> ();
			otherObj.damageObj (damage);
			float counterF = (other.gameObject.GetComponent<Movement> ().velocity.y * (1/Time.deltaTime));
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
		GameObject explosion = GameObject.Instantiate (ExplosionPrefab, transform.position, Quaternion.identity);
		explosion.transform.localScale = new Vector3 (hitboxScale.x/16f,hitboxScale.y/16f,hitboxScale.x/16f);
		gameObject.GetComponent<HitboxMaker> ().createHitbox(hitboxScale,Vector2.zero,damage,hitboxDuration,knockback,false,"noFaction",false);
		Destroy (this.gameObject);
//		FindObjectOfType<GameManager> ().soundfx.gameObject.transform.Find ("MineExplosion").GetComponent<AudioSource> ().Play ();
	}

}
