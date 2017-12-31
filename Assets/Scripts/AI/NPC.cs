using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (OffenseAI))]
[RequireComponent (typeof (NPCMovement))]
[RequireComponent (typeof (DialogueParser))]
public class NPC : Character {

	public OffenseAI offense;
	public bool SimpleEnemy = false;
	public List<string> GoalNames;

	// Use this for initialization
	List<Proposal> m_newProposals;
	List<Proposal> m_currentProposals;
	List<Proposal> m_currentActions;
	List<Goal> m_currentGoals;
	int m_lastNumGoals = 0;

	List<Event> m_currentEvents;

	void Start () {
		base.init ();
		m_currentEvents = new List<Event> ();
		offense = GetComponent<OffenseAI> ();
		m_currentGoals = new List<Goal> ();
		m_newProposals = new List<Proposal>();
		m_currentProposals = new List<Proposal>();
		if (SimpleEnemy) {
			Goal g = (Goal)(System.Activator.CreateInstance(Type.GetType("GlAttackEnemies")));
			addGoal (g);
		}

		foreach (string goalName in GoalNames) {
			Goal g = (Goal)(System.Activator.CreateInstance(Type.GetType(goalName)));
			addGoal (g);
		}
	}

	void Update () {
		//Debug.Log ("Aut: " + autonomy + " :count: " + m_newProposals.Count);
		if (autonomy && m_newProposals.Count > 0) {
			//Debug.Log ("Executing Valid proposals");
			executeValidProposals ();
			m_currentEvents.Clear ();
		}
		updateGoalList ();
	}

	void updateGoalList() {
		int len = GoalNames.Count;
		if (len != m_lastNumGoals) {
			m_lastNumGoals = len;
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
		if (!m_newProposals.Contains (p)) {
			p.mEvent = e;
			if (rating > -100f) {
				p.setRating(rating);
			}
			m_newProposals.Add (p);
		}
	}

	void executeValidProposals() {
		Dictionary<ProposalClass, float> highestEvaluation = new Dictionary<ProposalClass, float> ();
		Dictionary<ProposalClass,Proposal> highestProposal = new Dictionary<ProposalClass, Proposal> ();

		foreach (Proposal p in m_newProposals) {
			/*if (p.evalMethod != null) {
				p.evalMethod (p);
			}*/

			//Debug.Log ("Rating is: " + p.getRating ());
			ProposalClass pClass = p.ProposalType;
			if (p.getRating () > 0f) {
				if (pClass == ProposalClass.None) {
					executeProposalEvent (p);
				} else if (!highestEvaluation.ContainsKey (pClass) || p.getRating () > highestEvaluation [pClass]) {
					highestEvaluation [pClass] = p.getRating ();
					highestProposal [pClass] = p;
				}
			}
		}
		foreach (Proposal p in highestProposal.Values){
			executeProposalEvent (p);
		}
		if (m_currentProposals.Count > 0) {
			for (int i= m_currentProposals.Count - 1; i >= 0; i --) {
				/*Proposal p = m_currentProposals [i];
				p.evalMethod (p);
				if (p.getRating() <= 0f) {
					m_currentProposals.RemoveAt (i);
				}*/
			}
		}
		m_newProposals.Clear ();
	}
	public void resolveProposal(Proposal p) {
		if (m_currentProposals.Contains (p)) {
			m_currentProposals.Remove (p);
		}
	}
	void executeProposalEvent(Proposal p) {
		p.mMethod (p);
		m_currentProposals.Add (p);
	}

	//Goals and GoalResponse
	public void addGoal(Goal g) {
		if (!m_currentGoals.Contains (g)) {
			g.mChar = this;
			m_currentGoals.Add (g);
			if (!GoalNames.Contains (g.GetType ().ToString ())) {
				GoalNames.Add (g.GetType ().ToString ());
			}
		}
	}
	public void removeGoal(Goal g) {
		if (m_currentGoals.Contains (g)) {
			if (g.successful) {
				g.onGoalSuccessful();
			} else {
				g.onGoalFail ();
			}
			g.mChar = null;
			m_currentGoals.Remove (g);
			GoalNames.Remove (g.GetType ().ToString ());
		}
	}

	public void onHurtBy(Character otherChar) {
	}

	public void onHit(Character otherChar) {
	}
	public override void respondToEvent(Event e) {
		//Debug.Log (name + " is responding to event: " + e.eventType);
		if (!m_currentEvents.Contains (e)) {
			foreach (Goal g in m_currentGoals) {
				//Debug.Log ("Goal is : " + g);
				g.respondToEvent (e);
			}
		}
		m_currentEvents.Add(e);
	}

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