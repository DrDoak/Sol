using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal {

	public bool successful = true;
	public NPC mChar = null;
	public string relationsPath = "relations";
	public string objectsPath = "objects";

	public Goal () {}

	public virtual void onImport() {
	}
	public virtual void respondToEvent(Event e) {
//		Debug.Log ("responding to: " + e.eventType);
		if (e.eventType == "sight") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			sightEvent (e, ci, mChar.pers);
		} else if (e.eventType == "attack") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			sawAttackEvent (e, ci, mChar.pers);
		} else if (e.eventType == "hit") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			if (e.targetChar.name == mChar.name) { 
				hitEvent (e, ci, mChar.pers);
			} else {
				sawHitEvent (e, ci, mChar.pers);
			}
		} else if (e.eventType == "interact") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			EVInteract ie = (EVInteract)e;
			//	Debug.Log ("I am: " + mChar + " he is: " + ie.listenerChar);
			if (ie.isCharInteraction && ie.listenerChar == mChar) { 
				interactEvent (e, ci, mChar.pers);
			} else {
				sawInteractEvent (e, ci, mChar.pers);
			}
		} else if (e.eventType == "ask") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			askEvent (e, ci, mChar.pers);
		} else if (e.eventType == "tell") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			tellEvent (e, ci, mChar.pers);
		} else if (e.eventType == "fact") {
			Relationship ci = mChar.getCharInfo (e.targetChar);
			factEvent (e, ci, mChar.pers);
		}
	}
	public virtual void sightEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawAttackEvent(Event e,Relationship ci,Personality p) {}
	public virtual void hitEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawHitEvent(Event e,Relationship ci,Personality p) {}
	public virtual void interactEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawInteractEvent(Event e,Relationship ci,Personality p) {}

	public virtual void askEvent(Event e,Relationship ci,Personality p) {}
	public virtual void tellEvent(Event e,Relationship ci,Personality p) {}
	public virtual void factEvent(Event e,Relationship ci,Personality p) {}

	public virtual void onGoalSuccessful() {}
	public virtual void onGoalFail() {}
}
