using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPDatabase : MonoBehaviour {

	public static RPDatabase Instance;

	[SerializeField] private string ImportPath;
	List<RPTemplate> m_ResponseTemplates;
	List<RPTemplate> m_SubjectTemplates;
	List<RPTemplate> m_VerbTemplates;

	//KNManager knm;
	bool m_Init;
	// Use this for initialization
	void Start () {
		if (Instance == null)
			Instance = this;
		//knm = FindObjectOfType<KNManager> ();
		m_SubjectTemplates = new List<RPTemplate> ();
		m_ResponseTemplates = new List<RPTemplate> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("RT: " + m_ResponseTemplates);
		if (m_Init)
			return;
		ImportFromFile (ImportPath);
		m_Init = true;
	}

	public static List<RPTemplate> GetMatches(string exclamation, Character speaker) {
		var responses = new List<RPTemplate> ();
		foreach (var r in Instance.m_ResponseTemplates) {
			r.setSpeaker (speaker);
			if (r.match (exclamation))
				responses.Add (r);
		}
		return responses;
	}

	public static List<RPTemplate> GetMatches(Assertion a,Character speaker) {
		var responses = new List<RPTemplate> ();
		foreach (var r in Instance.m_ResponseTemplates) {
			r.setSpeaker (speaker);
			if (r.match (a))
				responses.Add (r);
		}
		return responses;
	}
	public static List<RPTemplate> GetMatches(KNSubject s,Character speaker) {
		var responses = new List<RPTemplate> ();
		foreach (var r in Instance.m_SubjectTemplates) {
			r.setSpeaker (speaker);
			if (r.match (s))
				responses.Add (r);
		}
		return responses;
	}
	public static List<RPTemplate> GetMatches(KNVerb v,Character speaker) {
		var responses = new List<RPTemplate> ();
		foreach (var r in Instance.m_VerbTemplates) {
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
			rpt.OutputTemplate = d ["template"];
			if (t == "assertion") {
				rpt.templateAssertion = new Assertion ();
				if (d ["subjects"] != "*") {
					List<string> subjects = FactCSVImporter.splitStringRow (d ["subjects"]);
					if (subjects.Count > 0) {
						foreach (string s in subjects) {
							//Debug.Log ("searching for subject: " + s);
							//KNSubSelf kss = (KNSubSelf)knm.CopySubject (s);
							KNSubject par = KNManager.CopySubject (s);
							//Debug.Log ("ID is: " + par.getID ());
							rpt.templateAssertion.AddSubject (par);
						}
					}
				}
				if (d ["verb"] != "*") {
					KNVerb verb = KNManager.CopyVerb (d["verb"]);
					rpt.templateAssertion.Verb = verb;
				}
				if (d ["receivers"] != "*") {
					List<string> dobjs = FactCSVImporter.splitStringRow (d ["receivers"]);
					if (dobjs.Count > 0) {
						foreach (string s in dobjs) {
							KNSubject par = KNManager.CopySubject (s);
							rpt.templateAssertion.AddReceivor (par);
						}
					}
				}
			} else if (t == "exclamation") {
				rpt.templateAssertion = new Assertion ();
				List<string> subjects = FactCSVImporter.splitStringRow (d ["subjects"]);
				if (subjects.Count > 0) {
					foreach (string s in subjects) {
						KNSubject par = KNManager.GetSubject (s);
						par.Exclamation = true;
						rpt.templateAssertion.AddSubject (par.Copy());
					}
				}
			}
			m_ResponseTemplates.Add(rpt);
		}
	}
}
