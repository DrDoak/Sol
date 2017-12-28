using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlObediance : Goal {
	public GlObediance() {
		registerEvent ("command", attackCommand,executeAttack);
	}

	float attackCommand(Event e) {
		Debug.Log ("Attack event");
		EVCommand evf = (EVCommand)e;
		Assertion a = e.assertion;
		Debug.Log ("Received command event: " + evf.Commander.name + " wants " + a.Verb.VerbName);
		Character commander = evf.Commander;
		Relationship r = mChar.getCharInfo (commander);
		if (!r.openHostile && a.Verb.VerbName == "attack") {
			Character targetChar = CharacterManager.FindChar (a.Receivors [0].SubjectName);
			Debug.Log ("Found target: " + targetChar);
			if (targetChar != null && targetChar != mChar) {
				return 1.0f;
			}
		}
		return 0f;
	}

	public void executeAttack(Proposal p) {
		//Debug.Log ("OpenAllegience: " + p.opennessAllegiance + " Favor rating: " + favor);
		EVCommand evf = (EVCommand)p.mEvent;
		Character targetChar = CharacterManager.FindChar (evf.assertion.Receivors [0].SubjectName);
		mChar.GetComponent<OffenseAI> ().setTarget (targetChar);
	}
}
