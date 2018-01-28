using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (OffenseAI))]
[RequireComponent (typeof (Playable))]
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
	Playable m_player;
	int m_lastNumGoals = 0;

	List<Event> m_currentEvents;

	void Start () {
		base.init ();
		m_currentEvents = new List<Event> ();
		offense = GetComponent<OffenseAI> ();
		m_currentGoals = new List<Goal> ();
		m_newProposals = new List<Proposal>();
		m_currentProposals = new List<Proposal>();
		m_player = GetComponent<Playable> ();
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
		if (!m_player.IsCurrentPlayer) {
			if (autonomy && m_newProposals.Count > 0) {
				executeValidProposals ();
				m_currentEvents.Clear ();
			}
			updateGoalList ();
		}
	}

	void updateGoalList() {
		int len = GoalNames.Count;
		if (len != m_lastNumGoals) {
			m_lastNumGoals = len;
		}
	}
	public override void setAutonomy(bool au) {
		//Debug.Log ("Setting autonomy " + gameObject + " to " + au);
		autonomy = au;
		if (au) {
		} else {
			GetComponent<Playable> ().endTarget ();
		}
		GetComponent<Playable> ().autonomy = au;
	}
	public void AddProposal(Proposal p, Event e) {
		AddProposal (p, e, -100f);
	}

	public void AddProposal(Proposal p, Event e,float rating) {
		if (!m_newProposals.Contains (p)) {
			p.mEvent = e;
			if (rating > -100f) {
				p.setRating(rating);
			}
			m_newProposals.Add (p);
		}
	}

	public void AddProposal(Goal.executionMethod ex, Event e, float rating, ProposalClass pClass = ProposalClass.None) {
		Proposal p = new Proposal();
		Debug.Log ("Method: " + ex + " proposed with rating: " + rating);
		p.mMethod = ex;
		p.rating = rating;
		p.mNPC = this;
		AddProposal(p,e,rating);
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
		if (!m_player.IsCurrentPlayer) {
			//Debug.Log (name + " is responding to event: " + e.eventType);
			if (!m_currentEvents.Contains (e)) {
				foreach (Goal g in m_currentGoals) {
					//Debug.Log ("Goal is : " + g);
					g.respondToEvent (e);
				}
			}
			m_currentEvents.Add (e);
		}
	}

	public override void processDialogueRequest(Character c,DialogueUnit d) {
		if (!GetComponent<OffenseAI> () || GetComponent<OffenseAI> ().currentTarget != c) {
			//c.acceptDialogue (this,d);
		}
	}
	public override void acceptDialogue(Character c,DialogueUnit d) {	}
	public override void setTargetPoint(Vector3 targetPoint, float proximity) {
		GetComponent<Playable> ().setTargetPoint (targetPoint, proximity);
	}
	public override DialogueSubunit chooseDialogueOption(List<DialogueSubunit> dList) {
		if (dList.Count > 0) {
			return dList [0];
		} else {
			return null;
		}
	}

	public void SetTarget(Character target) {
		getCharInfo (target).openHostile = true;
		offense.setTarget (target);
	}
}