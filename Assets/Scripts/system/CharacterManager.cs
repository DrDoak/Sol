using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterManager : MonoBehaviour {

	public static CharacterManager Instance;

	[SerializeField] private string m_CharDatabase;

	Dictionary<string,Dictionary<string,string>> m_LoadedCharData;
	Dictionary<string,Character> m_RegisteredChars;

	bool m_InitChar = false;

	void Awake () {
		if (Instance == null)
			Instance = this;
		m_RegisteredChars = new Dictionary<string,Character> ();
		m_LoadedCharData = new Dictionary<string,Dictionary<string,string>> ();

		initCharacters (m_CharDatabase);
	}

	void Start() {}

	void Update () {}

	void initCharacters(string source) {
		if (!KNManager.Instance.DatabaseInitialized) {
			KNManager.Instance.InitDatabase ();
		}
		List<Dictionary<string,string>> subjects = FactCSVImporter.importFile (source);
		foreach (Dictionary<string,string> d in subjects) {
			KNSubject sub = KNManager.CopySubject (d ["name"]);
			Character c = m_findChar (d ["name"]);
			if (c != null) {
				applyLoadedDataToChar(c);
				sub.Owner = c;
			}
			m_LoadedCharData [d ["name"]] = d;
		}
	}

	public void applyLoadedDataToChar(Character c) {
	//	Debug.Log ("Applying loaded Data to: " + c.name);
		if (m_LoadedCharData.ContainsKey (c.name)) {
			Dictionary<string,string> d = m_LoadedCharData [c.name];
			c.knowledgeGroups = FactCSVImporter.splitStringRow (d ["knowledgeGroups"]);

			float.TryParse (d ["perception"],out c.perception);
			float.TryParse (d ["persuasion"],out c.persuasion);
			float.TryParse (d ["logic"],out c.logic);

			Personality p = c.PersonalityData;
			float.TryParse (d ["egoCombat"],out p.egoCombat);
			float.TryParse (d ["egoLogic"],out p.egoLogic);
			float.TryParse (d ["egoSocial"],out p.egoSocial);

			float.TryParse (d ["boldness"],out p.boldness);
			float.TryParse (d ["temperament"],out p.temperament);
			float.TryParse (d ["emotionLogic"],out p.emotionLogic);
			float.TryParse (d ["opennessAllegiance"],out p.opennessAllegiance);
			float.TryParse (d ["agreeableness"],out p.agreeableness);
			float.TryParse (d ["pragmaticIdealistic"],out p.pragmaticIdealistic);
		}
		foreach (string s in c.knowledgeGroups) {
			KNManager.Instance.AddKnowledgeGroups (c.knowledgeBase, s);
		}
	}

	public static void RegisterChar(Character c) {
		//Instance.m_toRegisterCharacters.Add (c);
		Instance.initChar (c);
	}

	void initChar(Character c) {
		Debug.Log ("Registering Character: " + c.gameObject);
		if (m_RegisteredChars.ContainsKey (c.name.ToLower ())) {
			Debug.Log ("TODO: re-add character information from save data. Now initializing as if new.");
			//return;
		}
		m_RegisteredChars[c.name.ToLower()] = c;
		KNSubject ks = KNManager.CopySubject (c.name);
		ks.Owner = c;
		KNManager.Instance.SetSubject (c.name, ks);
		applyLoadedDataToChar (c);
	}
	public void animateChar(string name, string animation){}
	public void setDialogueUnit(string name, DialogueUnit ds) {
		Character c = m_findChar (name);
		if (c != null) {
			c.setDialogueUnit (ds);
		}
	}
	Character m_findChar(string targetName) {
		string lowerName = targetName.ToLower ();
		//Debug.Log ("Searching for character: " + lowerName);
		foreach (string k in m_RegisteredChars.Keys) {
			//Debug.Log ("found: " + m_RegisteredChars [k].name.ToLower ());
			if (m_RegisteredChars[k].name.ToLower () == lowerName) {
				return m_RegisteredChars[k];
			}
		}
		return null;
	}
	public Character findChar(Character targetC) {
		foreach (string k in m_RegisteredChars.Keys) {
			if (m_RegisteredChars[k] == targetC) {
				return m_RegisteredChars[k];
			}
		}
		return null;
	}

	public static Character FindChar(string targetName) {
		return Instance.m_findChar (targetName);
	}

}