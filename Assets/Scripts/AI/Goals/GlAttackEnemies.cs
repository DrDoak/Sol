using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlAttackEnemies : Goal {

	// Use this for initialization
	public GlAttackEnemies () {
		registerEvent ("sight", sightEvent,initiateAttack);
		//registerEvent ("hit", hitEvent, initiateAttack);
	}

	void initiateAttack(Proposal p) {
		mChar.offense.setTarget (p.mEvent.targetChar);
	}

	float sightEvent(Event e,Relationship r,Personality p) {
		Character tC = e.targetChar;
		if (tC.faction != mChar.faction)
			return 1f;
		return 0;
	}

	void investigateHit(Proposal p) {
	}
	float hitEvent(Event e, Relationship r, Personality p) {
	}
}
