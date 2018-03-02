using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal {

	public delegate float evaluationMethod( Event e );
	public delegate void immediateExecute( Event e );
	public delegate void executionMethod( Proposal p );

	public bool successful = true;
	public NPC mChar = null;
	public string relationsPath = "relations";
	public string objectsPath = "objects";

	Dictionary<EventType,List<evaluationMethod>> evalMethods = new Dictionary<EventType,List<evaluationMethod>> ();
	Dictionary<EventType,List<immediateExecute>> immExecMethods = new Dictionary<EventType,List<immediateExecute>>();
	Dictionary<EventType,List<executionMethod>> probabilityMethods = new Dictionary<EventType,List<executionMethod>>();
	Dictionary<evaluationMethod,executionMethod> execMethods = new Dictionary<evaluationMethod,executionMethod> ();
	Dictionary<executionMethod,ProposalClass> execToClass = new Dictionary<executionMethod,ProposalClass> ();

	public Goal () {}

	protected void registerEvent(EventType eventType, immediateExecute immM) {
		if (!(immExecMethods.ContainsKey (eventType)))
			immExecMethods [eventType] = new List<immediateExecute> ();
		immExecMethods[eventType].Add(immM);
	}
	protected void registerEvent(EventType eventType, float probability, executionMethod immE, ProposalClass proposalClass = ProposalClass.None) {
		if (Random.value > probability)
			return;
		if (!(probabilityMethods.ContainsKey (eventType)))
			probabilityMethods [eventType] = new List<executionMethod> ();
		probabilityMethods[eventType].Add(immE);
		execToClass[immE] = proposalClass;
	}
	protected void registerEvent(EventType eventType, evaluationMethod evalM) {
		if (!(evalMethods.ContainsKey (eventType)))
			evalMethods [eventType] = new List<evaluationMethod> ();
		evalMethods[eventType].Add(evalM);
	}
	protected void registerEvent(EventType eventType, evaluationMethod evalMethod, executionMethod execMethod, ProposalClass proposalClass = ProposalClass.None) {
		if (!(evalMethods.ContainsKey (eventType)))
			evalMethods [eventType] = new List<evaluationMethod> ();
		evalMethods[eventType].Add(evalMethod);
		execMethods[evalMethod] = execMethod;
		execToClass [execMethod] = proposalClass;
	}

	public virtual void onImport() {}

	public virtual void respondToEvent(Event e) {
		EventType eventName = e.eventType;
		//Debug.Log ("Event of type: " + eventName);
		//Debug.Log (this + " hasEvent: " + evalMethods.ContainsKey (eventName));
		if (immExecMethods.ContainsKey(eventName)) {
			foreach (immediateExecute eX in immExecMethods[eventName]) {
				//Relationship ci = mChar.getCharInfo (e.targetChar);
				eX (e);
			}
		}
		if (probabilityMethods.ContainsKey(eventName)) {
			foreach (executionMethod eX in probabilityMethods[eventName]) {
				Proposal p = new Proposal ();
				p.mNPC = mChar;
				p.mEvent = e;
				p.mMethod = eX;
				p.rating = 1.0f;
				p.ProposalType = execToClass [eX];
				eX (p);
			}
		}
		if (evalMethods.ContainsKey (eventName)) {
			foreach (evaluationMethod eM in evalMethods[eventName]) {
				//Relationship ci = mChar.getCharInfo (e.targetChar);
				//Debug.Log ("Relationship retrieved: " + ci);
				float rating = eM ( e );
				if (rating != 0.0 && rating != 1.0) {
					Debug.Log("Char:" + mChar.name + " Event: " + eventName + " Goal: " + this.GetType() + " Rating: " + rating + " Class: "+ execToClass[ execMethods[eM]]);
				}
				if (execMethods.ContainsKey(eM) && rating > 0.0) {
					Proposal p = new Proposal ();
					p.rating = rating;
					p.mNPC = mChar;
					p.mEvent = e;
					p.mMethod = execMethods[eM];
					p.ProposalType = execToClass[ execMethods[eM]];
					mChar.AddProposal (p, e, rating);
				}
			}
		}

	}
	protected float always(Event e )  {
		return 1f;
	}

	public virtual void onGoalSuccessful() {}
	public virtual void onGoalFail() {}
}