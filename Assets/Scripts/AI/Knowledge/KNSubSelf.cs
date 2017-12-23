using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNSubSelf : KNSubject {

	public KNSubSelf() {
		SubjectName = "self";
		Parents = new List<KNSubject> ();
		Contradictions = new List<KNSubject> ();
	}
	public override bool Match(KNSubject ks) {
		//Debug.Log ("attempting match: " + ks.getID());
		//Debug.Log ("I am: " + mChar.name.ToLower ());
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
