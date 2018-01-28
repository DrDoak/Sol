using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlObedience : Goal {
	public GlObedience() {
		registerEvent (EventType.Command, attackCommand);
	}

	void attackCommand(Event e) {
		EVCommand evf = (EVCommand)e;
		Assertion a = e.assertion;
		Character commander = evf.Commander;
		Relationship r = mChar.getCharInfo (commander);
		if (!r.openHostile && a.Verb.VerbName == "attack") {
			Character targetChar = CharacterManager.FindChar (a.Receivors [0].SubjectName);
			if (targetChar != null && targetChar != mChar) {
				float supportChar = r.GetAuthorityScaled ();
				float supportEnemy = r.GetFavorScaled () + (mChar.PersonalityData.agreeableness * 0.05f) + 0.01f;
				if (supportEnemy < 0)
					supportChar -= supportEnemy;
				if (supportEnemy > supportChar) {
					mChar.AddProposal (refuseAttack, e, supportEnemy);
				} else {
					mChar.AddProposal (executeAttack, e, supportChar);
				}
			}
		}
	}

	public void refuseAttack(Proposal p) {
		EVCommand evf = (EVCommand)p.mEvent;
		mChar.speaker.EmitResponse(mChar.speaker.Convey ("no", evf.Commander));
	}
	public void executeAttack(Proposal p) {
		//Debug.Log ("OpenAllegience: " + p.opennessAllegiance + " Favor rating: " + favor);
		EVCommand evf = (EVCommand)p.mEvent;
		Character targetChar = CharacterManager.FindChar (evf.assertion.Receivors [0].SubjectName);
		mChar.GetComponent<OffenseAI> ().setTarget (targetChar);
	}
}
