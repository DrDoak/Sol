using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlSurvival: Goal {
	Proposal initAttackProp;
	Proposal fleeProp;
	Proposal negotiateProp;
	public GlSurvival() {
		fleeProp = new Proposal ();
		fleeProp.mMethod = initiateFlee;
		fleeProp.ProposalType = ProposalClass.Action;

		//registerEvent (EventType.Sight, sightEvent,attackOnSight);
		registerEvent (EventType.Attack, sawAttackEvent,initiateAttack);
		registerEvent (EventType.Hit, hitEvent, initiateAttack,ProposalClass.Action);
	}

	float sightEvent(Event e) {
		EVSight es = (EVSight)e;
		if (es.ObservedChar) {
			Relationship r = mChar.getCharInfo (es.ObservedChar);
			Personality p = mChar.PersonalityData;
			if (r.openHostile) {
				fightFlight (r, p, e);
			} else {
				//Initial empty impression of this guy
				float favor = r.favorability * r.relevance;
				//What is my tendency to trust my friends/ hate my enemies
				favor += (favor * p.opennessAllegiance);
				//natural tendency to violence or negotiation
				favor += (p.agreeableness * 0.4f);
				// tendancy for pragmatic people to avoid combat.
				favor -= (p.pragmaticIdealistic * 0.2f);
				// natural human nature to avoid fighting
				favor += (0.4f - p.temperament * 0.2f);
				return -favor;
			}
		}
		return 0f;
	}
	void attackOnSight(Proposal p) {
		EVSight es = (EVSight)p.mEvent;
		mChar.offense.setTarget (es.ObservedChar);
	}

	public float sawAttackEvent(Event e)  {
		EVAttack ea = (EVAttack)e;

		Relationship r = mChar.getCharInfo (ea.attacker);
		Personality p = mChar.PersonalityData;
		//Initial impression of this guy
		float favorAggressor = r.favorability * r.relevance;
		//What is my tendency to trust my friends/ hate my enemies
		favorAggressor +=  (favorAggressor * p.opennessAllegiance);
		//natural tendency to violence or agree
		favorAggressor += (p.agreeableness * 0.4f);
		// tendancy for pragmatic people to avoid combat.
		favorAggressor -= (p.pragmaticIdealistic * 0.2f);
		// natural human nature to avoid fighting
		favorAggressor += (0.2f - p.temperament * 0.2f);
		//Distance currently ungauged maybe used later?
		//float dist = Vector3.Distance (e.targetChar.transform.position, mChar.transform.position);
		return -favorAggressor;
	}

	float hitEvent(Event e) {
		EVHitConfirm eva = (EVHitConfirm)e;
		if (eva.ObjectHit == mChar.gameObject) {
			Relationship r = mChar.getCharInfo (eva.attacker);
			if (r.openHostile)
				return 1.0f;
			Personality p = mChar.PersonalityData;
			Assertion a = new Assertion ();
			a.AddSubject (KNManager.CopySubject (eva.attacker.name));
			a.AddVerb (KNManager.CopyVerb ("attack"));
			a.AddReceivor (KNManager.CopySubject (mChar.name));
			float scale = mChar.knowledgeBase.GetScaleRatio (a, 1800f) * ((-r.favorability * (1 + p.opennessAllegiance)) +
				(0.25f + (p.temperament * 0.05f) - (p.agreeableness * 0.05f) - (r.authority * (1f - r.affirmation))));
			mChar.getCharInfo(eva.attacker).ChangeFavor(-0.05f * mChar.knowledgeBase.GetScaleRatio(a,1800f,1.0f));
			return scale;
		}
		return 0f;
	}
	void initiateAttack(Proposal p) {
		//Debug.Log ("Attacking the attacker");
		EVAttack eva = (EVAttack)p.mEvent;
		mChar.SetTarget (eva.attacker);
	}

	void initiateFlee(Proposal p) {
		EVAttack eva = (EVAttack)p.mEvent;
		mChar.SetTarget (eva.attacker);
	}

	void initiateNegotiate(Proposal p) {
		EVAttack eva = (EVAttack)p.mEvent;
		mChar.SetTarget (eva.attacker);
	}

	void investigateHit(Proposal p) {
		EVHitConfirm eva = (EVHitConfirm)p.mEvent;
		if (eva.attacker.transform.position.x < mChar.transform.position.x) {
			mChar.GetComponent<Movement> ().setFacingLeft (true);
		} else {
			mChar.GetComponent<Movement> ().setFacingLeft (false);
		}
	}

	void fightFlight(Relationship r, Personality p,Event e) {
		r.openHostile = true;
		float modifiedCombatEgo = (p.egoCombat + (p.egoCombat * p.confidence));
		//First calculate gut fight or flight instinct
		float combatR = r.relativeCombat + modifiedCombatEgo; // base perceived strength
		combatR -= (((1 - r.relevance) * modifiedCombatEgo) * p.boldness); //boldness from unfamiliar target
		combatR -= (p.boldness * (p.emotion + (p.emotion * p.emotionLogic))); // boldness due to emotion

		//then calculate gut instinct for peace.
		float modFavor = (r.favorability + (r.favorability * p.opennessAllegiance)) * Mathf.Max(0.1f,r.relevance);
		float peacefulResolve = modFavor + (modFavor * p.agreeableness); // more aggreeable people tend to favor peaceful resolutions.
		peacefulResolve += (0.2f * p.agreeableness); //natural bias to peace/combat.
		if (combatR < 0f) {
		} else {
			peacefulResolve -= 0.2f;
		}
		//mChar.addProposal (negotiateProp,e,peacefulResolve);
		mChar.AddProposal (initiateAttack,e,combatR);
		mChar.AddProposal (initiateFlee, e, -combatR);
	}
}