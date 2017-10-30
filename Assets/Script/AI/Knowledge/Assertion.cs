using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AssertionType {
	fact,
	observation
};

public class Assertion {
	public Character mChar;
	public KNDatabase parentDB;
	public AssertionType assertionType;
	public List<string> knowledgeGroups;
	public string info;

	public float timeLearned = 0f;
	public float lastTimeDiscussed = 0f;
	public Character lastCharacterDiscussed;
	public KNSubject factSource;
	public bool inquiry;
	public float secrecy = 0f;

	float assuration;
	float support;

	public List<KNSubject> subjects;
	public KNVerb verb;
	public List<KNSubject> directObjects;

	public Assertion() {
		knowledgeGroups = new List<string> ();
		subjects = new List<KNSubject> ();
		directObjects = new List<KNSubject> ();
	}
	public bool equals(Assertion f) {
		for (int i = 0; i < f.subjects.Count; i++) {
			if (f.subjects[i].subjectName != subjects[i].subjectName) {
				return false;
			}
		}
		if (f.verb.verbName != verb.verbName) {
			return false;
		}
		for (int i = 0; i < f.subjects.Count; i++) {
			if (f.directObjects[i].subjectName != directObjects[i].subjectName) {
				return false;
			}
		}
		return true;
	}

	public string getID() {
		string idStr = "";
		if (subjects.Count == 0) {
			idStr += "ANYSUBJECT-";
		} else {
			idStr += subjects [0].subjectName + "-";
		}
		if (verb == null) {
			idStr += "ANYVERB-";
		} else {
			idStr += verb.verbName +  "-";
		}
		if (directObjects.Count == 0) {
			idStr += "ANYSUBJECT";
		} else {
			idStr += directObjects [0].subjectName;
		}

		return idStr;
	}

	public Assertion copyAssertion() {
		Assertion newA = new Assertion ();
		newA.assertionType = assertionType;
		newA.knowledgeGroups = knowledgeGroups;
		foreach (KNSubject item in subjects) {
			if (item != null)
				newA.subjects.Add (item);
		}
		newA.verb = verb;
		foreach (KNSubject item in directObjects) {
			if (item != null)
				newA.directObjects.Add (item);
		}
		newA.info = info;
		return newA;
	}

	public bool isMatch(Assertion a) {
		bool found = false;
		if (a.subjects != null) {
			foreach (KNSubject s in a.subjects) {
				if (subjectMatch (s)) {
					found = true;
					continue;
				}
			}
			if (!found) {
				return false;
			}
		}
		if (a.verb != null) {
			if (!verbMatch (a.verb))
				return false;
		}
		if (a.directObjects != null) {
			found = false;
			foreach (KNSubject s in a.directObjects) {
				if (DOMatch (s)) {
					found = true;
					continue;
				}
			}
			if (!found) {
				return false;
			}
		}
		return true;
	}
	
	public bool subjectMatch(KNSubject ks) {
		bool found = false;
		foreach (KNSubject s in subjects) {
			if (s.match(ks)) {
				found = true;
				continue;
			}
		}
		return found;
	}

	public bool verbMatch(KNVerb kv) {
		return verb.match(kv);
	}

	public bool DOMatch(KNSubject ks) {
		bool found = false;
		foreach (KNSubject s in directObjects) {
			if (s.match(ks)) {
				found = true;
				continue;
			}
		}
		return found;
	}

}