using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class representing a database.
// intended to represent all "facts" known by a certain character.

public class KNDatabase {
	
	public Character Owner;

	public Dictionary<string,Assertion> Knowledge = new Dictionary<string,Assertion>();
	public List<KNSubject> Subjects = new List<KNSubject>();
	public List<KNVerb> Verbs = new List<KNVerb>();
	KNManager km;

	public void InitKnowledgeBase() {
		foreach (string s in Owner.knowledgeGroups) {
			km.AddKnowledgeGroups (this, s);
		}
	}
	public void DisplayKnowledgeBase() {}

	public List<KNVerb> MatchingVerbs(Assertion a) {
		var verbs = new List<KNVerb> ();
		foreach (var kv in Verbs) {
			bool match = false;
			if (a.Subjects != null) {
				foreach (var s in a.Subjects) {
					if (!kv.CanAct (s))
						continue;
				}
			}
			if (a.Receivors != null) {
				foreach (var s in a.Receivors) {
					if (!kv.CanReceive (s))
						continue;
				}
			}
			verbs.Add (kv);
		}
		return verbs;
	}
	public List<KNSubject> MatchingDirectObjects(Assertion a) {
		var DOs = new List<KNSubject> ();
		foreach (var kv in Subjects) {
			if (a.Verb != null) {
				if (!a.Verb.CanReceive (kv))
					continue;
			}
			DOs.Add (kv);
		}
		return DOs;
	}
	public List<Assertion> GetMatches(Assertion matchA) {
		var matches = new List<Assertion> ();
		foreach (var a in Knowledge.Values) {
			if (a.IsMatch (matchA))
				matches.Add (a);
		}
		return matches;
	}

	public Assertion GetAssertion(string factID) {
		if (Knowledge.ContainsKey(factID)) {
			return null;
		} else {
			return Knowledge[factID];
		}
	}

	public void AddAssertion(Assertion k) {
		Assertion newA = k;
		foreach (var f in Knowledge.Values) {
			if (f.GetID() == k.GetID()) {
				f.LastTimeDiscussed = k.LastTimeDiscussed;
				return;
			}
		}
		foreach (KNSubject sub in k.Subjects) {
			sub.Owner = Owner;
			if (!Subjects.Contains(sub) )
				Subjects.Add (sub);
		}
		foreach (KNSubject sub in k.Receivors) {
			sub.Owner = Owner;
			if (!Subjects.Contains(sub) )
				Subjects.Add (sub);
		}
		if (!Verbs.Contains (k.Verb)) {
			k.Owner = Owner;
			Verbs.Add (k.Verb);
		}
		newA.SetOwner (Owner);
		k.ParentDatabase = this;
		Knowledge.Add (k.GetID(), k);
	}

	public void LearnFact(Assertion newF) {
		Debug.Log (Owner.name + " is learning fact: " + newF.GetID());
		//Debug.Log ("source is: " + newF.mChar);
		var evf = new EVFact ();
		evf.assertion = newF;
		foreach (var f in Knowledge.Values) { 
			if (f.Equals (newF)) {
				f.LastTimeDiscussed = newF.LastTimeDiscussed;
				evf.isDuplicate = true;
				evf.assertion = f;
				return;
			}
		}
		if (!evf.isDuplicate) {
			evf.assertion = newF;
			AddAssertion (evf.assertion);
		}
		Owner.respondToEvent (evf);
	}
}
