using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseEntry {
	public string category;
	public List<string> factGroups;
	public string info;

	public List<KNSubject> subjects;
	public KNVerb verb;
	public List<KNSubject> directObjects;

	public Fact toFact() {
		Fact newF = new Fact ();
		newF.category = category;
		newF.factGroups = factGroups;
		foreach (KNSubject item in subjects) {
			newF.subjects.Add (item);
		}
		foreach (KNSubject item in directObjects) {
			newF.directObjects.Add (item);
		}
		newF.info = info;
		return newF;
	}
}
