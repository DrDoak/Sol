using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkSolUpAir : AtkDash {

	float timeSinceAttack = 0;
	int numHits = 1;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {}
	public override void onAttack() {
		base.onAttack ();
		timeSinceAttack = 0f;
	}
	public override void recoveryTick() {
		if (timeSinceAttack == 0f) {
			numHits = 1;
		}
		if (timeSinceAttack > 0.2f && numHits == 1) {
			numHits = 2;
			HitboxMaker hbm = GetComponent<HitboxMaker> ();
			Vector2 realKB = new Vector2(2f,10f);
			Vector2 realOff = new Vector2(1.5f,-1f);
			if (GetComponent<Movement> ().facingLeft) {
				realOff.x = realOff.x * -1f;
				realKB.x = realKB.x * -1f;
			}
			hbm.stun = 0.5f;
			hbm.createHitbox (new Vector2(2.5f,0.8f), realOff, 5f, 0.2f, realKB, true, GetComponent<Attackable>().faction, true);
		} else if (timeSinceAttack > 0.4f && numHits == 2) {
			numHits = 3;
			HitboxMaker hbm = GetComponent<HitboxMaker> ();
			Vector2 realKB = new Vector2(10f,18f);
			Vector2 realOff = new Vector2(1.5f,-0.5f);
			if (GetComponent<Movement> ().facingLeft) {
				realOff.x = realOff.x * -1f;
				realKB.x = realKB.x * -1f;
			}

			hbm.stun = 1.0f;
			hbm.createHitbox (new Vector2(2.5f,0.9f), realOff, 5f, 0.5f, realKB, true, GetComponent<Attackable>().faction, true);
		}

		timeSinceAttack += Time.deltaTime;
		
	}

}
