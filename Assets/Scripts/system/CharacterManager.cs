using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterManager : MonoBehaviour {

	public static CharacterManager Instance;

	[SerializeField] private string m_CharDatabase;

	Dictionary<string,Dictionary<string,string>> m_LoadedCharData;
	Dictionary<string,Character> m_RegisteredChars;

	bool m_InitChar = false;

	KNManager km;

	void Awake () {
		if (Instance == null)
			Instance = this;
		m_RegisteredChars = new Dictionary<string,Character> ();
		m_LoadedCharData = new Dictionary<string,Dictionary<string,string>> ();
	}

	void Start() {
		km = KNManager.Instance;
	}

	void Update () {
		if (m_InitChar)
			return;
		
		initCharacters (m_CharDatabase);
		m_InitChar = true;
	}

	void initCharacters(string source) {
		//Debug.Log ("Iniitializing characters");
		List<Dictionary<string,string>> subjects = FactCSVImporter.importFile (source);
		foreach (Dictionary<string,string> d in subjects) {
			KNSubject sub = km.FindOrCreateSubject (d ["name"]);
			Character c = findChar (d ["name"]);
			if (c != null) {
				applyLoadedDataToChar(c);
			}
			m_LoadedCharData [d ["name"]] = d;
		}
	}
	public void applyLoadedDataToChar(Character c) {
		//Debug.Log ("Applying loaded Data to: " + c.name);
		if (m_LoadedCharData.ContainsKey (c.name)) {
			Dictionary<string,string> d = m_LoadedCharData [c.name];
			c.knowledgeGroups = FactCSVImporter.splitStringRow (d ["knowledgeGroups"]);
			float.TryParse (d ["perception"],out c.perception);
			float.TryParse (d ["persuasion"],out c.persuasion);
			float.TryParse (d ["logic"],out c.logic);

			Personality p = c.pers;
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
			km.AddKnowledgeGroups (c.knowledgeBase, s);
		}
	}

	public void registerChar(Character c) {
		if (m_RegisteredChars.ContainsKey (c.name.ToLower ())) {
			Debug.Log ("TODO: re-add character information from data");
			return;
		}
		m_RegisteredChars.Add (c.name.ToLower(), c);
		if (km != null) {
			KNSubject ks = km.FindOrCreateSubject (c.name);
			ks.Owner = c;
			km.SetSubject (c.name, ks);
			applyLoadedDataToChar (c);
		}
	}
	public void animateChar(string name, string animation){}
	public void setDialogueUnit(string name, DialogueUnit ds) {
		Character c = findChar (name);
		if (c != null) {
			c.setDialogueUnit (ds);
		}
	}
	public Character findChar(string targetName) {
		string lowerName = targetName.ToLower ();
		foreach (string k in m_RegisteredChars.Keys) {
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
}
