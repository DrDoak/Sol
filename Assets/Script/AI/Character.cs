using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PersItem))]
[RequireComponent (typeof (Movement))]
[RequireComponent (typeof (Fighter))]
[RequireComponent (typeof (Attackable))]
public class Character : Interactable {
	public CharData data = new CharData();

	public bool aggression = true;
	public string name = "default";
	public float detectionRange = 15.0f;
	public float absoluteDetection = 1.0f;
	public string faction = "noFaction";
	public bool facingLeft = false;
	public float health = 100.0f;
	public float healthPerc = 1.0f;
	float sinceLastScan;
	float scanInterval = 0.5f;
	float postLineVisibleTime = 3.0f;
	TextboxManager tm;
	Movement movt;
	List<Character> visibleCharacters = new List<Character>();
	List<Character> observers = new List<Character>();
	Dictionary<Character,Relationship> charInfo = new Dictionary<Character,Relationship> ();
	bool awaitingDialogue = false;
	bool choosingDialogue = false;
	DialogueSequence presetDS;
	bool isPresetDS;
	GameObject dBox;
	public bool recreated = false;
	bool registryChecked = false;

	bool autonomy = false;

	float interactRange = 0.9f;

	//Skills:
	public float perception = 0.0f;
	public float persuasion = 0.0f;
	public float logic = 0.0f;
	public Personality pers;

	public void setAutonomy(bool active) {
		autonomy = active;
		if (GetComponent<NPC> ()) {
			GetComponent<NPC> ().autonomy = active;
		}
		if (GetComponent<Player> ()) {
			GetComponent<Player> ().autonomy = active;
		}
	}
	void Start () {
		init ();
	}
	protected void init() {
		movt = GetComponent<Movement> ();
		tm = FindObjectOfType<TextboxManager> ();
		sinceLastScan = UnityEngine.Random.Range (0.0f, scanInterval);
		//charInfo = new Dictionary<Character,CharacterInfo> ();
		//visibleCharacters 
		//observers = new List<Character> ();
		if (faction == "noFaction" && GetComponent<Attackable> ()) {
			faction = GetComponent<Attackable> ().faction;
		}
		pers = new Personality ();
		//FindObjectOfType<CharacterManager> ().registerChar (this);
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
		updateScan ();
		if (choosingDialogue) {
		}
	}
	//Dialogue and interaction
	public override void onInteract(Character interactor) {
		if (isPresetDS) {
			presetDS.initiateDialogue ();
		} else {
			interactor.initiateDialogueRequest (this);
		}
	}
	public void attemptInteraction(Interactable i) {
		float cDist = Vector3.Distance (i.transform.position, transform.position);
		if (cDist < interactRange) {
			Debug.Log ("Attempting Interaction");
			InteractEvent ie = new InteractEvent ();
			broadcastToObservers (ie);
			i.onInteract (this);
		}
	}

	//Dialogue conditions
	public void setDialogueSequence(DialogueSequence ds) {
		isPresetDS = true;
		ds.speaker = this;
		presetDS = ds;
	}
	public void onTBComplete() {
		presetDS.parseNextElement ();
	}
	public virtual void initiateDialogueRequest(Character targetChar) {
		//DialogueElement dialogOpt = chooseDialogueOption (getDialogueOptions(targetChar,null));
		//targetChar.processDialogueRequest (this, dialogOpt);
	}
	public virtual List<DialogueElement> getDialogueOptions(Character otherChar, DialogueElement prevDialogue) {
		List<DialogueElement> dList = new List<DialogueElement> ();
		return dList;
	}
	public virtual DialogueSequence chooseDialogueOption(List<DialogueSequence> dList) {
		choosingDialogue = true;
		return null;
	}
	public virtual void processDialogueRequest(Character c,DialogueSequence d) {}
	public virtual void acceptDialogue(Character c,DialogueSequence d) {}

	public void playerInteraction() {
		Interactable[] allIter = FindObjectsOfType<Interactable> ();
		float maxPriority = 0f;
		Interactable interObj = null;
		foreach (Interactable c in allIter) {
			if (c != this  && c.transform.position.x < transform.position.x && movt.facingLeft || 
				c.transform.position.x > transform.position.x && !movt.facingLeft) {
				float cDist = Vector3.Distance (c.transform.position, transform.position);
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
		if (maxPriority > 0f) {
			attemptInteraction (interObj);
		}
	}
	protected void updateScan() {
		if (sinceLastScan > scanInterval) {
			scanForEnemies ();
		}
		sinceLastScan += Time.deltaTime;
	}
	public virtual void respondToEvent(Event e) {}

	//Enemy detection
	void scanForEnemies() {
		Character[] allChars = FindObjectsOfType<Character> ();
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
							cin.lastTimeSeen = Time.realtimeSinceStartup;
							charInfo.Add (c, cin);
						} else {
							charInfo [c].lastTimeSeen = Time.realtimeSinceStartup;
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
				} else if (Time.realtimeSinceStartup - charInfo [c].lastTimeSeen > postLineVisibleTime) {
					c.removeObserver (this);
					outOfSight (c);
					visibleCharacters.RemoveAt (i);
				}
			}
		}

		sinceLastScan = 0f;
	}
	public virtual void onSight(Character otherChar) {
		SightEvent se = new SightEvent ();
		respondToEvent (se);
	}
	public virtual void outOfSight(Character otherChar) {
		SightEvent se = new SightEvent ();
		respondToEvent (se);
	}
	public Relationship getCharInfo(Character c) {
		Debug.Log (name + " Getting character: " + c.name);
		if (charInfo.ContainsKey (c)) {
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
	protected void say(string text) {
		tm.addTextbox (text, gameObject, true);
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