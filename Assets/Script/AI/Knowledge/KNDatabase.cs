using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class representing a database.
// intended to represent all "facts" known by a certain character.

public class KNDatabase {
	KNManager km;
	public Character c;
	public bool isMaster = false;

	public Dictionary<string,Assertion> knowledge = new Dictionary<string,Assertion>();
	public List<KNSubject> knownSubjects = new List<KNSubject>();
	public List<KNVerb> knownVerbs = new List<KNVerb>();

	Dictionary<Character,Relationship> charInfo = new Dictionary<Character,Relationship> ();

	public void initKnowledgeBase() {
		foreach (string s in c.knowledgeGroups) {
			km.addKnowledgeGroups (this, s);
		}
	}
	public void DisplayKnowledgeBase() {
	}

	public List<KNVerb> matchingVerbs(Assertion a) {
		List<KNVerb> verbs = new List<KNVerb> ();
		foreach (KNVerb kv in knownVerbs) {
			bool match = false;
			if (a.subjects != null) {
				foreach (KNSubject s in a.subjects) {
					if (!kv.canAct (s))
						continue;
				}
			}
			if (a.directObjects != null) {
				foreach (KNSubject s in a.directObjects) {
					if (!kv.canReceive (s))
						continue;
				}
			}
			verbs.Add (kv);
		}
		return verbs;
	}
	public List<KNSubject> matchingDirectObjects(Assertion a) {
		List<KNSubject> DOs = new List<KNSubject> ();
		foreach (KNSubject kv in knownSubjects) {
			if (a.verb != null) {
				if (!a.verb.canReceive (kv))
					continue;
			}
			DOs.Add (kv);
		}
		return DOs;
	}
	public List<Assertion> getMatches(Assertion matchA) {
		List<Assertion> matches = new List<Assertion> ();
		foreach (Assertion a in knowledge.Values) {
			if (a.isMatch (matchA)) {
				matches.Add (a);
			}
		}
		return matches;
	}

	public Assertion searchID(string factID) {
		if (knowledge.ContainsKey(factID)) {
			return null;
		} else {
			return knowledge[factID];
		}
	}

	public void addAssertion(Assertion k) {
		foreach (Assertion f in knowledge.Values) {
			if (f.equals (k)) {
				f.lastTimeDiscussed = k.lastTimeDiscussed;
				return;
			}
		}
		foreach (KNSubject sub in k.subjects) {
			if (!knownSubjects.Contains(sub) ){
				knownSubjects.Add (sub);
			}
		}
		foreach (KNSubject sub in k.directObjects) {
			if (!knownSubjects.Contains(sub) ){
				knownSubjects.Add (sub);
			}
		}
		if (!knownVerbs.Contains (k.verb)) {
			knownVerbs.Add (k.verb);
		}

		k.parentDB = this;
		knowledge.Add (k.getID(), k);
		EVFact factEvent = new EVFact ();
		factEvent.assertion = k;
		if (c) {
			k.mChar = c;
			c.respondToEvent (factEvent);
		}
	}

	public void learnFact(Assertion newF) {
		Debug.Log (c.name + " is learning fact: " + newF.getID());
		EVFact evf = new EVFact ();
		evf.assertion = newF;
		foreach (Assertion f in knowledge.Values) { 
			if (f.equals (newF)) {
				f.lastTimeDiscussed = newF.lastTimeDiscussed;
				evf.isDuplicate = true;
				evf.assertion = f;
				return;
			}
		}
		if (!evf.isDuplicate) {
			evf.assertion = newF;
			addAssertion (evf.assertion);
		}
		c.respondToEvent (evf);
	}
}
