using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {

	Dictionary<string,Character> registeredChars;
	List<string> runEvents;
	// Use this for initialization
	void Start () {
		registeredChars = new Dictionary<string,Character> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void registerChar(Character c) {
		registeredChars.Add (c.name, c);
	}
	public void animateChar(string name, string animation){
	}
	public void setDialogueSequence(string name, DialogueSequence ds) {
		if (registeredChars.ContainsKey(name)) {
			Debug.Log ("found the key for character: " + name);
			registeredChars [name].setDialogueSequence (ds);

		}
	}
}
