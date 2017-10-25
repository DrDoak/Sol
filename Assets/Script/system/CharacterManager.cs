using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterManager : MonoBehaviour {

	Dictionary<string,Character> registeredChars;
	public string charDatabase;
	KNManager km;
	List<string> runEvents;
	Dictionary<string,Dictionary<string,string>> loadedCharData;
	bool initChars = false;
	// Use this for initialization
	void Awake () {
		registeredChars = new Dictionary<string,Character> ();
		loadedCharData = new Dictionary<string,Dictionary<string,string>> ();
	}
	void Start() {
		km = FindObjectOfType<KNManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!initChars) {
			initCharacters (charDatabase);
			initChars = true;
		}
	}

	void initCharacters(string source) {
		//Debug.Log ("Iniitializing characters");
		List<Dictionary<string,string>> subjects = FactCSVImporter.importFile (source);
		foreach (Dictionary<string,string> d in subjects) {
			KNSubject sub = km.findOrCreateSubject (d ["name"]);
			Character c = findChar (d ["name"]);
			if (c != null) {
				applyLoadedDataToChar(c);
			}
			loadedCharData [d ["name"]] = d;
		}
	}
	public void applyLoadedDataToChar(Character c) {
		//Debug.Log ("Applying loaded Data to: " + c.name);
		if (loadedCharData.ContainsKey (c.name)) {
			Dictionary<string,string> d = loadedCharData [c.name];
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
			km.addKnowledgeGroups (c.knowledgeBase, s);
		}
	}

	public void registerChar(Character c) {
		registeredChars.Add (c.name.ToLower(), c);
		if (km != null) {
			applyLoadedDataToChar (c);
		}
	}
	public void animateChar(string name, string animation){}
	public void setDialogueUnit(string name, DialogueUnit ds) {
		Character c = findChar (name);
		if (c != null) {
			registeredChars [name].setDialogueUnit (ds);
		}
	}
	public Character findChar(string targetName) {
		string lowerName = targetName.ToLower ();
		foreach (string k in registeredChars.Keys) {
			if (registeredChars[k].name.ToLower () == lowerName) {
				return registeredChars[k];
			}
		}
		//Debug.Log ("Character not found: "+ lowerName);
		return null;
	}
	public Character findChar(Character targetC) {
		foreach (string k in registeredChars.Keys) {
			if (registeredChars[k] == targetC) {
				return registeredChars[k];
			}
		}
		//Debug.Log ("Character not found: "+ targetC.name);
		return null;
	}
}
