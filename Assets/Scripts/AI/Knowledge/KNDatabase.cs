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
			//Debug.Log ("All verbs: " + kv.GetID());
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
	public bool HasMatch(Assertion matchA) {
		foreach (var a in Knowledge.Values) {
			if (a.IsMatch (matchA))
				return true;
		}
		return false;
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

	public void LearnAssertion(Assertion newF) {
		var evf = new EVFact ();
		Assertion oldAssertion = GetAssertion (newF);
		if (oldAssertion != null) {
			oldAssertion.LastTimeDiscussed = newF.LastTimeDiscussed;
			evf.assertion = oldAssertion;
			evf.isDuplicate = true;
		} else {
			evf.assertion = newF;
			AddAssertion (evf.assertion);
			if (newF.Source == null) {
				newF.Source = KNManager.CopySubject (Owner.name);
			}
		}
		//Debug.Log (Owner.name + " is learning: " + newF.GetID() + " source is: " + evf.assertion.Source.SubjectName);
		Owner.respondToEvent (evf);
	}

	public Assertion GetAssertion(Assertion a) {
		foreach (var f in Knowledge.Values) {
			if (f.Equals (a)) {
				return f;
			}
		}
		return null;
	}
	public bool HasSubject(KNSubject sub) {
		foreach(KNSubject s in Subjects) {
			if (s.GetID() == sub.GetID())
				return true;
		}
		return false;
	}
	public void LearnSubject(KNSubject s) {
		if (!Subjects.Contains (s)) {
			Subjects.Add (s);
		}
	}
	public bool HasVerb(KNVerb v) {
		return Verbs.Contains(v);
	}
	public void LearnVerb(KNVerb v) {
		//Debug.Log ("learning verb" + v.GetID () + " for: " + Owner.name + " Added: " + !Verbs.Contains(v));
		if (!Verbs.Contains (v)) {
			Verbs.Add (v);
		}
	}
}