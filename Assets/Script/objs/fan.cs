using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fan : MonoBehaviour {
	public Vector2 knockback = new Vector2(50.0f,0.0f);
	public GameObject hitboxClass;
	public bool facingLeft = true;
	public bool overrideDir = false;
	continuousHitbox mHitbox;
	Movement movement;

	public AudioClip fanClip;
	private AudioSource fanAudio;

	// Use this for initialization
	void Start () {
		fanAudio = gameObject.AddComponent<AudioSource> ();
		fanAudio.clip = fanClip;
		fanAudio.volume = 0.5f;
		fanAudio.Play ();


		float xOffset = 0;
		if (!overrideDir) {
			if (gameObject.GetComponent<Spawnable> ().angleDiff.x < 0) {
				facingLeft = true;

			} else {
				facingLeft = false;
			}
		}
		if (facingLeft) {
			xOffset = -7;
			GetComponent<SpriteRenderer> ().flipX = true;
		} else {
			xOffset = 7;
		}
		Vector3 initPos = new Vector3 (transform.position.x + xOffset, transform.position.y, transform.position.z);
		GameObject go = Instantiate(hitboxClass,initPos,Quaternion.identity) as GameObject; 
		mHitbox = go.GetComponent<continuousHitbox> ();
		mHitbox.setScale (new Vector2 (14.0f, 4.0f));
		mHitbox.setDamage (0.0f);
		mHitbox.setFixedKnockback (true); 
		mHitbox.setTimedHitbox (false);
		if (!facingLeft) {
			mHitbox.setKnockback (new Vector2(knockback.x,knockback.y));
		} else {
			mHitbox.setKnockback (new Vector2(knockback.x * -1.0f,knockback.y));
			ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem> ();
			ps.transform.rotation = Quaternion.Euler (0.0f, 270.0f, 0.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float xOffset = 0;
		if (facingLeft) {
			xOffset = -7;
		} else {
			xOffset = 7;
		}
		Vector3 fanPos = new Vector3 (transform.position.x + xOffset, transform.position.y, transform.position.z);
		mHitbox.transform.position = fanPos;
	}

	void OnDestroy () {
		GameObject.Destroy (mHitbox.gameObject);
	}
}
