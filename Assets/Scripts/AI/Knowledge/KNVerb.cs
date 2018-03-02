using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNVerb {
	public string VerbName;
	public string VerbDisplayed = "none";
	public bool IsCommand = false;
	public bool Inverted;
	public bool Hide = false;

	public Character Owner;
	public List<KNSubject> Actors;
	public List<KNSubject> Receivors;
	public List<KNVerb> Parents;
	public List<KNVerb> Contradictory;

	public KNVerb() {
		Parents = new List<KNVerb> ();
		Contradictory = new List<KNVerb> ();
		Actors = new List<KNSubject> ();
		Receivors = new List<KNSubject> ();
	}
	public KNVerb Copy() {
		KNVerb v = new KNVerb {VerbName = VerbName, Actors = Actors,
			Receivors = Receivors, Parents = Parents, Contradictory = Contradictory,
			Inverted = Inverted,IsCommand = IsCommand};
		return v;
	}
	public bool CanAct(KNSubject a) {
		if (a == null)
			return true;
		foreach (var s in Actors) {
			if (a.Equals (s))
				return true;
		}
		return false;
	}
	public bool CanReceive(KNSubject receiver) {
		if (receiver == null)
			return true;
		foreach (var s in Receivors) {
			if (receiver.Equals (s))
				return true;
		}
		return false;
	}
	public bool EqualsID(string id) {
		return Equals (KNManager.CopyVerb (id));
	}
	public override bool Equals( System.Object obj ) {
		if (obj == null)
			return false;
		
		KNVerb kv = obj as KNVerb;
		if (kv.VerbName == VerbName && (kv.Inverted == Inverted))
			return true;
		foreach (var v in Parents) {
			if (v.Equals (kv))
				return true;
		}
		return false;
	}
	public virtual string GetID() {
		string idStr = "";
		if (Inverted)
			idStr += "!";
		idStr += VerbName;
		return idStr;
	}
	public virtual string Convey() {
		return VerbName;
	}
}