using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSequence  {

	List<DialogueElement> elements;
	textbox currentTB;
	DialogBox currentDB;
	public Character speaker;
	bool awaitingResponse = false;
	int currentElement = 0;
	string unparsed;
	TextboxManager tm;
	DialogBox.optionResponse responseFunction;
	// Use this for initialization
	public DialogueSequence () {
		tm = MonoBehaviour.FindObjectOfType<TextboxManager> ();
		elements = new List<DialogueElement> ();
		responseFunction = respondToOption;
	}
	
	// Update is called once per frame
	void Update () {
		if (!awaitingResponse) {
			if (currentTB == null) {
				Debug.Log ("tb destroyed moving to next element");
				parseNextElement ();
			}
		}
	}

	public void parseNextElement() {
		Debug.Log ("Parsing next element");
		if (currentElement >= elements.Count) {
			endDialogue ();
		} else {
			bool dialogueOption = false;
			DialogueElement ne = elements [currentElement];
			if (ne.isOption) {
				awaitingResponse = true;
				currentDB = tm.addDialogueOptions (ne.text,speaker.gameObject,ne.options,responseFunction);
			} else {
				awaitingResponse = false;
				currentTB = tm.addTextbox(ne.text,speaker.gameObject,true);
			}
			currentElement += 1;
		}
	}
	public void parseSequence(string s) {
	}
	public void addTextbox(string s) {
		DialogueElement ne = new DialogueElement ();
		ne.text = s;
		elements.Add (ne);
	}
	public void addDialogueOptions(List<string> options) {
	}

	public void goToElement(int i) {
		currentElement = i;
	}
	public void respondToOption(int i) {
		Debug.Log ("Got response " + i + " but no response function set!!!");
	}
	public void setOptionResponse(DialogBox.optionResponse or) {
		responseFunction = or;
	}

	public void initiateDialogue() {
		currentElement = 0;
		parseNextElement ();
	}

	public void endDialogue() {
		Debug.Log ("Ending dialogue");
	}
	void runEvent() {
		
	}
}
