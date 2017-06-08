using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkReflector : AttackInfo {

	public bool rapidRecovery = false;

	public AudioClip ReflectSound;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public override void onStartUp() {
		GetComponent<Fighter> ().reflectProj = true;
	}
	public override void onAttack() {
		GetComponent<Attackable>().addResistence ("shot", base.hitboxDuration + 0.5f);
	}

	public override void onHitConfirm(GameObject other) {
		if (other.GetComponent<Projectile> ()) {
			Projectile p = other.GetComponent<Projectile> ();
			p.projectileSpeed.x = p.projectileSpeed.x * -1f;
			p.projectileSpeed.y = p.projectileSpeed.y * -1f;
			if (ReflectSound != null) {AudioSource.PlayClipAtPoint (ReflectSound, gameObject.transform.position);}
			hitbox hb = other.GetComponentInChildren<hitbox> ();
			hb.creator = gameObject;

			hb.faction = GetComponent<Attackable> ().faction;
			hb.collidedObjs = new List<Attackable> (); 
			hb.hitboxDuration = Mathf.Min(Mathf.Max(0.5f,hb.hitboxDuration),2.0f) * 4.0f;
			p.duration = hb.hitboxDuration;
		}
		if (rapidRecovery) {
			GetComponent<Fighter>().recoveryTime = GetComponent<Fighter>().recoveryTime * 0.5f;
		}
	}

	public override void onInterrupt(float stunTime, bool successfulHit, hitbox hb) {
		if (rapidRecovery) {
			GetComponent<Fighter>().recoveryTime = GetComponent<Fighter>().recoveryTime * 0.5f;
		}
	}
}
