using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlObserve : Goal{

	public Character targetCh;
	Proposal turnProp;
	public GlObserve() {
		turnProp = new Proposal ();
		turnProp.mMethod = turn;
		turnProp.evalMethod = evaluateTurn;
	}

	void evaluateTurn(Proposal p) {}
	void turn(Proposal p) {
		if (p.mEvent.targetChar.transform.position.x > mChar.transform.position.x) {
			mChar.GetComponent<Movement> ().setFacingLeft (false);
		} else {
			mChar.GetComponent<Movement> ().setFacingLeft (true);
		}
	}

	public override void sightEvent(Event e,Relationship r,Personality p) {
		SightEvent se = (SightEvent)e;
		if (!se.onSight) {
			if (r.openHostile) {
				mChar.addProposal (turnProp, e, 1f);
			} else {
				//Natural curiousity can be based on agreeableness.
				float favor = p.agreeableness * 0.3f;
				favor += r.relevance;
				favor += (r.authority * p.opennessAllegiance);

				// natural human nature nature to want to see things
				favor += (0.1f - p.temperament * 0.2f);
				mChar.addProposal (turnProp, e, favor);
			}
		}
	}
}