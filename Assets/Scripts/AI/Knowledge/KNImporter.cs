using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KNImporter {
	
	public static void InitDatabase(KNManager knm, string entriesSource, string subjectSource, string verbSource) {
		InitVerbs (knm, verbSource);
		InitSubjects (knm, subjectSource);
		InitEntries (knm, entriesSource);
	}

	static void InitSubjects(KNManager knm, string source) {
		List<Dictionary<string,string>> subjects = FactCSVImporter.importFile (source);
		foreach (Dictionary<string,string> d in subjects) {
			KNSubject sub = knm.FindOrCreateSubject (d ["name"]);
			List<string> parents = FactCSVImporter.splitStringRow (d ["parent"]);
			if (parents.Count > 0) {
				foreach (string s in parents) {
					KNSubject par = knm.FindOrCreateSubject (s);
					sub.Parents.Add (par);
				}
			}
			List<string> contradictions = FactCSVImporter.splitStringRow (d ["contradictions"]);
			if (parents.Count > 0) {
				foreach (string s in parents) {
					KNSubject con = knm.FindOrCreateSubject (s);
					sub.Contradictions.Add (con);
				}
			}
		}
	}

	static void InitVerbs(KNManager knm, string source) {
		List<Dictionary<string,string>> verbs = FactCSVImporter.importFile (source);
		int t = 0;
		foreach (Dictionary<string,string> d in verbs) {
			t += 1;
			KNVerb sub = knm.FindOrCreateVerb (d ["name"]);
			List<string> actors = FactCSVImporter.splitStringRow (d ["actors"]);
			if (actors.Count > 0) {
				foreach (string s in actors) {
					KNSubject par = knm.FindOrCreateSubject (s);
					sub.Actors.Add (par);
				}
			}
			List<string> receivers = FactCSVImporter.splitStringRow (d ["receivers"]);
			if (receivers.Count > 0) {
				foreach (string s in receivers) {
					KNSubject con = knm.FindOrCreateSubject (s);
					sub.Receivors.Add (con);
				}
			}
		}
	}

	static void InitEntries(KNManager knm, string source) {
		List<Dictionary<string,string>> dbEntries = FactCSVImporter.importFile (source);
		foreach (Dictionary<string,string> d in dbEntries) {
			Assertion newEntry = new Assertion ();

			newEntry.Subjects = ParseSubjectEntry (knm,d ["subjects"]);
			newEntry.Verb = knm.FindOrCreateVerb (d ["verb"]);
			newEntry.Receivors = ParseSubjectEntry (knm,d ["directObjects"]);
			newEntry.KnowledgeGroups = FactCSVImporter.splitStringRow (d ["knowledgeGroups"]);
			knm.AddAssertion (newEntry);
		}
	}


	static List<KNSubject> ParseSubjectEntry (KNManager knm, string subjectStr) {
		List<KNSubject> ret = new List<KNSubject> ();
		string lastWord = "";
		foreach (char lastC in subjectStr.ToCharArray()) {
			if (lastC == ';') {
				ret.Add (knm.FindOrCreateSubject (lastWord));
				lastWord = "";
			} else if (lastC != ' ') {
				lastWord += lastC;
			}
		}
		ret.Add (knm.FindOrCreateSubject (lastWord));
		return ret;
	}

	static void ParseKnowledgeGroup(KNManager knm, Assertion a, string groupID) {
		string lastWord = "";
		foreach (char lastC in groupID) {
			if (lastC == ';') {
				a.KnowledgeGroups.Add (lastWord);
				lastWord = "";
			} else if (lastC != ' ') {
				lastWord += lastC;
			}
		}
		KNSubject subj = new KNSubject ();
		a.Receivors.Add (subj);
	}
}
