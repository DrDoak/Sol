using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cutscene : MonoBehaviour {
	GameManager gm;
	CharacterManager cm;

	List<CutscenePiece> eventList = new List<CutscenePiece>();
	List<Character> lockedCharacters;
	CutscenePiece currentEvent;
	public bool instantStart = true;
	bool toStart = false;
	void Start() {
		init ();
	}
	void Update() {
		if (toStart) {
			toStart = false;
			startCutscene ();
		}
		if (currentEvent) {
			cutsceneUpdate (Time.deltaTime);
		}
	}
	public void init() {
		gm = GameObject.FindObjectOfType<GameManager> ();
		cm = GameObject.FindObjectOfType<CharacterManager> ();
		if (instantStart) {
			toStart = true;
		}
	}
	public virtual void cutsceneUpdate(float dt) {
		currentEvent.activeTick (dt);
	}

	public void lockCharacter(string charName) {
		//Debug.Log ("Attempting to lock character: " + charName);
		Character c = cm.findChar (charName);
		c.setAutonomy (false);
		lockedCharacters.Add (c);
	}
	public void lockCharacter(Character searchC) { 
		//Debug.Log ("Attempting to lock character: " + searchC.name);
		Character c = cm.findChar(searchC);
		c.setAutonomy (false);
		lockedCharacters.Add (c);
		//Debug.Log ("Done with lock");
	}
	public void concludeCutscene () {
		foreach (Character c in lockedCharacters) {
			//Debug.Log ("unlocking char:" + c.name);
			c.setAutonomy (true);
		}
		gm.concludeCutscene (this);
		Destroy (gameObject);
	}
	public void addEvent(CutscenePiece cp) {
		eventList.Add (cp);
		cp.parent = this;
		cp.gm = gm;
		if (cp.order == 0) {
			cp.order = eventList.Count;
		}
		eventList.Sort((p1,p2)=>p1.order.CompareTo(p2.order));
	}
	public void startCutscene() {
		//Debug.Log ("Starting CS. CPs:");
		lockedCharacters = new List<Character> ();
		foreach (CutscenePiece cp in eventList) {
			cp.parent = this;
			cp.gm = gm;
			cp.cm = cm;
			//Debug.Log ("LOCKING:" + cp.targetCharName);
			if (cp.targetCharName != "notSet") {
				lockCharacter (cp.targetCharName);
			}
		}
		currentEvent = eventList [0];
		currentEvent.onEventStart ();
	}
	public void progressEvent() {
		//Debug.Log ("Progressing to next cutscene");
		currentEvent.onComplete ();
		eventList.Remove (currentEvent);
		if (eventList.Count > 0) {
			currentEvent = eventList [0];
			currentEvent.onEventStart ();
		} else {
			Debug.Log ("no more, concluding");
			concludeCutscene ();
		}
	}
}
