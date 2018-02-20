using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlProtectFriend : Goal {

	public GlProtectFriend() {
		registerEvent (EventType.Hit,hitFriend,initiateAttack,ProposalClass.Action);
	}

	float hitFriend(Event e) {
		EVHitConfirm eva = (EVHitConfirm)e;
		if (eva.ObjectHit.GetComponent<Character>()) {
			Character victim = eva.ObjectHit.GetComponent<Character> ();
			if (victim.name != mChar.name) {
				Personality p = mChar.PersonalityData;
				Relationship r = mChar.getCharInfo (eva.attacker);
				Relationship vr = mChar.getCharInfo (victim);
				Assertion a = new Assertion ();
				a.AddSubject (KNManager.CopySubject (eva.attacker.name));
				a.AddVerb (KNManager.CopyVerb ("attack"));
				a.AddReceivor (KNManager.CopySubject (victim.name));
				float scale = mChar.knowledgeBase.GetScaleRatio (a, 1800f) * ((-r.favorability * (1 + p.opennessAllegiance)) +
				              (0.25f + (p.temperament * 0.05f) - (p.agreeableness * 0.05f) - (r.authority * (1f - r.affirmation))));
				mChar.getCharInfo (eva.attacker).ChangeFavor (-0.05f * mChar.knowledgeBase.GetScaleRatio (a, 1800f, 1.0f));
				return scale;
			}
		}
		return 0f;
	}
	void initiateAttack(Proposal p) {
		EVAttack eva = (EVAttack)p.mEvent;
		mChar.SetTarget (eva.attacker);
	}
}
