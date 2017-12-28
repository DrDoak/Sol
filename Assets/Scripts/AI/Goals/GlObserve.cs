﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlObserve : Goal{

	public Character targetCh;
	Proposal turnProp;
	public GlObserve() {
		registerEvent("sight",outOfSight,turnSight);
		registerEvent ("sight", sawCharacter, learnCharacter);
		registerEvent ("interact", 1.0f, turnInteract);
		registerEvent ("interact", 1.0f, rememberInteraction);
		registerEvent ("hit", 1.0f, rememberHit);
	}

	void rememberInteraction(Proposal p) {
		Assertion a = new Assertion ();
		EVInteract evi = (EVInteract)p.mEvent;
		a.AddSubject (KNManager.CopySubject (evi.Interactor.name));
		a.AddVerb (KNManager.CopyVerb("interact"));
		a.AddReceivor(KNManager.CopySubject(evi.Interactee.name));
		mChar.knowledgeBase.LearnAssertion (a);
	}

	void rememberHit(Proposal p) {
		Assertion a = new Assertion ();
		EVHitConfirm evh = (EVHitConfirm)p.mEvent;
		a.AddSubject (KNManager.CopySubject (evh.attacker.name));
		a.AddVerb (KNManager.CopyVerb("attack"));
		a.AddReceivor(KNManager.CopySubject(evh.ObjectHit.name));
		mChar.knowledgeBase.LearnAssertion (a);
	}
	void turnInteract(Proposal p) {
		EVInteract evi = (EVInteract)p.mEvent;
		mChar.GetComponent<Movement> ().TurnToTransform (evi.Interactor.transform);
	}
	void turnSight(Proposal p) {
		EVSight evs = (EVSight)p.mEvent;
		mChar.GetComponent<Movement> ().TurnToTransform (evs.Observee.transform);
	}
	void learnCharacter(Proposal p) {
		EVSight se = (EVSight)p.mEvent;
		Debug.Log ("Learning Character: " + se.ObservedChar.name);
		mChar.knowledgeBase.LearnSubject (KNManager.CopySubject(se.ObservedChar.name));
	}

	float sawCharacter(Event e) {
		EVSight se = (EVSight)e;
		if (se.onSight && se.ObservedChar) {
			//Debug.Log ("Saw character: " + r.Name);
			Relationship r = mChar.getCharInfo (se.ObservedChar);
			if (!mChar.knowledgeBase.HasSubject(KNManager.CopySubject(r.Name)))
				return 1.0f;
		}
		return 0.0f;
	}

	float outOfSight(Event e) {
		EVSight se = (EVSight)e;
		if (!se.onSight && se.ObservedChar != null) {
			Relationship r = mChar.getCharInfo (se.ObservedChar);
			Personality p = mChar.PersonalityData;
			if (mChar.GetComponent<OffenseAI> ().currentTarget &&
				mChar.GetComponent<OffenseAI> ().currentTarget != se.ObservedChar)
				return 0.0f;
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