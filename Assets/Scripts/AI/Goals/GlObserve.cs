using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlObserve : Goal{

	public Character targetCh;
	Proposal turnProp;
	public GlObserve() {
		registerEvent("sight",outOfSight,turn);
		registerEvent ("sight", sawCharacter, learnCharacter);
		registerEvent ("interact", turn);
		registerEvent ("interact", rememberInteraction);
		registerEvent ("hit", rememberHit);
	}

	void rememberInteraction(Proposal p) {
		Assertion a = new Assertion ();
		EVInteract evi = (EVInteract)p.mEvent;
		a.AddSubject (KNManager.FindOrCreateSubject (evi.targetChar.name));
		a.AddVerb (KNManager.FindOrCreateVerb("interact"));
		a.AddReceivor(KNManager.FindOrCreateSubject(evi.targetedObj.name));
		mChar.knowledgeBase.LearnAssertion (a);
	}

	void rememberHit(Proposal p) {
		Assertion a = new Assertion ();
		Debug.Log (p.mEvent.eventType);
		EVHitConfirm evh = (EVHitConfirm)p.mEvent;
		a.AddSubject (KNManager.FindOrCreateSubject (evh.targetChar.name));
		a.AddVerb (KNManager.FindOrCreateVerb("hit"));
		a.AddReceivor(KNManager.FindOrCreateSubject(evh.ObjectHit.name));
		mChar.knowledgeBase.LearnAssertion (a);
	}
	void turn(Proposal p) {
		if (p.mEvent.targetChar.transform.position.x > mChar.transform.position.x) {
			mChar.GetComponent<Movement> ().setFacingLeft (false);
		} else {
			mChar.GetComponent<Movement> ().setFacingLeft (true);
		}
	}

	void learnCharacter(Proposal p) {
		Debug.Log ("Learning Character: " + p.mEvent.targetChar.name);
		mChar.knowledgeBase.LearnSubject (KNManager.FindOrCreateSubject(p.mEvent.targetChar.name));
	}

	float sawCharacter(Event e,Relationship r,Personality p) {
		EVSight se = (EVSight)e;
		if (se.onSight) {
			//Debug.Log ("Saw character: " + r.Name);
			if (!mChar.knowledgeBase.HasSubject(KNManager.FindOrCreateSubject(r.Name))) {
				return 1.0f;				
			}
		}
		return 0.0f;
	}
	float outOfSight(Event e,Relationship r,Personality p) {
		EVSight se = (EVSight)e;
		if (!se.onSight) {
			if (mChar.GetComponent<OffenseAI> ().currentTarget) {
				if (mChar.GetComponent<OffenseAI> ().currentTarget !=
					e.targetChar){
					return 0.0f;
				}
			}
			if (r.openHostile) {
				mChar.addProposal (turnProp, e, 1f);
			} else {
				//Natural curiousity can be based on agreeableness.
				float favor = p.agreeableness * 0.3f;
				favor += r.relevance;
				favor += (r.authority * p.opennessAllegiance);

				// natural human nature nature to want to see things
				favor += (0.1f - p.temperament * 0.2f);
				return favor;
			}
		}
		return 0;
	}
}