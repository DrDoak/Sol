using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterManager : MonoBehaviour {

	Dictionary<string,Character> registeredChars;
	List<string> runEvents;
	Dictionary<string,DatabaseEntry> fullDatabase;
	// Use this for initialization
	void Awake () {
		registeredChars = new Dictionary<string,Character> ();
		fullDatabase = new Dictionary<string,DatabaseEntry> ();
	}
	
	// Update is called once per frame
	void Update () {}

	public void registerChar(Character c) {
		registeredChars.Add (c.name, c);
	}
	public void animateChar(string name, string animation){}
	public void setDialogueUnit(string name, DialogueUnit ds) {
		Character c = findChar (name);
		if (c != null) {
			registeredChars [name].setDialogueUnit (ds);
		}
	}
	public Character findChar(string targetName) {
		foreach (string k in registeredChars.Keys) {
			if (registeredChars[k].name == targetName) {
				return registeredChars[k];
			}
		}
		Debug.Log ("Character not found: "+ targetName);
		return null;
	}
	public Character findChar(Character targetC) {
		foreach (string k in registeredChars.Keys) {
			if (registeredChars[k] == targetC) {
				return registeredChars[k];
			}
		}
		Debug.Log ("Character not found: "+ targetC.name);
		return null;
	}
	public void parseDatabase(string factID) {
		DatabaseEntry newK = new DatabaseEntry ();
		fullDatabase.Add (factID, newK);
	}
	public void addFactCategory(Character c, string cat) {
		foreach (string s in fullDatabase.Keys) {
			if (fullDatabase [s].factGroups.Contains (cat)) {
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
