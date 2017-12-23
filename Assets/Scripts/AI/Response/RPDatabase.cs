using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPDatabase : MonoBehaviour {

	[SerializeField] private string ImportPath;
	List<RPTemplate> m_ResponseTemplates;
	List<RPTemplate> m_SubjectTemplates;
	List<RPTemplate> m_VerbTemplates;

	KNManager knm;
	bool m_Init;
	// Use this for initialization
	void Start () {
		knm = FindObjectOfType<KNManager> ();
		m_ResponseTemplates = new List<RPTemplate> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_Init)
			return;
		ImportFromFile (ImportPath);
		m_Init = true;
	}

	public List<RPTemplate> GetMatches(Assertion a,Character speaker) {
		var responses = new List<RPTemplate> ();
		foreach (var r in m_ResponseTemplates) {
			r.setSpeaker (speaker);
			if (r.match (a))
				responses.Add (r);
		}
		return responses;
	}
	public List<RPTemplate> GetMatches(KNSubject s,Character speaker) {
		var responses = new List<RPTemplate> ();
		foreach (var r in m_SubjectTemplates) {
			r.setSpeaker (speaker);
			if (r.match (s))
				responses.Add (r);
		}
		return responses;
	}
	public List<RPTemplate> GetMatches(KNVerb v,Character speaker) {
		var responses = new List<RPTemplate> ();
		foreach (var r in m_VerbTemplates) {
			r.setSpeaker (speaker);
			if (r.match (v))
				responses.Add (r);
		}
		return responses;
	}

	public void ImportFromFile(string path) {
		List<Dictionary<string,string>> templates = FactCSVImporter.importFile (path);
		foreach (Dictionary<string,string> d in templates) {
			RPTemplate rpt = new RPTemplate ();
			string t = d ["type"];
			rpt.speechGroup = d ["group"];
			rpt.template = d ["template"];
			if (t == "assertion") {
				rpt.templateAssertion = new Assertion ();
				if (d ["subjects"] != "*") {
					List<string> subjects = FactCSVImporter.splitStringRow (d ["subjects"]);
					if (subjects.Count > 0) {
						foreach (string s in subjects) {
							//Debug.Log ("searching for subject: " + s);
							//KNSubSelf kss = (KNSubSelf)knm.FindOrCreateSubject (s);
							KNSubject par = knm.FindOrCreateSubject (s);
							//Debug.Log ("ID is: " + par.getID ());
							rpt.templateAssertion.AddSubject (par);
						}
					}
				}
				if (d ["verb"] != "*") {
					KNVerb verb = knm.FindOrCreateVerb (d["verb"]);
					rpt.templateAssertion.Verb = verb;
				}
				if (d ["receivers"] != "*") {
					List<string> dobjs = FactCSVImporter.splitStringRow (d ["receivers"]);
					if (dobjs.Count > 0) {
						foreach (string s in dobjs) {
							KNSubject par = knm.FindOrCreateSubject (s);
							rpt.templateAssertion.AddReceivor (par);
						}
					}
				}
			} else {}
			m_ResponseTemplates.Add(rpt);
		}
	}
}
