using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlPeace : Goal {

	public GlPeace() {
		registerEvent (EventType.Hit,hitMe,expressPain,ProposalClass.Action);
	}

	float hitMe(Event e) {
		EVHitConfirm eva = (EVHitConfirm)e;
		if (eva.ObjectHit.GetComponent<Character>()) {
			Personality pers = mChar.PersonalityData;
			Relationship r = mChar.getCharInfo (eva.attacker);
			float peaceVal = 0.4f + pers.agreeableness - (pers.temperament * 0.2f) + r.GetFavorScaled();
			return peaceVal;
		}
		return 0f;
	}
	void expressPain(Proposal p) {
		EVHitConfirm evh = (EVHitConfirm)p.mEvent;
		mChar.speaker.EmitResponse(mChar.speaker.Convey ("stop",evh.attacker));
	}
}
