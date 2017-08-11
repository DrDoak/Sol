using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlAttackEnemies : Goal {

	Proposal initAttackProp;
	// Use this for initialization
	public GlAttackEnemies () {
		initAttackProp = new Proposal ();
		//initAttackProp.mNPC = targetChar;
		initAttackProp.mMethod = initiateAttack;
		initAttackProp.evalMethod = evaluateAttack;
	}
	
	// Update is called once per frame
	void Update () {

	}
	void evaluateAttack(Proposal p) {
		p.setRating(1.0f);
	}
	void initiateAttack(Proposal p) {
		mChar.offense.setTarget (p.mEvent.targetChar);
	}

	public override void respondToEvent(Event e) {
		if (e.eventType == "sight") {
			Character tC = e.targetChar;
			if (tC.faction != mChar.faction) {
				mChar.addProposal (initAttackProp,e);
			}
		}
	}

}
