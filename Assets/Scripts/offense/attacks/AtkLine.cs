using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkLine : AttackInfo {

	public float range = 0f;
	public Vector2 direction = new Vector2 (0f, 0f);

	void Start () {
		init ();
	}

	public override void onAttack() {
		Vector2 realKB = knockback;
		Vector2 realOff = HitboxOffset;
		Vector2 realD = direction;
		string fac = GetComponent<Attackable> ().faction;
		if (GetComponent<Movement> ().facingLeft) {
			realKB = new Vector2 (-knockback.x, knockback.y);
			realOff = new Vector2 (-HitboxOffset.x, HitboxOffset.y);
			realD = new Vector2 (-direction.x, direction.y);
		}
	
		LineHitbox lbox = 	GetComponent<HitboxMaker>().createLineHB (range, realD, realOff, damage, hitboxDuration, realKB,fac, true);
		lbox.stun = stun;
	}
}
