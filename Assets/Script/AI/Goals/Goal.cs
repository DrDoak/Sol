using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal {

	public bool successful = true;
	public NPC mChar = null;

	public Goal () {}

	public virtual void respondToEvent(Event e) {
		Debug.Log ("responding to: " + e.eventType);
		if (e.eventType == "sight") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			sightEvent (e, ci,mChar.pers);
		} else if (e.eventType == "attack") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			sawAttackEvent (e, ci,mChar.pers);
		} else if (e.eventType == "hit") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			if (e.targetChar.name == mChar.name) { 
				hitEvent (e, ci,mChar.pers);
			} else {
				sawHitEvent (e, ci,mChar.pers);
			}
		} else if (e.eventType == "interact") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			if (e.targetChar.name == mChar.name) { 
				interactEvent (e, ci,mChar.pers);
			} else {
				sawInteractEvent (e, ci,mChar.pers);
			}
		}
	}
	public virtual void sightEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawAttackEvent(Event e,Relationship ci,Personality p) {}
	public virtual void hitEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawHitEvent(Event e,Relationship ci,Personality p) {}
	public virtual void interactEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawInteractEvent(Event e,Relationship ci,Personality p) {}

	public virtual void onGoalSuccessful() {}
	public virtual void onGoalFail() {}
}
