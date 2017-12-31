using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlMakeFriends : Goal {

	public GlMakeFriends() {
		registerEvent ("exclamation",greeting,respond, "verbal");
		registerEvent ("sight", sawFriend, respondToSight, "verbal");

	}

	float sawFriend(Event e) {
		EVSight es = (EVSight)e;
		if (es.ObservedChar != null) {
			Assertion a = new Assertion ();
			a.AddSubject (KNManager.CopySubject (mChar.name));
			a.AddVerb (KNManager.CopyVerb("sight"));
			a.AddReceivor(KNManager.CopySubject(es.ObservedChar.name));
			float decVal = mChar.knowledgeBase.GetDecayRatio (a) - 0.5f;
			Debug.Log ("Exclaim rating: " + decVal);
			return decVal;
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
