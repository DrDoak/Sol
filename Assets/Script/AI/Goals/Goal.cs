using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal {

	public bool successful = true;
	public NPC mChar = null;

	public Goal () {}

	public void respondToEvent(Event e) {
		Debug.Log ("responding to: " + e.eventType);
		if (e.eventType == "sight") {
			Relationship ci = mChar.getCharInfo (e.targetChar,mChar.pers);
			sightEvent (e, ci);
		} else if (e.eventType == "attack") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			sawAttackEvent (e, ci);
		} else if (e.eventType == "hit") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			if (e.targetChar.name == mChar.name) { 
				hitEvent (e, ci);
			} else {
				sawHitEvent (e, ci);
			}
		} else if (e.eventType == "interact") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			if (e.targetChar.name == mChar.name) { 
				interactEvent (e, ci);
			} else {
				sawInteractEvent (e, ci);
			}
		}
	}
	public virtual void sightEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawAttackEvent(Event e,Relationship ci) {}
	public virtual void hitEvent(Event e,Relationship ci) {}
	public virtual void sawHitEvent(Event e,Relationship ci) {}
	public virtual void interactEvent(Event e,Relationship ci) {}
	public virtual void sawInteractEvent(Event e,Relationship ci) {}

	public virtual void onGoalSuccessful() {}
	public virtual void onGoalFail() {}
}
