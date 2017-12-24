using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlObserve : Goal{

	public Character targetCh;
	Proposal turnProp;
	public GlObserve() {
		//turnProp = new Proposal ();
		//turnProp.mMethod = turn;
		//turnProp.evalMethod = evaluateTurn;
		registerEvent("sight",outOfSight,turn);
		registerEvent ("sight", sawCharacter, learnCharacter);
		registerEvent ("interact", interactEvent, turn);

	}

	//void evaluateTurn(Proposal p) {}
	void turn(Proposal p) {
		if (p.mEvent.targetChar.transform.position.x > mChar.transform.position.x) {
			mChar.GetComponent<Movement> ().setFacingLeft (false);
		} else {
			mChar.GetComponent<Movement> ().setFacingLeft (true);
		}
	}

	void learnCharacter(Proposal p) {
		Debug.Log ("Learning Character: " + p.mEvent.targetChar.name);
		mChar.knowledgeBase.LearnSubject (GameObject.FindObjectOfType<KNManager>().FindOrCreateSubject(p.mEvent.targetChar.name));
	}

	float sawCharacter(Event e,Relationship r,Personality p) {
		EVSight se = (EVSight)e;
		if (se.onSight) {
			//Debug.Log ("Saw character: " + r.Name);
			if (!mChar.knowledgeBase.HasSubject(GameObject.FindObjectOfType<KNManager>().FindOrCreateSubject(r.Name))) {
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
				//mChar.addProposal (turnProp, e, favor);
				return favor;
			}
		}
		return 0;
	}
	public float interactEvent(Event e,Relationship ci,Personality p) {
		return 1f;
	}
}