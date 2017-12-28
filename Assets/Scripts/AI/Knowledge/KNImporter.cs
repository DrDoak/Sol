using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KNImporter {
	
	public static void InitDatabase(KNManager knm, string entriesSource, string subjectSource, string verbSource) {
		InitVerbs ( verbSource);
		InitSubjects ( subjectSource);
		InitEntries ( entriesSource);
	}

	static void InitSubjects(string source) {
		List<Dictionary<string,string>> subjects = FactCSVImporter.importFile (source);
		foreach (Dictionary<string,string> d in subjects) {
			if (d ["name"].Length == 0)
				continue;
			KNSubject sub = KNManager.GetSubject (d ["name"]);
			sub.Hide = (d ["hide"].Length > 0);
			sub.Exclamation = (d ["exclamation"].Length > 0);
			sub.SubjectDisplayed = (d ["displayed"].Length > 0) ? d ["displayed"] : "none";
			Debug.Log ("name: " + sub.SubjectName + " hide: " + sub.Hide);
			sub = KNManager.CopySubject (d ["name"]);
			Debug.Log ("name: " + sub.SubjectName + " hide: " + sub.Hide);
			List<string> parents = FactCSVImporter.splitStringRow (d ["parent"]);
			if (parents.Count > 0) {
				foreach (string s in parents) {
					KNSubject par = KNManager.CopySubject (s);
					sub.Parents.Add (par);
				}
			}
			List<string> contradictions = FactCSVImporter.splitStringRow (d ["contradictions"]);
			if (parents.Count > 0) {
				foreach (string s in parents) {
					KNSubject con = KNManager.CopySubject (s);
					sub.Contradictions.Add (con);
				}
			}
		}
	}

	static void InitVerbs( string source) {
		List<Dictionary<string,string>> verbs = FactCSVImporter.importFile (source);
		int t = 0;
		foreach (Dictionary<string,string> d in verbs) {
			t += 1;
			if (d ["name"].Length == 0)
				continue;
			KNVerb sub = KNManager.GetVerb (d ["name"]);
			sub.IsCommand = (d ["command"].Length > 0);
			sub.Hide = (d ["hide"].Length > 0);
			sub.VerbDisplayed = (d ["displayed"].Length > 0) ? d ["displayed"] : "none";
			sub = KNManager.CopyVerb (d ["name"]);
			//Debug.Log ("INitializing verbs: " + d ["name"]);
			List<string> actors = FactCSVImporter.splitStringRow (d ["actors"]);
			if (actors.Count > 0) {
				foreach (string s in actors) {
					KNSubject par = KNManager.CopySubject (s);
					sub.Actors.Add (par);
				}
			}
			List<string> receivers = FactCSVImporter.splitStringRow (d ["receivers"]);
			if (receivers.Count > 0) {
				foreach (string s in receivers) {
					KNSubject con = KNManager.CopySubject (s);
					sub.Receivors.Add (con);
				}
			}
		}
	}

	static void InitEntries( string source) {
		List<Dictionary<string,string>> dbEntries = FactCSVImporter.importFile (source);
		foreach (Dictionary<string,string> d in dbEntries) {
			Assertion newEntry = new Assertion ();

			newEntry.Subjects = ParseSubjectEntry (d ["subjects"]);
			newEntry.Verb = KNManager.CopyVerb (d ["verb"]);
			newEntry.Receivors = ParseSubjectEntry (d ["directObjects"]);
			newEntry.KnowledgeGroups = FactCSVImporter.splitStringRow (d ["knowledgeGroups"]);
			KNManager.Instance.AddAssertion (newEntry);
		}
	}


	static List<KNSubject> ParseSubjectEntry (string subjectStr) {
		List<KNSubject> ret = new List<KNSubject> ();
		string lastWord = "";
		foreach (char lastC in subjectStr.ToCharArray()) {
			if (lastC == ';') {
				ret.Add (KNManager.CopySubject (lastWord));
				lastWord = "";
			} else if (lastC != ' ') {
				lastWord += lastC;
			}
		}
		ret.Add (KNManager.CopySubject (lastWord));
		return ret;
	}

	static void ParseKnowledgeGroup ( Assertion a, string groupID) {
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
