using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNManager : MonoBehaviour {

	public static KNManager Instance;

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
				AddAssertion (kd, a.CopyAssertion());
			}
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
	public KNSubject FindOrCreateSubject(string sid) {
		if (m_Subjects.ContainsKey (sid)) {
			KNSubject ks = m_Subjects [sid].Copy ();
			return ks;
		} else {
			var newSubject = new KNSubject { SubjectName = sid };
			if (cm.findChar (sid) != null)
				newSubject.Owner = cm.findChar (sid);
			
			m_Subjects.Add (sid, newSubject);
			KNSubject ks = m_Subjects [sid].Copy ();
			return ks;
		}
	}
	public KNVerb FindVerb(string vid) {
		if (m_Verbs.ContainsKey(vid) )
			return m_Verbs[vid];
		return null;
	}
	public KNVerb FindOrCreateVerb(string vid) {
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

	public Assertion NewAssertion(KNSubject subject, KNVerb verb, KNSubject receiver, KNSubject source,bool isInquiry,Character c) {
		var f = new Assertion { TimeLearned = Time.realtimeSinceStartup,Inquiry = isInquiry,
			LastTimeDiscussed = Time.realtimeSinceStartup, Source = source};
		f.SetOwner (c);
		f.AddSubject (subject);
		f.AddVerb (verb);
		f.AddReceivor (receiver);
		return f;
	}

//----dialogue boxes-----
	public void CreateSubjectList(Character speaker) {
		CreateSubjectList (speaker, null,false);
	}

	public void CreateSubjectList(Character speaker,Character listener,bool isInquiry) {
		var du = new DialogueUnit {speaker = speaker, listener = listener};
		du.addDialogueOptions (GetSubjectOptions (speaker, true, isInquiry));
		listener.processDialogueRequest (speaker, du);
		du.startSequence ();
	}

	void AddWildCard(List<DialogueOption> dos,Assertion a){ 
		var o = new OptionKnowledgeBase {text = "Anything?",
			assertion = a, responseFunction = FinishFact};
		dos.Add (o);
	}

	public List<DialogueOption> GetSubjectOptions(Character c,bool includeWildcard,bool isInquiry) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (var ks in kd.Subjects) {
			OptionKnowledgeBase o = new OptionKnowledgeBase {responseFunction = SubjectSelected,
				text = ks.SubjectName};
			o.assertion = NewAssertion(ks,null,null,FindOrCreateSubject(c.name),isInquiry,c);
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, NewAssertion(null,null,null,FindOrCreateSubject(c.name),isInquiry,c)); }
		return dos;
	}
	public List<DialogueOption> GetVerbOptions (Character c, Assertion a, bool includeWildcard) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (var kv in kd.MatchingVerbs(a)) {
			//Debug.Log ("Found verb: " + kv.VerbName + " subName: " + sub.subjectName + " canAct: " + kv.canAct(sub));
			var o = new OptionKnowledgeBase { responseFunction = VerbSelected,
				text = kv.VerbName, assertion = a.CopyAssertion()};
			o.assertion.AddVerb( kv);
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, a.CopyAssertion()); }
		return dos;
	}

	public List<DialogueOption> getPossibleReceivers(Character c, Assertion a, bool includeWildcard) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (var ks in kd.MatchingDirectObjects(a)) {
			var o = new OptionKnowledgeBase {responseFunction = FinishFact, text = ks.SubjectName,
				assertion = a.CopyAssertion()};
			o.assertion.AddReceivor(ks);
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, a); }
		return dos;
	}

	void SubjectSelected(DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;

		var du = new DialogueUnit {speaker = dob.speaker, listener = dob.listener};

		du.addDialogueOptions (GetVerbOptions(dob.speaker,dob.assertion,true));
		o.closeSequence();
		du.startSequence ();
	}
	void VerbSelected (DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;

		DialogueUnit du = new DialogueUnit {speaker = dob.speaker, listener = dob.listener};
		du.addDialogueOptions (getPossibleReceivers(dob.speaker,dob.assertion,true));
		o.closeSequence();
		du.startSequence ();
	}
	void FinishFact(DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;
		o.closeSequence ();
		if (dob.assertion != null && dob.listener)
			dob.listener.knowledgeBase.LearnFact (dob.assertion);
	}
}

