using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNSubject {
	public Assertion ParentAssertion;
	public string SubjectName;
	public List<KNSubject> Parents;
	public List<KNSubject> Contradictions;
	public Character Owner;
	int count;
	public KNSubject() {
		Parents = new List<KNSubject> ();
		Contradictions = new List<KNSubject> ();
	}
	public virtual bool Match(KNSubject ks) {
		if (ks.SubjectName == SubjectName)
			return true;
		foreach (var s in Parents) {
			if (s.Match (ks)) {
				return true;
			}
		}
		return false;
	}
	public virtual KNSubject Copy() {
		KNSubject ks = new KNSubject {SubjectName = SubjectName, Parents = Parents,
			Contradictions = Contradictions, Owner = Owner};
		return ks;
	}
	public virtual string GetID() {
		return SubjectName;
	}
	public virtual string Convey() {
		return SubjectName;
	}
}
