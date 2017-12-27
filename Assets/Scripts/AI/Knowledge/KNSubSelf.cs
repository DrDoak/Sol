using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNSubSelf : KNSubject {

	public KNSubSelf() {
		SubjectName = "self";
		Parents = new List<KNSubject> ();
		Contradictions = new List<KNSubject> ();
	}
	public override bool Equals( System.Object obj ) {
		if (obj == null || Owner == null)
			return false;
		KNSubject ks = obj as KNSubject;
		if (ks.GetID().ToLower() == Owner.name.ToLower ()) {
			//Debug.Log ("I subject MATCHED");
			return true;
		}
		return false;
	}
	public override string GetID() {
		if (Owner != null)
			return Owner.name;
		return "NoCharacter";
	}
	public override KNSubject Copy() {
		KNSubSelf ks = new KNSubSelf ();
		ks.SubjectName = "self";
		ks.Owner = Owner;
		return ks;
	}
}
