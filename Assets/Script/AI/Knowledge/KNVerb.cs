using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNVerb {
	public string verbName;
	public List<KNSubject> actor;
	public List<KNSubject> receivor;
	public List<KNVerb> parentGroups;
	public List<KNVerb> contradictoryGroups;

	public KNVerb() {
		parentGroups = new List<KNVerb> ();
		contradictoryGroups = new List<KNVerb> ();
		actor = new List<KNSubject> ();
		receivor = new List<KNSubject> ();
	}
	public bool canAct(KNSubject a) {
		foreach (KNSubject s in actor) {
			if (a.match (s)) {
				return true;
			}
		}
		return false;
	}
	public bool canReceive(KNSubject receiver) {
		foreach (KNSubject s in receivor) {
			if (receiver.match (s)) {
				return true;
			}
		}
		return false;
	}
	public bool match(KNVerb kv) {
		if (kv.verbName == verbName) {
			return true;
		}
		foreach (KNVerb v in parentGroups) {
			if (v.match (kv)) {
				return true;
			}
		}
		return false;
	}
}