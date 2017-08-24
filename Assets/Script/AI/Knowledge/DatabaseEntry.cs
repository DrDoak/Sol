using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseEntry {
	public KNSubject subject;
	public KNVerb verb;
	public string category;
	public List<string> factGroups;
	public string info;

	public Fact toFact() {
		Fact newF = new Fact ();
		newF.factID = factID;
		newF.category = category;
		newF.factGroups = factGroups;
		newF.info = info;
		return newF;
	}
}
