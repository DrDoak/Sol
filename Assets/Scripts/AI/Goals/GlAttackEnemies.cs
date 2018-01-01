using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlAttackEnemies : Goal {

	// Use this for initialization
	public GlAttackEnemies () {
		registerEvent (EventType.Sight, sightEvent,initiateAttack);
		registerEvent (EventType.Hit, hitEvent, investigateHit);
	}

	void initiateAttack(Proposal p) {
		EVSight evs = (EVSight)p.mEvent;
		mChar.offense.setTarget (evs.ObservedChar);
	}

	float sightEvent(Event e) {
		EVSight evs = (EVSight)e;
		Character tC = evs.ObservedChar;
		if (tC != null && tC.faction != mChar.faction)
			return 1f;
		return 0;
	}

	void investigateHit(Proposal p) {
		EVHitConfirm eva = (EVHitConfirm)p.mEvent;
		if (eva.attacker.transform.position.x < mChar.transform.position.x) {
			mChar.GetComponent<Movement> ().setFacingLeft (true);
		} else {
			mChar.GetComponent<Movement> ().setFacingLeft (false);
		}
	}
	float hitEvent(Event e) {
		EVHitConfirm eva = (EVHitConfirm)e;
		if (eva.ObjectHit == mChar.gameObject) {
			return 1f;
		}
		return 0f;
	}
}
