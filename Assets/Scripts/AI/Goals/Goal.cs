using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal {

	public delegate float evaluationMethod( Event e,Relationship ci,Personality p);
	public delegate void immediateExecute( Event e,Relationship ci,Personality p);
	public delegate void executionMethod(Proposal p);

	public bool successful = true;
	public NPC mChar = null;
	public string relationsPath = "relations";
	public string objectsPath = "objects";

	Dictionary<string,List<evaluationMethod>> evalMethods = new Dictionary<string,List<evaluationMethod>> ();
	Dictionary<string,List<immediateExecute>> immExecMethods = new Dictionary<string,List<immediateExecute>>();
	Dictionary<evaluationMethod,executionMethod> execMethods = new Dictionary<evaluationMethod,executionMethod> ();

	public Goal () {}

	protected void registerEvent(string eventType, immediateExecute immM) {
		if (!(immExecMethods.ContainsKey (eventType)))
			immExecMethods [eventType] = new List<immediateExecute> ();
		immExecMethods[eventType].Add(immM);
	}
	protected void registerEvent(string eventType, evaluationMethod evalM) {
		if (!(evalMethods.ContainsKey (eventType)))
			evalMethods [eventType] = new List<evaluationMethod> ();
		evalMethods[eventType].Add(evalM);
	}
	protected void registerEvent(string eventType, evaluationMethod evalMethod, executionMethod execMethod) {
		if (!(evalMethods.ContainsKey (eventType)))
			evalMethods [eventType] = new List<evaluationMethod> ();
		evalMethods[eventType].Add(evalMethod);
		execMethods[evalMethod] = execMethod;
	}

	public virtual void onImport() {}

	public virtual void respondToEvent(Event e) {
		string eventName = e.eventType;
		//Debug.Log ("Event of type: " + eventName);
		//Debug.Log (this + " hasEvent: " + evalMethods.ContainsKey (eventName));
		if (immExecMethods.ContainsKey(eventName)) {
			foreach (immediateExecute eX in immExecMethods[eventName]) {
				Relationship ci = mChar.getCharInfo (e.targetChar);
				eX (e, ci, mChar.pers);
			}
		}
		if (evalMethods.ContainsKey (eventName)) {
			foreach (evaluationMethod eM in evalMethods[eventName]) {
				Relationship ci = mChar.getCharInfo (e.targetChar);
				float rating = eM (e, ci, mChar.pers);
				if (execMethods.ContainsKey(eM) && rating > 0.0) {
					Proposal p = new Proposal ();
					p.mNPC = mChar;
					p.mEvent = e;
					p.mMethod = execMethods [eM];
					mChar.addProposal (p, e,rating);
				}
			}
		}
	}
	/*
	public virtual void sightEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawAttackEvent(Event e,Relationship ci,Personality p) {}
	public virtual void hitEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawHitEvent(Event e,Relationship ci,Personality p) {}
	public virtual void interactEvent(Event e,Relationship ci,Personality p) {}
	public virtual void sawInteractEvent(Event e,Relationship ci,Personality p) {}

	public virtual void askEvent(Event e,Relationship ci,Personality p) {}
	public virtual void tellEvent(Event e,Relationship ci,Personality p) {}
	public virtual void factEvent(Event e,Relationship ci,Personality p) {}
	*/

	public virtual void onGoalSuccessful() {}
	public virtual void onGoalFail() {}
}

/*	public virtual void respondToEvent(Event e) {
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
	}*/