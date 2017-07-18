using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (OffenseAI))]
[RequireComponent (typeof (NPCMovement))]
public class NPC : Character {

	List<Goal> currentGoals;
	// Use this for initialization
	List<Proposal> newProposals;
	List<Proposal> currentProposals;
	List<Proposal> currentActions;
	public OffenseAI offense;

	void Start () {
		base.init ();
		offense = GetComponent<OffenseAI> ();
		currentGoals = new List<Goal> ();
		newProposals = new List<Proposal>();
		currentProposals = new List<Proposal>();
		Goal g = (Goal)(new GlSurvival ());
		addGoal (g);
	}
	
	// Update is called once per frame
	void Update () {
		base.updateScan ();
		if (newProposals.Count > 0) {
			executeValidProposals ();
		}
	}

	public void addProposal(Proposal p) {
		if (!newProposals.Contains (p)) {
			newProposals.Add (p);
		}
	}
	void executeValidProposals() {
		foreach (Proposal p in newProposals) {
			p.evalMethod (p);
			if (p.rating > 0f) {
				executeProposalEvent (p);
			}
		}
		if (currentProposals.Count > 0) {
			for (int i= currentProposals.Count - 1; i >= 0; i --) {
				Proposal p = currentProposals [i];
				p.evalMethod (p);
				if (p.rating <= 0f) {
					currentProposals.RemoveAt (i);
				}
			}
		}
		newProposals.Clear ();
	}
	public void resolveProposal(Proposal p) {
		if (currentProposals.Contains (p)) {
			currentProposals.Remove (p);
		}
	}
	void executeProposalEvent(Proposal p) {
		p.mMethod (p);
		currentProposals.Add (p);
	}

	//Goals and GoalResponse
	public void addGoal(Goal g) {
		if (!currentGoals.Contains (g)) {
			g.mChar = this;
			currentGoals.Add (g);
		}
	}
	public void removeGoal(Goal g) {
		if (currentGoals.Contains (g)) {
			if (g.successful) {
				g.onGoalSuccessful();
			} else {
				g.onGoalFail ();
			}
			g.mChar = null;
			currentGoals.Remove (g);
		}
	}
	public override void onSight(Character otherChar) {
		SightEvent se = new SightEvent ();
		se.targetChar = otherChar;
		respondToEvent (se);
	}
	public override void outOfSight(Character otherChar) {
		SightEvent se = new SightEvent ();
		se.targetChar = otherChar;
		se.onSight = false;
		respondToEvent (se);
	}

	public void onHurtBy(Character otherChar) {
	}

	public void onHit(Character otherChar) {
	}
	public override void respondToEvent(Event e) {
		Debug.Log ("Responding to Event: " + e.eventType);
		foreach (Goal g in currentGoals) {
			g.respondToEvent (e);
		}
	}
	//Dialogue Request:
	public void setDialogueSequence(DialogueSequence ds, bool oneTime) {
		
	}
	public override void processDialogueRequest(Character c,DialogueSequence d) {
		if (!GetComponent<OffenseAI> () || GetComponent<OffenseAI> ().currentTarget != c) {
			c.acceptDialogue (this,d);
		}
	}
	public override void acceptDialogue(Character c,DialogueSequence d) {

	}
	public override DialogueSequence chooseDialogueOption(List<DialogueSequence> dList) {
		if (dList.Count > 0) {
			return dList [0];
		} else {
			return null;
		}
	}
}