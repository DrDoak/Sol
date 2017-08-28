using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnowledgeManager : MonoBehaviour {

	Dictionary<string,DatabaseEntry> fullDatabase;
	GameObject knowledgeScrollbox;
	CharacterManager cm;
	public string mainDatabase;
	// Use this for initialization
	void Start () {
		fullDatabase = new Dictionary<string,DatabaseEntry> ();
		cm = FindObjectOfType<CharacterManager> ();
		initDatabase (mainDatabase);
	}
	
	// Update is called once per frame
	void Update () {}

	public void initDatabase(string s) {
		FactCSVImporter.readFile (s);
	}
	public void outputKnowledgeBox(string characterName) {
		GameObject go = Instantiate (knowledgeScrollbox);
		Character c = cm.findChar (characterName);
	}
	public void parseDatabase(string factID) {
		DatabaseEntry newK = new DatabaseEntry ();
		fullDatabase.Add (factID, newK);
	}

	public void addFactCategory(Character c, string cat) {
		foreach (string s in fullDatabase.Keys) {
			if (fullDatabase [s].factGroups.Contains (cat)) {
				c.addFact (fullDatabase [s].toFact());
			}
		}
	}
	public void addFact(Character c, string nameID) {
		c.addFact (fullDatabase[nameID].toFact());
	}
	public void addFact(Character c, Fact k) {
		c.addFact (k);
	}
}

