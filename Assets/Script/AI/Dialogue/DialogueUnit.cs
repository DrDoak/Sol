using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUnit  {

	List<DialogueSubunit> elements;
	textbox currentTB;
	DialogBox currentDB;
	ListSelection currentList;
	public Character speaker;
	public Character listener;
	public bool finished = false;
	bool awaitingResponse = false;
	int currentElement = 0;
	string unparsed;
	TextboxManager tm;
	List<Character> modifiedAnims;
	DialogueSubunit lastOptionsBox;

	// Use this for initialization
	public DialogueUnit () {
		//Debug.Log ("starting a ds!");
		tm = GameObject.FindObjectOfType<TextboxManager> ();
		modifiedAnims = new List<Character> ();
		elements = new List<DialogueSubunit> ();
	}

	public void startSequence() {
		parseNextElement ();
	}
	public void parseNextElement() {
		if (currentElement >= elements.Count) {
			endSequence ();
		} else {
			if (currentList) 
				GameObject.Destroy (currentList.gameObject);
			if (currentTB)
				GameObject.Destroy (currentTB.gameObject);
			DialogueSubunit ne = elements [currentElement];
			if (ne.isOption) {
				awaitingResponse = true;
				if (speaker != null)
					speaker.setAutonomy (false);
				//currentDB = tm.addDialogueOptions (ne.text,speaker.gameObject,ne.options);
				currentList = tm.addListOptions(ne.text,speaker.gameObject,ne.options);
				currentList.masterSequence = this;
			} else {
				awaitingResponse = false;
				currentTB = tm.addTextbox(ne.text,speaker.gameObject,true);
				currentTB.masterSequence = this;
			}
			setAnimation (ne.animation);
			currentElement += 1;
		}
	}

	//-----------Setting animation -------------
	public void setAnimation(string animateChar, string animation) {
		if (animateChar == null) {
			setAnimation (animation);
		} else {
			setAnimation (GameObject.FindObjectOfType<CharacterManager> ().findChar (animateChar), animation);
		}
	}
	public void setAnimation(string animation) {
		setAnimation (speaker, animation);
	}
	public void setAnimation(Character c, string animation) {
		if (animation != null && c.animCutscene != null) {
			//Debug.Log ("setting animation for " + c.name + " to " + animation);
			if (!modifiedAnims.Contains(c)) {
				c.GetComponent<Animator> ().runtimeAnimatorController = c.animCutscene;
				modifiedAnims.Add (c);
			}
			if (animation == "none") {
				c.GetComponent<Animator> ().runtimeAnimatorController = c.animDefault;
				modifiedAnims.Remove (c);
			} else {
				c.GetComponent<Animator> ().Play (animation);
			}
		} 
	}
	//-------------------------------------------
	//public void walkToPoint(Vector2 point, float prox) {}
	public void walkToChar(string animateChar, string name, float prox) {
		if (animateChar == null) {
			walkToChar (speaker,name,prox);
		} else {
			walkToChar (GameObject.FindObjectOfType<CharacterManager> ().findChar (animateChar), name,prox);
		}
	}
	public void walkToChar(Character c, string name, float prox) {
		Character talkTarget = GameObject.FindObjectOfType<CharacterManager> ().findChar (name);
		if (talkTarget != null) {
			c.setTargetPoint (talkTarget.transform.position, prox);
		}
	}

	//-----------Turning towards-------------
	public void turnTowards(string turningPerson,string name, bool towards) {
		if (turningPerson == null) {
			turnTowards (name, towards);
		} else {
			turnTowards (GameObject.FindObjectOfType<CharacterManager> ().findChar (name), name, towards);
		}
	}
	public void turnTowards(string name, bool towards) {
		turnTowards (speaker, name, towards);
	}
	public void turnTowards(Character c, string name, bool towards) {
		Character talkTarget = GameObject.FindObjectOfType<CharacterManager> ().findChar (name);
		Debug.Log ("Turning: " + c.name + " to " + name + " with: " + towards);
		if (talkTarget != null) {
			if (talkTarget.transform.position.x > c.transform.position.x) {
				c.GetComponent<Movement> ().setFacingLeft (!towards);
			} else {
				c.GetComponent<Movement> ().setFacingLeft (towards);
			}
		}
	}
	//---------------------------------------

	//public void parseSequence(string s) {}
	public void addTextbox(string s) {
		DialogueSubunit ne = new DialogueSubunit ();
		ne.text = s;
		elements.Add (ne);
	}
	public void addTextbox(string s,string animation) {
		DialogueSubunit ne = new DialogueSubunit ();
		ne.text = s;
		ne.animation = animation;
		elements.Add (ne);
	}
	public void addDialogueOptions(List<DialogueOption> options) {
		addDialogueOptions (options, "What do you say?");
	}
	public void addOption(DialogueOption option) {
		if (lastOptionsBox == null) {
			List<DialogueOption> ops = new List<DialogueOption> ();
			addDialogueOptions (ops);
		} else {
			lastOptionsBox.options.Add (option);
		}
	}
	public void addDialogueOptions(List<DialogueOption> options,string mainPrompt) {
		foreach (DialogueOption dop in options) {
			dop.speaker = speaker;
			dop.listener = listener;
		}
		DialogueSubunit ne = new DialogueSubunit ();
		ne.options = options;
		ne.isOption = true;
		ne.text = mainPrompt;
		lastOptionsBox = ne;
		elements.Add (ne);
	}

	public void goToElement(int i) {
		currentElement = i;
	}
	public void initiateDialogue() {
		currentElement = 0;
		parseNextElement ();
	}

	public void endSequence() {
		if (awaitingResponse && speaker != null)
			speaker.setAutonomy (true);
		foreach(Character c in modifiedAnims){
			c.GetComponent<Animator> ().runtimeAnimatorController = c.animDefault;
		}
		if (currentList) 
			GameObject.Destroy (currentList.gameObject);
		if (currentTB)
			GameObject.Destroy (currentTB.gameObject);
		finished = true;
		//Debug.Log ("Ending dialogue");
	}

	void runEvent() {}
}