using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KNManager : MonoBehaviour {

	public static KNManager Instance;
	//public delegate void OnSelection(DialogueOption doption);

	[SerializeField] private string m_EntrySource;
	[SerializeField] private string m_SubjectSource;
	[SerializeField] private string m_VerbSource;

	[SerializeField] private GameObject m_List;

	KNDatabase m_Database;

	Dictionary<string,KNSubject> m_Subjects;
	Dictionary<string,KNVerb> m_Verbs;
	public bool DatabaseInitialized;

	void Awake() {
		if (Instance == null)
			Instance = this;
		m_Database = new KNDatabase ();
		m_Subjects = new Dictionary<string,KNSubject> ();
		m_Verbs = new Dictionary<string,KNVerb> ();
		m_Subjects ["self"] = new KNSubSelf (); //TEST FOR KNSUBSELf
	}
	void Start () {
		if (!DatabaseInitialized) {
			InitDatabase ();
		}
	}
	public void InitDatabase() {
		DatabaseInitialized = true;
		KNImporter.InitDatabase (this, m_EntrySource, m_SubjectSource, m_VerbSource);
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

	public static KNSubject CopySubject(string sid) {
		/*string s = sid.ToLower ();
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
		}*/
		KNSubject ks = KNManager.GetSubject (sid);
		return ks.Copy ();
	}

	public static KNSubject GetSubject(string sid) {
		string s = sid.ToLower ();
		if (Instance.m_Subjects.ContainsKey (s)) {
			return Instance.m_Subjects [s];
		} else {
			var newSubject = new KNSubject{ SubjectName = sid };
			if (CharacterManager.FindChar (sid) != null)
				newSubject.Owner = CharacterManager.FindChar (sid);
			Instance.m_Subjects.Add (s, newSubject);
			return Instance.m_Subjects [s];
		}
	}

	public static KNVerb CopyVerb(string vid) {
		return Instance.m_CopyVerb(vid);
	}

	KNVerb m_CopyVerb(string vid) {
		return GetVerb (vid).Copy ();
	}

	public static KNVerb GetVerb(string vid) {
		var invert = false;
		if (vid [0] == '!') {
			invert = true;
			vid = vid.Substring (1);
		}
		if (Instance.m_Verbs.ContainsKey (vid)) {
			KNVerb v = Instance.m_Verbs [vid];
			v.Inverted = invert;
			return v;
		} else {
			var newVerb = new KNVerb {VerbName = vid};
			Instance.m_Verbs.Add (vid, newVerb);
			KNVerb v = Instance.m_Verbs [vid];
			v.Inverted = invert;
			return v;
		}
	}
	Assertion m_newAssertion(KNSubject subject, KNVerb verb, KNSubject receiver, KNSubject source,bool isInquiry,Character c) {
		var f = new Assertion { Inquiry = isInquiry, Source = source};
		f.SetOwner (c);
		f.AddSubject (subject);
		f.AddVerb (verb);
		f.AddReceivor (receiver);
		return f;
	}

//----dialogue boxes-----
	public static DialogueUnit CreateSubjectList(Character speaker) {
		return CreateSubjectList (speaker, null, Instance.FinishFact);
	}

	public static DialogueUnit CreateSubjectList(Character speaker, Character listener) {
		return CreateSubjectList (speaker, listener, Instance.FinishFact);
	}
	public static DialogueUnit CreateSubjectList(Character speaker,Character listener,DialogueOption.OnSelection selectionFunction) {
		var du = new DialogueUnit {speaker = speaker, listener = listener};
		du.addDialogueOptions (Instance.GetSubjectOptions (speaker, true, selectionFunction), "Select a Subject: ");
		listener.processDialogueRequest (speaker, du);
		return du;
	}

	void AddWildCard(List<DialogueOption> dos,Assertion a){ 
		var o = new OptionKnowledgeBase {text = "Anything?",
			assertion = a, responseFunction = FinishFact};
		dos.Add (o);
	}

	public List<DialogueOption> GetExclamations(Character c,bool includeWildcard,DialogueOption.OnSelection selectionFunction) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (var ks in kd.Subjects) {
			if (!ks.Exclamation || ks.Hide)
				continue;
			OptionKnowledgeBase o = new OptionKnowledgeBase {responseFunction = selectionFunction,
				text = (ks.SubjectDisplayed == "none")?ks.SubjectName:ks.SubjectDisplayed};
			o.assertion = m_newAssertion(ks,null,null,CopySubject(c.name),false,c);
			o.SelectionFunction = selectionFunction;
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, m_newAssertion(null,null,null,CopySubject(c.name),false,c)); }
		return dos;
	}

	public List<DialogueOption> GetSubjectOptions(Character c,bool includeWildcard,DialogueOption.OnSelection selectionFunction) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (var ks in kd.Subjects) {
			if (ks.Hide || ks.Exclamation)
				continue;
			OptionKnowledgeBase o = new OptionKnowledgeBase {responseFunction = SubjectSelected,
				text = (ks.SubjectDisplayed == "none")?ks.SubjectName:ks.SubjectDisplayed};
			o.assertion = m_newAssertion (ks, null, null, CopySubject (c.name), false, c);
			o.SelectionFunction = selectionFunction;
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, m_newAssertion(null,null,null,CopySubject(c.name),false,c)); }
		return dos;
	}

	public List<DialogueOption> GetVerbOptions (Character c, Assertion a, bool includeWildcard, DialogueOption.OnSelection selectionFunction) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;

		foreach (var kv in kd.MatchingVerbs(a)) {
			//Debug.Log ("Found verb: " + kv.VerbName + " subName: " + sub.subjectName + " canAct: " + kv.canAct(sub));
			var o = new OptionKnowledgeBase { responseFunction = VerbSelected,
				text = (kv.VerbDisplayed == "none")?kv.VerbName:kv.VerbDisplayed, assertion = a.CopyAssertion()};
			o.assertion.AddVerb( kv);
			o.SelectionFunction = selectionFunction;
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, a.CopyAssertion()); }
		return dos;
	}
	public static DialogueUnit CreateCommandList(Character speaker, Assertion a, Character listener, DialogueOption.OnSelection selectionFunction) {
		var du = new DialogueUnit {speaker = speaker, listener = listener};
		du.addDialogueOptions (Instance.GetCommandOptions (speaker, a, false, selectionFunction),"What will you say?");
		listener.processDialogueRequest (speaker, du);
		return du;
	}
	public List<DialogueOption> GetCommandOptions (Character c, Assertion a, bool includeWildcard, DialogueOption.OnSelection selectionFunction) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;

		foreach (var kv in kd.MatchingVerbs(a)) {
			if (!kv.IsCommand)
				continue;
			var o = new OptionKnowledgeBase { responseFunction = VerbSelected,
				text = kv.VerbName, assertion = a.CopyAssertion()};
			o.assertion.AddVerb( kv);
			o.SelectionFunction = selectionFunction;
			dos.Add (o);
		}
		if (includeWildcard) { AddWildCard (dos, a.CopyAssertion()); }
		return dos;
	}

	public List<DialogueOption> GetReceiverOptions(Character c, Assertion a, bool includeWildcard,DialogueOption.OnSelection selectionFunction) {
		var dos = new List<DialogueOption> ();
		KNDatabase kd = c.knowledgeBase;
		foreach (var ks in kd.MatchingDirectObjects(a)) {
			//Debug.Log ("name: " + ks.SubjectName + " hide: " + ks.Hide);
			if (ks.Hide)
				continue;
			var o = new OptionKnowledgeBase {responseFunction = selectionFunction, text = ks.SubjectName,
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

		var du = new DialogueUnit {speaker = dob.speaker, listener = dob.listener,Previous = o.GetSequence()};
		//Debug.Log ("Previous sequence: " + du.Previous);
		du.addDialogueOptions (GetVerbOptions(dob.speaker,dob.assertion,true,dob.SelectionFunction), "Select a Verb");
		o.closeSequence();
		du.startSequence ();
	}

	void VerbSelected (DialogueOption o) {
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;

		DialogueUnit du = new DialogueUnit {speaker = dob.speaker, listener = dob.listener,Previous = o.GetSequence()};
		du.addDialogueOptions (GetReceiverOptions(dob.speaker,dob.assertion,true, dob.SelectionFunction));
		o.closeSequence();
		du.startSequence ();
	}

	void FinishFact(DialogueOption o) {
		//Debug.Log ("Finishing fact");
		OptionKnowledgeBase dob = (OptionKnowledgeBase)o;
		o.closeSequence ();
		if (dob.assertion != null && dob.listener)
			dob.listener.knowledgeBase.LearnAssertion (dob.assertion);
	}

	public static DialogueUnit CreateExclamationList(Character speaker, Character listener) {
		return CreateExclamationList (speaker, listener, Instance.FinishFact);
	}
	public static DialogueUnit CreateExclamationList(Character speaker,Character listener,DialogueOption.OnSelection selectionFunction) {
		var du = new DialogueUnit {speaker = speaker, listener = listener};
		du.addDialogueOptions (Instance.GetExclamations (speaker, false, selectionFunction), "What will you express?");
		listener.processDialogueRequest (speaker, du);
		return du;
	}
}