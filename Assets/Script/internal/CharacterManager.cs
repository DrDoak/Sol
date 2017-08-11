using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {

	Dictionary<string,Character> registeredChars;
	List<string> runEvents;
	// Use this for initialization
	void Awake () {
//		Debug.Log ("start cm init!");
		registeredChars = new Dictionary<string,Character> ();
	}
	
	// Update is called once per frame
	void Update () {}

	public void registerChar(Character c) {
		registeredChars.Add (c.name, c);
	}
	public void animateChar(string name, string animation){
	}
	public void setDialogueSequence(string name, DialogueSequence ds) {
		Character c = findChar (name);
		if (c != null) {
			registeredChars [name].setDialogueSequence (ds);
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
}
