using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNVerb {
	public string VerbName;
	public bool Inverted;
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
			Inverted = Inverted};
		return v;
	}
	public bool CanAct(KNSubject a) {
		if (a == null)
			return true;
		foreach (var s in Actors) {
			if (a.Match (s))
				return true;
		}
		return false;
	}
	public bool CanReceive(KNSubject receiver) {
		if (receiver == null)
			return true;
		foreach (var s in Receivors) {
			if (receiver.Match (s))
				return true;
		}
		return false;
	}
	public bool Match(KNVerb kv) {
		if (kv.VerbName == VerbName)
			return true;
		
		foreach (var v in Parents) {
			if (v.Match (kv))
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