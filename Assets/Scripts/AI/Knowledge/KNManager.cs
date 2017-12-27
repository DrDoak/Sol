using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNManager : MonoBehaviour {

	public static KNManager Instance;
	public delegate void OnSelection(DialogueOption doption);

	[SerializeField] private string m_EntrySource;
	[SerializeField] private string m_SubjectSource;
	[SerializeField] private string m_VerbSource;

	[SerializeField] private GameObject m_List;

	CharacterManager cm;
	KNDatabase m_Database;

	Dictionary<string,KNSubject> m_Subjects;
	Dictionary<string,KNVerb> m_Verbs;

	void Awake() {
		if (Instance == null)
			Instance = this;
		m_Database = new KNDatabase ();
		m_Subjects = new Dictionary<string,KNSubject> ();
		m_Verbs = new Dictionary<string,KNVerb> ();
	}
	void Start () {		
		cm = CharacterManager.Instance;
		m_Subjects ["self"] = new KNSubSelf (); //TEST FOR KNSUBSELF
		KNImporter.InitDatabase (this,m_EntrySource,m_SubjectSource,m_VerbSource);
	}
		
	public void AddKnowledgeGroups(KNDatabase kd, string kGroup) {
		
		foreach (Assertion a in m_Database.Knowledge.Values) {
			if (a.KnowledgeGroups.Contains (kGroup)) {
				kd.AddAssertion (a.CopyAssertion ());
				//AddAssertion (kd, a.CopyAssertion());
			}
		}
		foreach (KNVerb v in m_Verbs.Values) {
			kd.LearnVerb (v);
		}
		foreach (KNSubject s in m_Subjects.Values) {
			kd.LearnSubject (s);
		}
	}
	public void AddAssertion(Assertion a) {
		m_Database.AddAssertion (a);
	}
	public void AddAssertion(KNDatabase kd, string nameID) {
		kd.AddAssertion(m_Database.GetAssertion(nameID));
	}
	public void AddAssertion(KNDatabase kd, Assertion a) {
		m_Database.AddAssertion (a);
		kd.AddAssertion (a);
	}
	public void SetSubject(string sid, KNSubject subject) {
		m_Subjects [sid] = subject;
	}
	public static KNSubject FindOrCreateSubject(string sid, bool hide, bool exclamation) {
		return Instance.m_FindOrCreateSubject (sid, hide, exclamation);		
	}

	public static KNSubject FindOrCreateSubject(string sid) {
		return Instance.m_FindOrCreateSubject (sid, false, false);
	}

	KNSubject m_FindOrCreateSubject(string sid, bool hide, bool exclamation) {
		string s = sid.ToLower ();
		if (m_Subjects.ContainsKey (s)) {
			KNSubject ks = m_Subjects [s].Copy ();
			return ks;
		} else {
			var newSubject = new KNSubject { SubjectName = sid };
			if (CharacterManager.Instance.findChar (sid) != null)
				newSubject.Owner = CharacterManager.Instance.findChar (sid);
			newSubject.Hide = hide;
			newSubject.Exclamation = exclamation;
			m_Subjects.Add (s, newSubject);
			KNSubject ks = m_Subjects [s].Copy ();
			return ks;
		}
	}
	public KNVerb FindVerb(string vid) {
		if (m_Verbs.ContainsKey(vid) )
			return m_Verbs[vid];
		return null;
	}
	public static KNVerb FindOrCreateVerb(string vid) {
		return Instance.m_FindOrCreateVerb(vid);
	}

	KNVerb m_FindOrCreateVerb(string vid) {
		var invert = false;
		if (vid [0] == '!') {
			invert = true;
			vid = vid.Substring (1);
		}
		if (m_Verbs.ContainsKey (vid)) {
			KNVerb v = m_Verbs [vid].Copy();
			v.Inverted = invert;
			return v;
		} else {
			var newVerb = new KNVerb {VerbName = vid};
			m_Verbs.Add (vid, newVerb);
			KNVerb v = m_Verbs [vid].Copy ();
			v.Inverted = invert;
			return v;
		}
	}
	Assertion m_newAssertion(KNSubject subject, KNVerb verb, KNSubject receiver, KNSubject source,bool isInquiry,Character c) {
		var f = new Assertion { TimeLearned = GameManager.GameTime,Inquiry = isInquiry,
			LastTimeDiscussed = GameManager.GameTime, Source = source};
		f.SetOwner (c);
		f.AddSubject (subject);
		f.AddVerb (verb);
		f.AddReceivor (receiver);
		return f;
	}

//----dialogue boxes-----
	public static void CreateSubjectList(Character speaker) {
		CreateSubjectList (speaker, null, Instance.FinishFact);
	}

	public static void CreateSubjectList(Character speaker, Character listener) {
		CreateSubjectList (speaker, listener, Instance.FinishFact);
	}
	public static void CreateSubjectList(Character speaker,Character listener,OnSelection selectionFunction) {
		var du = new DialogueUnit {speaker = speaker, listener = listener};
		du.addDialogueOptions (Instance.GetSubjectOptions (speaker, true, selectionFunction));
		listener.processDialogueRequest (speaker, du);
		du.startSequence ();
	}

	void AddWildCard(List<DialogueOption> dos,Assertion a){ 
		var o = new OptionKnowledgeBase {text = "Anything?",
			assertion = a, responseFunction = FinishFact};
		dos.Add (o);
	}

	public List<DialogueOption> GetSubjectOptions(Character c,bool includeWildcard,OnSelection selectionFunction) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (var ks in kd.Subjects) {
			if (ks.Hide)
				continue;
			OptionKnowledgeBase o = new OptionKnowledgeBase {responseFunction = SubjectSelected,
				text = ks.SubjectName};
			o.assertion = m_newAssertion (ks, null, null, FindOrCreateSubject (c.name), false, c);
			o.SelectionFunction = selectionFunction;
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, m_newAssertion(null,null,null,FindOrCreateSubject(c.name),false,c)); }
		return dos;
	}

	public List<DialogueOption> GetVerbOptions (Character c, Assertion a, bool includeWildcard, OnSelection selectionFunction) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;

		foreach (var kv in kd.MatchingVerbs(a)) {
			//Debug.Log ("Found verb: " + kv.VerbName + " subName: " + sub.subjectName + " canAct: " + kv.canAct(sub));
			var o = new OptionKnowledgeBase { responseFunction = VerbSelected,
				text = kv.VerbName, assertion = a.CopyAssertion()};
			o.assertion.AddVerb( kv);
			o.SelectionFunction = selectionFunction;
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, a.CopyAssertion()); }
		return dos;
	}

	public List<DialogueOption> getPossibleReceivers(Character c, Assertion a, bool includeWildcard, OnSelection selectionFunction) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (var ks in kd.MatchingDirectObjects(a)) {
			if (ks.Hide)
				continue;
			var o = new OptionKnowledgeBase {responseFunction = FinishFact, text = ks.SubjectName,
				assertion = a.CopyAssertion()};
			o.assertion.AddReceivor(ks);
			o.SelectionFunction = selectionFunction;
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, a); }
		return dos;
	}

	void SubjectSelected(DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;

		var du = new DialogueUnit {speaker = dob.speaker, listener = dob.listener};

		du.addDialogueOptions (GetVerbOptions(dob.speaker,dob.assertion,true,dob.SelectionFunction));
		o.closeSequence();
		du.startSequence ();
	}

	void VerbSelected (DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;

		DialogueUnit du = new DialogueUnit {speaker = dob.speaker, listener = dob.listener};
		du.addDialogueOptions (getPossibleReceivers(dob.speaker,dob.assertion,true, dob.SelectionFunction));
		o.closeSequence();
		du.startSequence ();
	}

	void FinishFact(DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;
		o.closeSequence ();
		if (dob.assertion != null && dob.listener)
			dob.listener.knowledgeBase.LearnAssertion (dob.assertion);
	}

	public static void CreateExclamationList(Character speaker, Character listener) {
		CreateExclamationList (speaker, listener, Instance.FinishFact);
	}
	public static void CreateExclamationList(Character speaker,Character listener,OnSelection selectionFunction) {
		var du = new DialogueUnit {speaker = speaker, listener = listener};
		du.addDialogueOptions (Instance.GetExclamations (speaker, true, selectionFunction));
		listener.processDialogueRequest (speaker, du);
		du.startSequence ();
	}

	public List<DialogueOption> GetExclamations(Character c,bool includeWildcard,OnSelection selectionFunction) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (var ks in kd.Subjects) {
			if (!ks.Exclamation)
				continue;
			OptionKnowledgeBase o = new OptionKnowledgeBase {responseFunction = FinishFact,
				text = ks.SubjectName};
			o.assertion = m_newAssertion(ks,null,null,FindOrCreateSubject(c.name),false,c);
			o.SelectionFunction = selectionFunction;
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, m_newAssertion(null,null,null,FindOrCreateSubject(c.name),false,c)); }
		return dos;
	}
}

