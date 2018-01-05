using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AssertionType {
	fact,
	observation
};

public class Assertion : KNSubject{
	public KNDatabase ParentDatabase;
	public AssertionType AssertionType;
	public List<string> KnowledgeGroups;

	public float TimeLearned = 0f;
	public float LastTimeReferenced = 0f;
	public List<float> TimesReferenced;
	public Character LastCharacterDiscussed;
	public KNSubject Source;
	public bool Inquiry;

	public bool HasSubject = false;
	public List<KNSubject> Subjects;
	public bool HasVerb = false;
	public KNVerb Verb;
	public bool HasReceivor = false;
	public List<KNSubject> Receivors;

	public Assertion() {
		KnowledgeGroups = new List<string> ();
		Subjects = new List<KNSubject> ();
		Receivors = new List<KNSubject> ();
		LastTimeReferenced = GameManager.GameTime;
		TimesReferenced = new List<float> ();
		TimesReferenced.Add (LastTimeReferenced);
		TimeLearned = GameManager.GameTime;
	}

	public void AddSubject(KNSubject s) {
		if (s != null) {
			HasSubject = true;
			Subjects.Add (s);
		}
	}
	public void AddVerb(KNVerb v) {
		if (v != null)
			AddVerb (v, false);
	}
	public void AddVerb(KNVerb v,bool inverted) {
		if (v != null) {
			Verb = v;
			HasVerb = true;
			v.Inverted = inverted;
		}
	}
	public void AddReceivor(KNSubject d) {
		if (d != null) {
			HasReceivor = true;
			Receivors.Add (d);
		}
	}

	public bool Equals(Assertion f) {
		if (Subjects.Count > 0) {
			for (int i = 0; i < f.Subjects.Count; i++) {
				if (!f.Subjects [i].Equals (Subjects [0])) {
					return false;
				}
			}
		}
		if (f.Verb != null && !f.Verb.Equals(Verb)) {
			return false;
		}
		if (Receivors.Count > 0) {
			for (int i = 0; i < f.Receivors.Count; i++) {
				if (!f.Receivors [i].Equals (Receivors [0])) {
					return false;
				}
			}
		}
		return true;
	}
	public string GetSubjectID() {
		if (!HasSubject)
			return "?SUB";
		return Subjects [0].GetID ();
	}
	public string GetVerbID()
	{
		if (!HasVerb) 
			return "?VERB";
		return Verb.GetID();
	}
	public string GetReceivorID() {
		if (!HasReceivor) 
			return "?SUB";
		return Receivors [0].GetID();	
	}
	public override string GetID() {
		string idStr = "(";
		idStr += GetSubjectID () + "-";;
		idStr += GetVerbID ()+  "-";
		idStr += GetReceivorID ();
		return idStr + ")";
	}

	public Assertion CopyAssertion() {
		var newA = new Assertion {AssertionType = AssertionType, KnowledgeGroups = KnowledgeGroups, Source = Source};
		foreach (KNSubject item in Subjects) {
			newA.AddSubject (item);
		}
		newA.AddVerb (Verb);
		foreach (var item in Receivors) {
			newA.AddReceivor (item);
		}
		return newA;
	}

	public bool IsMatch(Assertion a) {
		bool found = false;
		if (a.Subjects.Count > 0) {
			foreach (var s in a.Subjects) {
				if (SubjectMatch (s)) {
					found = true;
					continue;
				}
			}
			if (!found) {
				return false;
			}
		}
		if (a.Verb != null) {
			if (!VerbMatch (a.Verb))
				return false;
		}
		if (a.Receivors.Count > 0) {
			found = false;
			foreach (KNSubject s in a.Receivors) {
				//Debug.Log ("DO: " + s.getID ());
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
	
	public bool SubjectMatch(KNSubject ks) {
		if (Subjects.Count == 0)
			return true;
		bool found = false;
		foreach (var s in Subjects) {
			if (s.Equals(ks)) {
				found = true;
				continue;
			}
		}
		return found;
	}

	public bool VerbMatch(KNVerb kv) {
		if (Verb == null)
			return false;
		return Verb.Equals(kv);
	}

	public bool DOMatch(KNSubject ks) {
		if (Receivors.Count == 0)
			return true;
		bool found = false;
		foreach (var s in Receivors) {
			if (s.Equals(ks)) {
				found = true;
				continue;
			}
		}
		return found;
	}

	public Character GetOwner() {
		return Owner;
	}
	public void SetOwner(Character c) {
		Owner = c;
		foreach (KNSubject sub in Subjects) {
			sub.Owner = c;
		}
		foreach (KNSubject sub in Receivors) {
			sub.Owner = c;
		}
		if (Verb != null)
			Verb.Owner = c;
	}
	public override string Convey () {
		string s = "";
		if (HasSubject)
			s += Subjects [0].Convey () + " ";
		if (HasVerb)
			s += Verb.Convey () + " ";
		if (HasReceivor)
			s += Receivors [0].Convey ();
		return s;
	}
}