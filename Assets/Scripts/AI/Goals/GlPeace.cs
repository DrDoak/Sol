using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlPeace : Goal {

	public GlPeace() {
		registerEvent ("hit",hitMe,expressPain,"attack");
	}

	float hitMe(Event e) {
		EVHitConfirm eva = (EVHitConfirm)e;
		if (eva.ObjectHit == mChar.gameObject) {
			return 0.5f;
		}
		return 0f;
	}
	void expressPain(Proposal p) {
		EVHitConfirm evh = (EVHitConfirm)p.mEvent;
		mChar.speaker.EmitResponse(mChar.speaker.Convey ("stop",evh.attacker));
	}
}
