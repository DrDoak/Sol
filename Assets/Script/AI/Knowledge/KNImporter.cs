using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KNImporter {
	
	public static void initDatabase(KNManager knm, string entriesSource, string subjectSource, string verbSource) {
		initVerbs (knm, verbSource);
		initSubjects (knm, subjectSource);
		initEntries (knm, entriesSource);
	}

	static void initSubjects(KNManager knm, string source) {
		List<Dictionary<string,string>> subjects = FactCSVImporter.importFile (source);
		foreach (Dictionary<string,string> d in subjects) {
			KNSubject sub = knm.findOrCreateSubject (d ["name"]);
			List<string> parents = FactCSVImporter.splitStringRow (d ["parent"]);
			if (parents.Count > 0) {
				foreach (string s in parents) {
					KNSubject par = knm.findOrCreateSubject (s);
					sub.parentGroups.Add (par);
				}
			}
			List<string> contradictions = FactCSVImporter.splitStringRow (d ["contradictions"]);
			if (parents.Count > 0) {
				foreach (string s in parents) {
					KNSubject con = knm.findOrCreateSubject (s);
					sub.contradictoryGroups.Add (con);
				}
			}
		}
	}

	static void initVerbs(KNManager knm, string source) {
		List<Dictionary<string,string>> verbs = FactCSVImporter.importFile (source);
		int t = 0;
		foreach (Dictionary<string,string> d in verbs) {
			t += 1;
			KNVerb sub = knm.findOrCreateVerb (d ["name"]);
			List<string> actors = FactCSVImporter.splitStringRow (d ["actors"]);
			if (actors.Count > 0) {
				foreach (string s in actors) {
					KNSubject par = knm.findOrCreateSubject (s);
					sub.actor.Add (par);
				}
			}
			List<string> receivers = FactCSVImporter.splitStringRow (d ["receivers"]);
			if (receivers.Count > 0) {
				foreach (string s in receivers) {
					KNSubject con = knm.findOrCreateSubject (s);
					sub.receivor.Add (con);
				}
			}
		}
	}

	static void initEntries(KNManager knm, string source) {
		List<Dictionary<string,string>> dbEntries = FactCSVImporter.importFile (source);
		foreach (Dictionary<string,string> d in dbEntries) {
			Assertion newEntry = new Assertion ();

			//newEntry.assertionType = d ["category"];
			newEntry.info = d ["info"];
			newEntry.subjects = parseSubjectEntry (knm,d ["subjects"]);
			newEntry.verb = knm.findOrCreateVerb (d ["verb"]);
			newEntry.directObjects = parseSubjectEntry (knm,d ["directObjects"]);
			newEntry.knowledgeGroups = FactCSVImporter.splitStringRow (d ["knowledgeGroups"]);
			knm.masterDatabase.addAssertion (newEntry);
		}
	}


	static List<KNSubject> parseSubjectEntry (KNManager knm, string subjectStr) {
		List<KNSubject> ret = new List<KNSubject> ();
		string lastWord = "";
		foreach (char lastC in subjectStr.ToCharArray()) {
			if (lastC == ';') {
				ret.Add (knm.findOrCreateSubject (lastWord));
				lastWord = "";
			} else if (lastC != ' ') {
				lastWord += lastC;
			}
		}
		ret.Add (knm.findOrCreateSubject (lastWord));
		return ret;
	}

	static void parseKnowledgeGroup(KNManager knm, Assertion a, string groupID) {
		string lastWord = "";
		foreach (char lastC in groupID) {
			if (lastC == ';') {
				a.knowledgeGroups.Add (lastWord);
				lastWord = "";
			} else if (lastC != ' ') {
				lastWord += lastC;
			}
		}
		KNSubject subj = new KNSubject ();
		a.directObjects.Add (subj);
	}
}
