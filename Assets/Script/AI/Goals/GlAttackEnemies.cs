using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlAttackEnemies : Goal {

	Proposal initAttackProp;
	// Use this for initialization
	public GlAttackEnemies () {
		registerEvent ("sight", sightEvent,initiateAttack);
	}

	void initiateAttack(Proposal p) {
		mChar.offense.setTarget (p.mEvent.targetChar);
	}

	public float sightEvent(Event e,Relationship r,Personality p) {
		Character tC = e.targetChar;
		if (tC.faction != mChar.faction)
			return 1f;
		return 0;
	}

}
