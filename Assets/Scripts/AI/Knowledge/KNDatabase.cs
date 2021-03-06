﻿using System.Collections;
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

	public bool Match(KNSubject sub1, string sub2) {
		return Match (sub1, KNManager.CopySubject (sub2));
	}

	public bool Match(KNSubject sub1, KNSubject sub2) {
		if (sub1.Equals (sub2)) {
			return true;
		} else {
			return HasMatch (new Assertion (sub1.SubjectName, "isa", sub2.SubjectName));
		}
	}
	public bool Match(KNVerb ver1, string ver2) {
		return Match (ver1, KNManager.CopyVerb (ver2));
	}

	public bool Match(KNVerb ver1, KNVerb ver2) {
		if (ver1.Equals (ver2)) {
			return true;
		} else {
			return HasMatch (new Assertion (ver1.VerbName, "isa", ver2.VerbName));
		}
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

	public float TimeSinceLastMatch(Assertion matchA) {
		float time = float.MaxValue;
		foreach (var a in Knowledge.Values) {
			if (a.IsMatch (matchA))
				time = Mathf.Min (time, GameManager.GameTime - a.LastTimeReferenced);
		}
		return time;
	}

	public void AddAssertion(Assertion k) {
		Assertion newA = k;
		foreach (var f in Knowledge.Values) {
			if (f.GetID() == k.GetID()) {
				f.LastTimeReferenced = k.LastTimeReferenced;
				f.TimesReferenced.Add (k.LastTimeReferenced);
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

	public Assertion LearnAssertion(Assertion newF) {
		var evf = new EVFact ();
		Assertion oldAssertion = GetAssertion (newF);
		if (oldAssertion != null) {
			oldAssertion.LastTimeReferenced = newF.LastTimeReferenced;
			oldAssertion.TimesReferenced.Add (newF.LastTimeReferenced);

			//Debug.Log ("Adding old reference: " + oldAssertion.TimesReferenced.Count);
			evf.assertion = oldAssertion;
			evf.isDuplicate = true;
			Owner.respondToEvent (evf);
			return oldAssertion;
		} else {
			//Debug.Log ("new assertion reference!");
			evf.assertion = newF;
			AddAssertion (evf.assertion);
			if (newF.Source == null)
				newF.Source = KNManager.CopySubject (Owner.name);
			Owner.respondToEvent (evf);
			return newF;
		}
		//Debug.Log (Owner.name + " is learning: " + newF.GetID() + " source is: " + evf.assertion.Source.SubjectName);
	}

	public Assertion GetAssertion(string factID) {
		if (Knowledge.ContainsKey(factID)) {
			return null;
		} else {
			return Knowledge[factID];
		}
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

	public float GetDecayRatio(Assertion a, float maxDecayTime = 300.0f, float compoundDecay = 0.4f, bool learn = false) {
		float ratio = 1.0f;
		Assertion b = GetAssertion (a);
		if (b != null) {
			//Debug.Log ("Times referenced: " + b.TimesReferenced.Count);
			foreach (float t in b.TimesReferenced) {
				ratio -= compoundDecay * Mathf.Max (0f, (1f - (GameManager.GameTime - t) / maxDecayTime));
			}
		}
		if (learn) {
			//Debug.Log ("Learning assertion");
			if (b == null) {
				LearnAssertion (a);
			} else {
				LearnAssertion (b);
			}
		}
		return Mathf.Max(0f,ratio);
	}

	public float GetScaleRatio(Assertion a, float maxDecayTime = 300.0f, float compoundScale = 0.5f, bool learn = false) {
		float ratio = 1.0f;
		Assertion b = GetAssertion (a);
		if (b != null) {
			foreach (float time in b.TimesReferenced) {
				ratio += compoundScale * Mathf.Max (0f, (1f - (GameManager.GameTime - time) / maxDecayTime));
			}
		}
		if (learn) {
			if (b == null) {
				LearnAssertion (a);
			} else {
				LearnAssertion (b);
			}
		}
		return Mathf.Max(0f,ratio);
	}
}