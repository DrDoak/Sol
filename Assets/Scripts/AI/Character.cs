using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PersItem))]
[RequireComponent (typeof (Movement))]
[RequireComponent (typeof (Fighter))]
[RequireComponent (typeof (Attackable))]
[RequireComponent (typeof (DialogueParser))]
[RequireComponent (typeof (Observer))]
[RequireComponent (typeof (Observable))]
[RequireComponent (typeof (RPSpeaker))]

public class Character : Interactable {

	public bool HighlightInteractables = false;
	new public string name = "default";
	public List<string> knowledgeGroups;
	public string faction = "noFaction";
	public bool facingLeft = false;
	public float health = 100.0f;
	public float healthPerc = 1.0f;

	Movement movt;
	DialogueParser parser;
	public Observable m_observable;
	Observer observer;

	//Saving
	public bool recreated = false;
	bool registryChecked = false;
	protected bool autonomy = true;
	float interactRange = 2.0f;

	//Dialogue 
	DialogueUnit presetDS;
	bool isPresetDS;
	bool choosingDialogue = false;
	TextboxManager tm;
	KNManager km;
	public RPSpeaker speaker;
	public RuntimeAnimatorController animDefault;
	public RuntimeAnimatorController animCutscene;

	//Skills:
	public float perception = 0.0f;
	public float persuasion = 0.0f;
	public float logic = 0.0f;
	public Personality PersonalityData;

	public CharData data = new CharData();
	public Dictionary<Character,Relationship> charInfo = new Dictionary<Character,Relationship> ();
	public KNDatabase knowledgeBase;

	void Start () {
		init ();
	}
	protected void init() {
		movt = GetComponent<Movement> ();
		observer = GetComponent<Observer> ();
		m_observable = GetComponent<Observable> ();
		tm = FindObjectOfType<TextboxManager> ();
		km = FindObjectOfType<KNManager> ();
		parser = GetComponent<DialogueParser> ();
		speaker = GetComponent<RPSpeaker> ();

		PersonalityData = new Personality ();
		knowledgeBase = new KNDatabase ();
		knowledgeBase.Owner = this;
		if (faction == "noFaction" && GetComponent<Attackable> ()) {
			faction = GetComponent<Attackable> ().faction;
		}
		Debug.Log ("Character initialization for: " + gameObject);
		CharacterManager.RegisterChar (this);
	}

	public virtual void setAutonomy(bool active) {
		//Debug.Log ("Setting autonomy " + gameObject + " to " + active);
		autonomy = active;
		if (GetComponent<NPC> ()) {
			GetComponent<NPC> ().setAutonomy(active);
		}
		if (GetComponent<Player> ()) {
			GetComponent<Player> ().autonomy = active;
		}
	}

//----Dialogue and interaction
	public override void onInteract(Character interactor) {}

	public void playerInteraction() {
		//Debug.Log (gameObject + " is trying an interaction");
		Interactable[] allIter = FindObjectsOfType<Interactable> ();
		float maxPriority = 0f;
		Interactable interObj = null;
		foreach (Interactable c in allIter) {
			if (c != this  && (c.transform.position.x < transform.position.x && movt.facingLeft || 
				c.transform.position.x > transform.position.x && !movt.facingLeft)) {
				float cDist = Vector3.Distance (c.transform.position, transform.position);
				//Debug.Log ("actual Dist: " + cDist + " :quota: " + interactRange + " true: " + (cDist < interactRange));
				if (cDist < interactRange) {
					float testPriority = c.interactionPriority;
					if ((c.transform.position.x < transform.position.x && movt.facingLeft) ||
						(!movt.facingLeft && c.transform.position.x > transform.position.x)) {
						testPriority *= 2f;
					}
					testPriority /= (cDist / interactRange);
					if (testPriority > maxPriority) {
						maxPriority = testPriority; 
						interObj = c;
					}
				}
			}
		}
		//Debug.Log ("priority: " + maxPriority);
		if (maxPriority > 0f) {
			attemptInteraction (interObj);
		}
	}

	public void attemptInteraction(Interactable i) {
		float cDist = Vector3.Distance (i.transform.position, transform.position);
		//Debug.Log ("actual Dist: " + cDist + " :quota: " + interactRange + " true; " + (cDist < interactRange));
		if (cDist < interactRange) {
			Debug.Log ("Attempting Interaction with object: " + i.gameObject);
			EVInteract ie = new EVInteract ();
			ie.Interactor = this;
			if (i.GetComponent<Character> ()) {
				ie.IsCharInteraction = true;
				ie.InteracteeChar = i.GetComponent<Character> ();
			}
			ie.Interactee = i;
			m_observable.broadcastToObservers (ie);
			i.onInteract (this);
			if (i.GetComponent<Observer> ()) {
				i.GetComponent<Observer> ().respondToEvent (ie);		
			}
		}
	}

	public virtual void respondToEvent(Event e) {}

	public virtual void setTargetPoint(Vector3 targetPoint, float proximity) {
		GetComponent<Player> ().setTargetPoint (targetPoint, proximity);
	}

//-----------------------Dialogue conditions----------------

	public void setDialogueUnit(DialogueUnit ds) {
		isPresetDS = true;
		ds.speaker = this;
		presetDS = ds;
	}
	public void onTBComplete() {
		if (isPresetDS) {
			presetDS.parseNextElement ();
		}
	}
/*
	public virtual void initiateDialogueRequest(Character targetChar) {
		Debug.Log ("dialogue Request initiated");
		EVInteract se = new EVInteract ();
		respondToEvent (se);
	}
*/

	public virtual DialogueSubunit chooseDialogueOption(List<DialogueSubunit> dList) {
		choosingDialogue = true;
		return null;
	}
	public virtual void processDialogueRequest(Character c,DialogueUnit d) {}
	public virtual void acceptDialogue(Character c,DialogueUnit d) {}

		
//--------Character info
	public Relationship getCharInfo(Character c) {
		//Debug.Log ("Getting Character info for: " + c);
		if (c == null) {
			//Debug.Log ("Why is the character null?");
			return null;
		} else if (charInfo.ContainsKey (c)) {
			return charInfo [c];
		} else {
			return evaluateNewChar (c);
		}
	}
	public Relationship evaluateNewChar(Character c) {
		//Debug.Log ("Evaluating new character: " + c.name);
		Relationship ci = new Relationship ();
		ci.Name = c.name;
		ci.ParentChar = this;
		charInfo.Add (c, ci);
		return ci;
	}
		
//---Speech
	public void say(string text) {
		say (text, "none");
	}
	public void say(string text,string talkTo) {
		parser.say(text,talkTo);
	}
	public void endDialogue() {
		parser.endDialogue ();
	}

//-------------Saving:--------------------//
	public void registryCheck() {
		/*if (data.regID == "") {
			data.regID = "Not Assigned";
		}
		if (FindObjectOfType<GameManager>().checkRegistered(gameObject)) {
			Destroy(gameObject);
		}
		registryChecked = true;*/
	}
	void Update () {
		/*if (!registryChecked) {
			registryCheck ();
		}*/
	}

	public void StoreData() {
		//Debug.Log ("storing data");
		/*data.name = gameObject.name;
		data.pos = transform.position;
		data.health = GetComponent<Attackable>().health;
		string properName = "";
		foreach (char c in gameObject.name) {
			if (!c.Equals ('(')) {
				properName += c;
			} else {
				break;
			}
		}
		//Debug.Log("ID: " + properName);
		data.prefabPath = properName; //gameObject.name;*/
	}

	public void LoadData() {
		name = data.name;
		/*transform.position = data.pos;
		GetComponent<Attackable>().health = data.health;*/
	}
	/*
	public void ApplyData() {
		SaveObjManager.AddCharData(data);
	}

	void OnEnable() {
		Debug.Log ("sol on enable");
		SaveObjManager.OnLoaded += LoadData;
		SaveObjManager.OnBeforeSave += StoreData;
		SaveObjManager.OnBeforeSave += ApplyData;
	}

	void OnDisable() {
		Debug.Log ("sol on disable");
		SaveObjManager.OnLoaded -= LoadData;
		SaveObjManager.OnBeforeSave -= StoreData;
		SaveObjManager.OnBeforeSave -= ApplyData;
	}*/
}


[Serializable]
public class CharData {
	public string regID = "Not Assigned";
	public string name;
	public Vector3 pos;
	public float health;
	public string prefabPath;
	public string targetID;
	public string targetDir;
}