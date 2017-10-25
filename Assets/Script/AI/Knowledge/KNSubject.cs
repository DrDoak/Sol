using System.Collections;
using System.Collections.Generic;

public class KNSubject {
	public string subjectName;
	public List<KNSubject> parentGroups;
	public List<KNSubject> contradictoryGroups;
	public Character mChar;
	public KNSubject() {
		parentGroups = new List<KNSubject> ();
		contradictoryGroups = new List<KNSubject> ();
	}
	public bool match(KNSubject ks) {
		if (ks.subjectName == subjectName) {
			return true;
		}
		foreach (KNSubject s in parentGroups) {
			if (s.match (ks)) {
				return true;
			}
		}
		return false;
	}
}
