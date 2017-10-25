using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNManager : MonoBehaviour {

	public KNDatabase masterDatabase;
	public GameObject listGO;
	CharacterManager cm;
	public string entrySource;
	public string subjectSource;
	public string verbSource;

	Dictionary<string,KNSubject> allSubjects;
	Dictionary<string,KNVerb> allVerbs;

	void Start () {
		masterDatabase = new KNDatabase ();
		allSubjects = new Dictionary<string,KNSubject> ();
		allVerbs = new Dictionary<string,KNVerb> ();
		cm = FindObjectOfType<CharacterManager> ();
		KNImporter.initDatabase (this,entrySource,subjectSource,verbSource);
		Debug.Log ("finished importing database");
	}

	public void outputKnowledgeBox(string characterName) {
		GameObject go = Instantiate (listGO);
		Character c = cm.findChar (characterName);
	}

	public void addKnowledgeGroups(KNDatabase kd, string kGroup) {
		foreach (Assertion a in masterDatabase.knowledge.Values) {
			if (a.knowledgeGroups.Contains (kGroup)) {
				addAssertion (kd, a.copyAssertion());
			}
		}
	}
	public void addAssertion(KNDatabase kd, string nameID) {
		kd.addAssertion(masterDatabase.searchID(nameID));
	}
	public void addAssertion(KNDatabase kd, Assertion a) {
		masterDatabase.addAssertion (a);
		kd.addAssertion (a);
	}
	public KNSubject findOrCreateSubject(string sid) {
		if (allSubjects.ContainsKey (sid)) {
			return allSubjects [sid];
		} else {
			KNSubject newSubject = new KNSubject ();
			newSubject.subjectName = sid;
			if (cm.findChar (sid) != null) {
				newSubject.mChar = cm.findChar (sid);
			}
			allSubjects.Add (sid, newSubject);
			return allSubjects [sid];
		}
	}
	public KNVerb findOrCreateVerb(string vid) {
		if (allVerbs.ContainsKey (vid)) {
			return allVerbs [vid];
		} else {
			KNVerb newVerb = new KNVerb ();
			newVerb.verbName = vid;
			allVerbs.Add (vid, newVerb);
			return allVerbs [vid];
		}
	}

	//----dialogue boxes-----

	public void createDialogueList(Character speaker) {
		createDialogueList (speaker, null,false);
	}

	public void createDialogueList(Character speaker,Character listener,bool ask) {
		GameObject l = GameObject.Instantiate (listGO);
		ListSelection list = l.GetComponent<ListSelection> ();
		list.addOptions (getSubjectOptions (speaker, listener, true,ask));
		l.transform.SetPositionAndRotation (new Vector3 (0f, 0f), Quaternion.identity);
		l.transform.SetParent (FindObjectOfType<GameManager> ().gameObject.transform.Find ("UI"), false);
	}

	void addWildCard(List<DialogueOption> dos){ 
		OptionKnowledgeBase o = new OptionKnowledgeBase ();
		o.text = "Anything?";
		o.responseFunction = finishFact;
		dos.Add (o);
	}

	public List<DialogueOption> getSubjectOptions(Character c,Character listener,bool includeWildcard,bool isInquiry) {
		List<DialogueOption> dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (KNSubject ks in kd.knownSubjects) {
			OptionKnowledgeBase o = new OptionKnowledgeBase ();
			o.speaker = c;
			o.listener = listener;
			o.responseFunction = subjectSelected;
			o.text = ks.subjectName;
			Assertion f = new Assertion ();
			f.timeLearned = Time.realtimeSinceStartup;
			f.lastTimeDiscussed = Time.realtimeSinceStartup;
			f.factSource = findOrCreateSubject (c.name);
			f.inquiry = isInquiry;
			f.subjects.Add (ks);
			o.assertion = f;
			dos.Add (o);
		}
		if (includeWildcard) { addWildCard (dos); }
		return dos;
	}

	public List<DialogueOption> getPossibleReceivers(Character c,Character listener,Assertion a,bool includeWildcard) {
		List<DialogueOption> dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (KNSubject ks in kd.knownSubjects) {
			if (a.verb.canReceive(ks) ) {
				OptionKnowledgeBase o = new OptionKnowledgeBase ();
				o.responseFunction = finishFact;
				o.speaker = c;
				o.listener = listener;
				o.text = ks.subjectName;
				o.assertion = a;
				o.assertion.directObjects.Add (ks);
				dos.Add (o);
			}
		}
		if (includeWildcard) { addWildCard (dos); }
		return dos;
	}

	public List<DialogueOption> getVerbOptions (Character c, Character listener, Assertion a, bool includeWildcard) {
		List<DialogueOption> dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (KNVerb kv in kd.knownVerbs) {
			//Debug.Log ("Found verb: " + kv.verbName + " subName: " + sub.subjectName + " canAct: " + kv.canAct(sub));
			if (kv.canAct (a.subjects[0])) {
				OptionKnowledgeBase o = new OptionKnowledgeBase ();
				o.speaker = c;
				o.listener = listener;
				o.responseFunction = verbSelected;
				o.text = kv.verbName;
				a.verb = kv;
				o.assertion = a;
				dos.Add (o);
			}
		}
		if (includeWildcard) { addWildCard (dos); }
		return dos;
	}
	void subjectSelected(DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;
		Destroy (dob.parentList.gameObject);
		GameObject l = GameObject.Instantiate (listGO);
		ListSelection list = l.GetComponent<ListSelection> ();
		list.addOptions (getVerbOptions (dob.speaker,dob.listener,dob.assertion,true));
		l.transform.SetPositionAndRotation (new Vector3 (0f, 0f), Quaternion.identity);
		l.transform.SetParent (FindObjectOfType<GameManager> ().gameObject.transform.Find ("UI"), false);
		if (dob.listener) {}
	}
	void verbSelected (DialogueOption o) {
		Debug.Log ("Verb is selected: " + o.text);
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;
		Destroy (dob.parentList.gameObject);
		GameObject l = GameObject.Instantiate (listGO);
		ListSelection list = l.GetComponent<ListSelection> ();
		list.addOptions (getPossibleReceivers (dob.speaker,dob.listener,dob.assertion,true));
		l.transform.SetPositionAndRotation (new Vector3 (0f, 0f), Quaternion.identity);
		l.transform.SetParent (FindObjectOfType<GameManager> ().gameObject.transform.Find ("UI"), false);
		if (dob.listener) {}
	}
	void finishFact(DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;
		Destroy (dob.parentList.gameObject);
		if (dob.listener) { dob.listener.knowledgeBase.learnFact (dob.assertion); }
	}

	/*public void parseDatabase(string factID) {
		Assertion a = new Assertion ();
		masterDatabase.addAssertion (a);
		//fullDatabase.Add (factID, newK);
	}*/
}

