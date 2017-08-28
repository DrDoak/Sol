using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fact : DatabaseEntry {
	public List<string> factGroups;
	public string info;
	float assuration;
	float support;
	public List<KNSubject> subjects;
	public KNVerb verb;
	public List<KNSubject> directObjects;
	public Fact() {
		subjects = new List<KNSubject> ();
		directObjects = new List<KNSubject> ();
	}
}
