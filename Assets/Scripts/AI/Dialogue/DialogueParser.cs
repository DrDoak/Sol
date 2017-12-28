using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour {
	//Dialogue 
	DialogueUnit presetDS;
	Character talkTarget;
	bool isPresetDS;
	bool choosingDialogue = false;
	public bool isSpeaking;
	DialogueUnit currDialogueUnit;
	DialogueSequence currDSequence;
	CharacterManager cm;
	Movement movt;
	void Start() {
		movt = GetComponent<Movement> ();
		name = GetComponent<Character> ().name;
		cm = FindObjectOfType<CharacterManager> ();
	}
	void Update() {
		if (isSpeaking) {
			dialogueTick ();
		}
	}
	public void say(string text) {
		say (text, "none");
	}
	public void say(string text,string talkTo) {
		if (talkTo != "none") {
			talkTarget = CharacterManager.FindChar (talkTo);
		}
		isSpeaking = true;
		currDSequence = parseSequence (text, 0, 0,null);
		advanceSequence ();
	}
	/* Cutscene scripting guide:
	 *  Normal text is shown as dialogue for the starting character.
	 * 	Using the '|' character or the enter character will create a new textbox.
	 *  At the start of a new textbox if the colon character is found within the first 18 characters, the game will attempt to search
	 *  For the character and make the dialogue come from that character instead.
	 * 
	 * */
	DialogueSequence parseSequence(string text,int startingChar,int indLevel,DialogueSequence parentSeq) {
		DialogueSequence newSeq = new DialogueSequence ();
		newSeq.parentSequence = parentSeq;
		List<DialogueUnit> subDS = new List<DialogueUnit> ();
		Character speaker = GetComponent<Character>();
		string targetCharName = speaker.name;
		DialogueUnit ds = new DialogueUnit {speaker = speaker};
		subDS.Add (ds);
		string lastText = "";
		string lastAnim = "none";
		int i = startingChar;
		bool specialGroup = false;
		int currIndent = 0;
		while (i < text.Length) {
			char lastC = text.ToCharArray () [i];
			newSeq.rawText += lastC;
			if (lastText.Length == 0 && lastC == '\t') {
				currIndent += 1;
			} else {
				if (currIndent < indLevel) {
					break;
				}
				if (lastText.Length == 0 && lastC == ' ') {
				} else if (lastText.Length == 0 && lastC == '-') {
					DialogueSequence newS = parseSequence (text, i, indLevel + 1, newSeq);
					i += newS.numChars;
				} else if (lastC == '`') {
					specialGroup = true;
					lastText += lastC;
				} else if (!specialGroup && lastC == ':' && lastText.Length < 18) {
					cm.setDialogueUnit (targetCharName, ds);
					ds = new DialogueUnit {speaker = speaker};
					subDS.Add (ds);
					targetCharName = lastText;
					//Debug.Log (targetCharName);
					lastText = "";
				} else if (lastC == '\n' || lastC == '|') {
					if (lastText.Length > 0) {
						if (lastAnim == "none") {
							ds.addTextbox (lastText);
						} else {
							ds.addTextbox (lastText, lastAnim);
						}
					}
					currIndent = 0;
					lastText = "";
					specialGroup = false;
				} else {
					lastText += lastC;
				}
			}
			newSeq.numChars += 1;
			i += 1;
		}
		if (lastAnim == "none") {
			ds.addTextbox (lastText);
		} else {
			ds.addTextbox (lastText,lastAnim);
		}
		cm.setDialogueUnit(targetCharName,ds);
		subDS.Add (ds);
		newSeq.allDUnits = subDS;
		return newSeq;
	}
	void dialogueTick () {
		if (talkTarget != null) {
			if (talkTarget.transform.position.x > transform.position.x) {
				movt.setFacingLeft (false);
			} else {
				movt.setFacingLeft (true);
			}
		}
		if (currDialogueUnit.finished) {
			if (currDSequence.allDUnits.Count > 0) {
				advanceSequence ();
			} else {
				if (currDSequence.parentSequence != null) {
					currDSequence = currDSequence.parentSequence;
					advanceSequence ();
				} else {
					endDialogue ();
				}
			}
		}
	}
	void advanceSequence() {
		currDialogueUnit = currDSequence.allDUnits [0];
		currDialogueUnit.startSequence ();
		currDSequence.allDUnits.Remove (currDialogueUnit);
	}
	public void endDialogue() {
		Debug.Log ("End of character's dialogue");
		currDSequence = null;
		currDialogueUnit = null;
		isSpeaking = false;
		talkTarget = null;
	}
}
