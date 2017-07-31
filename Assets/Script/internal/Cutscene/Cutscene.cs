using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cutscene : MonoBehaviour {
	GameManager gm;
	CharacterManager cm;

	List<CutscenePiece> eventList;
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
	}
	public void init() {
		gm = GameObject.FindObjectOfType<GameManager> ();
		cm = GameObject.FindObjectOfType<CharacterManager> ();
		lockedCharacters = new List<Character> ();
		eventList = new List<CutscenePiece> (GetComponents<CutscenePiece> ());
		eventList.Sort((p1,p2)=>p1.order.CompareTo(p2.order));
		if (instantStart) {
			toStart = true;
		}
	}
	public virtual void cutsceneUpdate(float dt) {
		currentEvent.activeTick (dt);
	}

	public void lockCharacter(string charName) {
		Debug.Log ("searching for char:" + charName);
		Character c = cm.findChar (charName);
		Debug.Log (c);
		c.setAutonomy (false);
		lockedCharacters.Add (c);
	}
	public void lockCharacter(Character searchC) { 
		Character c = cm.findChar(searchC);
		c.setAutonomy (false);
		lockedCharacters.Add (c);
	}
	public void concludeCutscene () {
		foreach (Character c in lockedCharacters) {
			c.setAutonomy (true);
		}
		gm.concludeCutscene (this);
		Destroy (gameObject);
	}
	public void addEvent(CutscenePiece cp) {
		eventList.Add (cp);
		cp.parent = this;
		cp.gm = gm;
		cp.order = eventList.Count;
	}
	public void startCutscene() {
		Debug.Log ("Starting Cutscene!!!");
		foreach (CutscenePiece cp in eventList) {
			cp.parent = this;
			cp.gm = gm;
			cp.cm = cm;
			if (cp.targetCharName != "notSet") {
				lockCharacter (cp.targetCharName);
			}
		}
		currentEvent = eventList [0];
		currentEvent.onEventStart ();
	}
	public void progressEvent() {
		eventList.Remove (currentEvent);
		if (eventList.Count > 0) {
			currentEvent = eventList [0];
			currentEvent.onEventStart ();
		} else {
			concludeCutscene ();
		}
	}
}
