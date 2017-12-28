using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNSubject {
	public Assertion ParentAssertion;
	public string SubjectName;
	public string SubjectDisplayed = "none";
	public List<KNSubject> Parents;
	public List<KNSubject> Contradictions;
	public Character Owner;
	public bool Hide = false;
	public bool Exclamation = false;
	int count;
	public KNSubject() {
		Parents = new List<KNSubject> ();
		Contradictions = new List<KNSubject> ();
	}
	public override bool Equals( System.Object obj ) {
		if (obj == null)
			return false;
		KNSubject ks = obj as KNSubject;
		if (ks.SubjectName == SubjectName)
			return true;
		foreach (var s in Parents) {
			if (s.Equals (ks)) {
				return true;
			}
		}
		return false;
	}
	public virtual KNSubject Copy() {
		KNSubject ks = new KNSubject {SubjectName = SubjectName, Parents = Parents,
			Contradictions = Contradictions, Owner = Owner, Hide = Hide, Exclamation = Exclamation};
		return ks;
	}
	public virtual string GetID() {
		return SubjectName;
	}
	public virtual string Convey() {
		return SubjectName;
	}
}
