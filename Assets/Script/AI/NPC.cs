using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (OffenseAI))]
[RequireComponent (typeof (NPCMovement))]
[RequireComponent (typeof (DialogueParser))]
public class NPC : Character {

	public List<string> goalNames;
	List<Goal> currentGoals;
	// Use this for initialization
	List<Proposal> newProposals;
	List<Proposal> currentProposals;
	List<Proposal> currentActions;
	public OffenseAI offense;
	public bool autonomy= true;

	void Start () {
		base.init ();
		offense = GetComponent<OffenseAI> ();
		currentGoals = new List<Goal> ();
		newProposals = new List<Proposal>();
		currentProposals = new List<Proposal>();
		Goal g = (Goal)(new GlSurvival ());
		addGoal (g);
		Goal g2 = (Goal)(new GlObserve ());
		addGoal(g2);
		Goal g3 = (Goal)(new GlEtiquette ());
		addGoal(g3);
	}
	
	// Update is called once per frame
	void Update () {
		base.mUpdate ();
		if (autonomy && newProposals.Count > 0) {
			executeValidProposals ();
		}
	}
	public override void setAutonomy(bool au) {
		autonomy = au;
		if (au) {
		} else {
			GetComponent<NPCMovement> ().endTarget ();
		}
	}
	public void addProposal(Proposal p, Event e) {
		addProposal (p, e, -100f);
	}
	public void addProposal(Proposal p, Event e,float rating) {
		if (!newProposals.Contains (p)) {
			p.mEvent = e;
			if (rating > -100f) {
				p.setRating(rating);
			}
			newProposals.Add (p);
		}
	}
	void executeValidProposals() {
		foreach (Proposal p in newProposals) {
			p.evalMethod (p);
			if (p.getRating() > 0f) {
				executeProposalEvent (p);
			}
		}
		if (currentProposals.Count > 0) {
			for (int i= currentProposals.Count - 1; i >= 0; i --) {
				Proposal p = currentProposals [i];
				p.evalMethod (p);
				if (p.getRating() <= 0f) {
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
			goalNames.Add (g.GetType ().ToString ());
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
			goalNames.Remove (g.GetType ().ToString ());
		}
	}
	public override void onSight(Character otherChar) {
		SightEvent se = new SightEvent ();
		se.targetChar = otherChar;
		respondToEvent (se);
	}
	public override void outOfSight(Character otherChar,bool full) {
		if (full) {
		} else {
			SightEvent se = new SightEvent ();
			se.targetChar = otherChar;
			se.onSight = false;
			respondToEvent (se);
		}
	}

	public void onHurtBy(Character otherChar) {
	}

	public void onHit(Character otherChar) {
	}
	public override void respondToEvent(Event e) {
	//	Debug.Log ("Responding to Event: " + e.eventType);
		foreach (Goal g in currentGoals) {
			g.respondToEvent (e);
		}
	}
	//Dialogue Request:
	/*public void setDialogueUnit(DialogueUnit ds, bool oneTime) {
		
	}*/
	public override void processDialogueRequest(Character c,DialogueUnit d) {
		if (!GetComponent<OffenseAI> () || GetComponent<OffenseAI> ().currentTarget != c) {
			//c.acceptDialogue (this,d);
		}
	}
	public override void acceptDialogue(Character c,DialogueUnit d) {	}
	public override void setTargetPoint(Vector3 targetPoint, float proximity) {
		GetComponent<NPCMovement> ().setTargetPoint (targetPoint, proximity);
	}
	public override DialogueSubunit chooseDialogueOption(List<DialogueSubunit> dList) {
		if (dList.Count > 0) {
			return dList [0];
		} else {
			return null;
		}
	}
}