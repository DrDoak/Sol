using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlSurvival: Goal {
	public GlSurvival() {}

	public override void sightEvent(Event e,Relationship r,Personality p) {
		if (r.openHostile) {
			fightFlight (r, p);
		} else {
			if (r.favorability < 0f) {
				//unfavorable people
			} else {
				//favorable people
			}
		}
	}
	void fightFlight(Relationship r, Personality p) {
		r.openHostile = true;
		float modifiedCombatEgo = (p.egoCombat + (p.egoCombat * p.confidence));
		//First calculate gut fight or flight instinct
		float combatR = r.relativeCombat + modifiedCombatEgo; // base perceived strength
		combatR -= (((1 - r.relevance) * modifiedCombatEgo) * p.boldness); //boldness from unfamiliar target
		combatR -= (p.boldness * (p.emotion + (p.emotion * p.emotionLogic))); // boldness due to emotion

		//then calculate gut instinct for peace.
		float modFavor = (r.favorability + (r.favorability * p.opennessAllegiance)) * Mathf.Max(0.1f,r.relevance);
		float peacefulResolve = modFavor + (modFavor * p.agreeableness); // more aggreeable people tend to favor peaceful resolutions.


		if (combatR < 0f) {
			//This enemy is weaker than me. I should attack.

		} else {
			//This enemy is stronger than me, I should flee
			if (peacefulResolve > 0f) {
			} else {
			}
		}
	}
	public override void sawAttackEvent(Event e,Relationship ci) {
		
	}
	public override void hitEvent(Event e,Relationship ci) {
		
	}
	public override void sawHitEvent(Event e,Relationship ci) {
		
	}
	public override void interactEvent(Event e,Relationship ci) {
		
	}
	public override void sawInteractEvent(Event e,Relationship ci) {
		
	}

}
