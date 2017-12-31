using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlMakeFriends : Goal {

	public GlMakeFriends() {
		registerEvent (EventType.Exclamation,greeting,respond, ProposalClass.Verbal);
		registerEvent (EventType.Sight, sawFriend, respondToSight, ProposalClass.Verbal);
	}

	float sawFriend(Event e) {
		EVSight es = (EVSight)e;
		if (es.ObservedChar != null && es.onSight) {
			Relationship r = mChar.getCharInfo (es.ObservedChar);
			if (!r.openHostile) {
				Assertion a = new Assertion ();
				a.AddSubject (KNManager.CopySubject (mChar.name));
				a.AddVerb (KNManager.CopyVerb ("sight"));
				a.AddReceivor (KNManager.CopySubject (es.ObservedChar.name));
				float decVal = mChar.knowledgeBase.GetDecayRatio (a,120f) - 0.5f;
				return decVal;
			}
		}
		return 0.0f;
	}
	void respondToSight(Proposal p) {
		EVSight es = (EVSight)p.mEvent;
		mChar.speaker.EmitResponse(mChar.speaker.Convey("greeting", es.ObservedChar));
	}
	float greeting(Event e) {
		EVExclamation eva = (EVExclamation)e;
		if (eva.Exclamation == "greeting") {
			return 0.5f;
		}
		return 0f;
	}
	void respond(Proposal p) {
		EVExclamation evh = (EVExclamation)p.mEvent;
		mChar.speaker.EmitResponse(mChar.speaker.Convey ("greeting",evh.speaker));
	}
}
