using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkLine : AttackInfo {

	public float range = 0f;
	public Vector2 direction = new Vector2 (0f, 0f);

	void Start () {}
	void Update () {}

	public override void onAttack() {
		Vector2 realKB = knockback;
		Vector2 realOff = offset;
		Vector2 realD = direction;
		string fac = GetComponent<Attackable> ().faction;
		if (GetComponent<Movement> ().facingLeft) {
			realKB = new Vector2 (-knockback.x, knockback.y);
			realOff = new Vector2 (-offset.x, offset.y);
			realD = new Vector2 (-direction.x, direction.y);
		}
	
		lineHitbox lbox = 	GetComponent<HitboxMaker>().createLineHB (range, realD, realOff, damage, hitboxDuration, realKB,fac, true);
		lbox.stun = stun;
	}
}
