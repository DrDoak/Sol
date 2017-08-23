using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PersItem))]
[RequireComponent (typeof (Movement))]
[RequireComponent (typeof (Fighter))]
[RequireComponent (typeof (Attackable))]
[RequireComponent (typeof (DialogueParser))]
public class Character : Interactable {
	//Basic references
	new public string name = "default";
	public float detectionRange = 15.0f;
	public string faction = "noFaction";
	public bool facingLeft = false;
	public float health = 100.0f;
	public float healthPerc = 1.0f;
	Movement movt;
	DialogueParser parser;

	//Saving
	public bool recreated = false;
	bool registryChecked = false;

	bool autonomy = false;

	float interactRange = 2.0f;
	//Dialogue 
	DialogueUnit presetDS;
	bool isPresetDS;
	bool choosingDialogue = false;
	TextboxManager tm;
	public RuntimeAnimatorController animDefault;
	public RuntimeAnimatorController animCutscene;

	//Skills:
	public float perception = 0.0f;
	public float persuasion = 0.0f;
	public float logic = 0.0f;
	public Personality pers;

	//Memory:
	public CharData data = new CharData();
	Dictionary<string,Fact> knowledge = new Dictionary<string,Fact>();
	List<Character> visibleCharacters = new List<Character>();
	List<Character> observers = new List<Character>();
	float sinceLastScan;
	float scanInterval = 0.5f;
	float postLineVisibleTime = 3.0f;
	Dictionary<Character,Relationship> charInfo = new Dictionary<Character,Relationship> ();


	public virtual void setAutonomy(bool active) {
//		Debug.Log ("Setting autonomy " + gameObject + " bool " + active);
		autonomy = active;
		if (GetComponent<NPC> ()) {
			GetComponent<NPC> ().setAutonomy(active);
		}
		if (GetComponent<Player> ()) {
			GetComponent<Player> ().autonomy = active;
		}
	}
	void Start () {
		init ();
	}
	protected void init() {
//		Debug.Log ("init from character");
		movt = GetComponent<Movement> ();
		tm = FindObjectOfType<TextboxManager> ();

		parser = GetComponent<DialogueParser> ();
		sinceLastScan = UnityEngine.Random.Range (0.0f, scanInterval);
		//charInfo = new Dictionary<Character,CharacterInfo> ();
		//visibleCharacters 
		//observers = new List<Character> ();
		if (faction == "noFaction" && GetComponent<Attackable> ()) {
			faction = GetComponent<Attackable> ().faction;
		}
		pers = new Personality ();
		FindObjectOfType<CharacterManager> ().registerChar (this);
	}
	public void registryCheck() {
		if (data.regID == "") {
			data.regID = "Not Assigned";
		}
		if (FindObjectOfType<GameManager>().checkRegistered(gameObject)) {
			Destroy(gameObject);
		}
		registryChecked = true;
	}
	void Update () {
		if (!registryChecked) {
			registryCheck ();
		}
		mUpdate ();
	}
	//Dialogue and interaction
	public override void onInteract(Character interactor) { }
	public void attemptInteraction(Interactable i) {
		float cDist = Vector3.Distance (i.transform.position, transform.position);
		//Debug.Log ("actual Dist: " + cDist + " :quota: " + interactRange + " true; " + (cDist < interactRange));
		if (cDist < interactRange) {
			Debug.Log ("Attempting Interaction");
			InteractEvent ie = new InteractEvent ();
			ie.targetChar = this;
			if (i.GetComponent<Character> ()) {
				ie.isCharInteraction = true;
				ie.listenerChar = i.GetComponent<Character> ();
			} else {
				ie.targetedObj = i;
			}
			broadcastToObservers (ie);
			i.onInteract (this);
		}
	}

	//Dialogue conditions
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
	public virtual void initiateDialogueRequest(Character targetChar) {
		Debug.Log ("dialogue Request initiated");
		InteractEvent se = new InteractEvent ();
		respondToEvent (se);
	}
	public virtual List<DialogueOption> getDialogueOptions(Character otherChar) {
		List<DialogueOption> options = new List<DialogueOption> (3);
		DialogueOption gossip = new DialogueOption ();
		gossip.text = "Gossip";
		gossip.responseFunction = startGossip;
		options.Add (gossip);
		DialogueOption askAbout = new DialogueOption ();
		askAbout.text = "Ask About...";
		askAbout.responseFunction = startAsk;
		options.Add (askAbout);
		DialogueOption leave = new DialogueOption ();
		leave.text = "Leave";
		leave.responseFunction = startGossip;
		//leave.setToReturn();
		options.Add (leave);

		return options;
	}
	void startGossip(DialogueOption d) {}

	void startAsk(DialogueOption d) {}
	public virtual DialogueSubunit chooseDialogueOption(List<DialogueSubunit> dList) {
		choosingDialogue = true;
		return null;
	}
	public virtual void processDialogueRequest(Character c,DialogueUnit d) {}
	public virtual void acceptDialogue(Character c,DialogueUnit d) {}

	public void playerInteraction() {
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
	protected void mUpdate() {
		if (sinceLastScan > scanInterval) {
			scanForEnemies ();
		}
		sinceLastScan += Time.deltaTime;
	}
	public virtual void respondToEvent(Event e) {}

	//Enemy detection
	void scanForEnemies() {
		Character[] allChars = FindObjectsOfType<Character> ();
		float lts = Time.realtimeSinceStartup;
		foreach (Character c in allChars) {
			if (c != this  && c.transform.position.x < transform.position.x && movt.facingLeft || 
				c.transform.position.x > transform.position.x && !movt.facingLeft) {
				float cDist = Vector3.Distance (c.transform.position, transform.position);
				if (cDist < detectionRange) {
					RaycastHit2D [] hits = Physics2D.RaycastAll(transform.position,c.transform.position - transform.position,cDist);
					Debug.DrawRay(transform.position,c.transform.position - transform.position,Color.green);
					float minDist = float.MaxValue;
					foreach (RaycastHit2D h in hits) {
						GameObject o = h.collider.gameObject;
						if (o != gameObject ) {
							minDist = Mathf.Min(minDist,Vector3.Distance (transform.position,h.point));
						}
					}
					float diff = Mathf.Abs (cDist - minDist);
					if ( diff < 1.0f) {
						if (!charInfo.ContainsKey (c)) {
							Relationship cin = new Relationship ();
							cin.lastTimeSeen = lts;
							charInfo.Add (c, cin);
						} else {
							charInfo [c].lastTimeSeen = lts;
							charInfo [c].canSee = true;
						}
						if (!visibleCharacters.Contains (c)) {
							onSight (c);
							c.addObserver (this);
							visibleCharacters.Add (c);
						}
					}
				}
			}
		}
		if (visibleCharacters.Count > 0) {
			for (int i= visibleCharacters.Count - 1; i >= 0; i --) {
				Character c = visibleCharacters [i];
				if (c == null || c.gameObject == null) {
					visibleCharacters.RemoveAt (i);
				} else if (lts - charInfo [c].lastTimeSeen > postLineVisibleTime) {
					c.removeObserver (this);
					outOfSight (c, true);
					visibleCharacters.RemoveAt (i);
				} else if (Math.Abs(lts - charInfo [c].lastTimeSeen) > 0.05f 
						&& charInfo [c].canSee == true){
					charInfo [c].canSee = false;
					outOfSight (c, false);
				}
			}
		}

		sinceLastScan = 0f;
	}
	public virtual void onSight(Character otherChar) {
		SightEvent se = new SightEvent ();
		respondToEvent (se);
	}
	public virtual void outOfSight(Character otherChar,bool full) {
		if (full) {
		} else {
			SightEvent se = new SightEvent ();
			se.onSight = false;
			respondToEvent (se);
		}
	}
	public virtual void setTargetPoint(Vector3 targetPoint, float proximity) {
		GetComponent<Player> ().setTargetPoint (targetPoint, proximity);
	}
	public Relationship getCharInfo(Character c) {
		if (c == null) {
			Debug.Log ("Why is the character null?");
			return null;
		} else if (charInfo.ContainsKey (c)) {
			return charInfo [c];
		} else {
			return evaluateNewChar (c);
		}
	}
	public Relationship evaluateNewChar(Character c) {
		Relationship ci = new Relationship ();
		charInfo.Add (c, ci);
		return ci;
	}
//Observers
	public void addObserver(Character obs) {
		if (!observers.Contains (obs)) {
			observers.Add (obs);
		}
	}
	public void removeObserver(Character obs) {
		if (observers.Contains(obs)) {
			observers.Remove (obs);
		}
	}
		
	public void broadcastToObservers(Event e) {
		foreach (Character c in observers) {	
			c.respondToEvent (e);
		}
	}
	void OnDestroy() {
		foreach (Character c in visibleCharacters) {
			c.removeObserver (this);	
		}
	}
//Speech
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
	public void StoreData() {
		//Debug.Log ("storing data");
		data.name = gameObject.name;
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
		data.prefabPath = properName; //gameObject.name;
	}

	public void LoadData() {
		name = data.name;
		transform.position = data.pos;
		GetComponent<Attackable>().health = data.health;
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
//-----------Knowledge------------------
	public void addFact(Fact k) {
		knowledge.Add (k.factID, k);
	}
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