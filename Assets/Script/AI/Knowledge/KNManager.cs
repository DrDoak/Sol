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

	public Assertion newAssertion(KNSubject subject, KNVerb verb, KNSubject receiver, KNSubject source,bool isInquiry) {
		Assertion f = new Assertion ();
		f.timeLearned = Time.realtimeSinceStartup;
		f.lastTimeDiscussed = Time.realtimeSinceStartup;
		f.factSource = source;
		f.inquiry = isInquiry;
		if (subject != null)
			f.subjects.Add (subject);
		f.verb = verb;
		if (receiver != null)
			f.directObjects.Add(receiver);
		return f;
	}
	//----dialogue boxes-----

	public void createDialogueList(Character speaker) {
		createDialogueList (speaker, null,false);
	}

	public void createDialogueList(Character speaker,Character listener,bool isInquiry) {
		DialogueUnit du = new DialogueUnit ();
		du.speaker = speaker;
		du.listener = listener;
		du.addDialogueOptions (getSubjectOptions (speaker, true, isInquiry));
		//p.mEvent.targetChar.processDialogueRequest (mChar,du);
		du.startSequence ();
	}

	void addWildCard(List<DialogueOption> dos,Assertion a){ 
		OptionKnowledgeBase o = new OptionKnowledgeBase ();
		o.text = "Anything?";
		o.assertion = a;
		o.responseFunction = finishFact;
		dos.Add (o);
	}

	public List<DialogueOption> getSubjectOptions(Character c,bool includeWildcard,bool isInquiry) {
		List<DialogueOption> dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (KNSubject ks in kd.knownSubjects) {
			OptionKnowledgeBase o = new OptionKnowledgeBase ();
			o.responseFunction = subjectSelected;
			o.text = ks.subjectName;
			o.assertion = newAssertion(ks,null,null,findOrCreateSubject(c.name),isInquiry);
			dos.Add (o);
		}
		if (includeWildcard) { addWildCard (dos, null); }
		return dos;
	}
	public List<DialogueOption> getVerbOptions (Character c, Assertion a, bool includeWildcard) {
		List<DialogueOption> dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (KNVerb kv in kd.matchingVerbs(a)) {
			//Debug.Log ("Found verb: " + kv.verbName + " subName: " + sub.subjectName + " canAct: " + kv.canAct(sub));
			OptionKnowledgeBase o = new OptionKnowledgeBase ();
			o.responseFunction = verbSelected;
			o.text = kv.verbName;
			o.assertion = a.copyAssertion();
			o.assertion.verb = kv;
			dos.Add (o);
		}
		if (includeWildcard) { addWildCard (dos, a.copyAssertion()); }
		return dos;
	}

	public List<DialogueOption> getPossibleReceivers(Character c, Assertion a, bool includeWildcard) {
		List<DialogueOption> dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (KNSubject ks in kd.matchingDirectObjects(a)) {
			OptionKnowledgeBase o = new OptionKnowledgeBase ();
			o.responseFunction = finishFact;
			o.text = ks.subjectName;
			o.assertion = a.copyAssertion();
			o.assertion.directObjects.Add (ks);
			dos.Add (o);
		}
		if (includeWildcard) { addWildCard (dos, a); }
		return dos;
	}

	void subjectSelected(DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;

		DialogueUnit du = new DialogueUnit ();
		du.speaker = dob.speaker;
		du.listener = dob.listener;
		du.addDialogueOptions (getVerbOptions(dob.speaker,dob.assertion,true));
		//p.mEvent.targetChar.processDialogueRequest (mChar,du);
		Destroy (dob.parentList.gameObject);
		du.startSequence ();
	}
	void verbSelected (DialogueOption o) {
		//Debug.Log ("Verb is selected: " + o.text);
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;

		DialogueUnit du = new DialogueUnit ();
		du.speaker = dob.speaker;
		du.listener = dob.listener;
		du.addDialogueOptions (getPossibleReceivers(dob.speaker,dob.assertion,true));
		//p.mEvent.targetChar.processDialogueRequest (mChar,du);
		Destroy (dob.parentList.gameObject);
		du.startSequence ();
	}
	void finishFact(DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;
		Destroy (dob.parentList.gameObject);
		if (dob.assertion != null && dob.listener)
			dob.listener.knowledgeBase.learnFact (dob.assertion);
	}

	/*public void parseDatabase(string factID) {
		Assertion a = new Assertion ();
		masterDatabase.addAssertion (a);
		//fullDatabase.Add (factID, newK);
	}*/
}

