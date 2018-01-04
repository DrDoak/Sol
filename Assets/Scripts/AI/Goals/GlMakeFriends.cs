using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlMakeFriends : Goal {

	public GlMakeFriends() {
		registerEvent (EventType.Exclamation, greeting);
		registerEvent (EventType.Sight, sawFriend, respondToSight, ProposalClass.Relationship);
		registerEvent (EventType.Ask, answerFriendQuestions);
	}
		
	void answerFriendQuestions(Event e) {
		EVAsk eva = (EVAsk)e;
		List<Assertion> matches = mChar.knowledgeBase.GetMatches (eva.assertion);
		//Debug.Log ("Answering questions, matches: " + matches.Count);
		if (matches.Count > 0) {
			foreach (Assertion match in matches) {
				Debug.Log (match.GetID ());
				//mChar.AddProposal (sawFriend, e, 0.5f);
			}
		}			
	}

	float sawFriend(Event e) {
		EVSight es = (EVSight)e;
		if (es.ObservedChar != null && es.onSight) {
			Relationship r = mChar.getCharInfo (es.ObservedChar);
			if (!r.openHostile) {
				Personality p = mChar.PersonalityData;
				Assertion a = new Assertion ();
				a.AddSubject (KNManager.CopySubject (mChar.name));
				a.AddVerb (KNManager.CopyVerb ("sight"));
				a.AddReceivor (KNManager.CopySubject (es.ObservedChar.name));
				float decVal = mChar.knowledgeBase.GetDecayRatio (a,120f) - (0.6f - p.agreeableness);
				return decVal;
			}
		}
		return 0.0f;
	}
	void respondToSight(Proposal p) {
		EVSight es = (EVSight)p.mEvent;
		mChar.speaker.EmitResponse(mChar.speaker.Convey("greeting", es.ObservedChar));
	}
	void greeting(Event e) {
		EVExclamation eve = (EVExclamation)e;
		Relationship r = mChar.getCharInfo (eve.speaker);
		if (eve.Exclamation == "greeting" && !r.openHostile) {
			Personality pers = mChar.PersonalityData;

			Assertion a = new Assertion ();
			a.AddSubject (KNManager.CopySubject (eve.speaker.name));
			a.AddVerb (KNManager.CopyVerb ("greet"));
			a.AddReceivor (KNManager.CopySubject (mChar.name));
			float decRatio = mChar.knowledgeBase.GetDecayRatio (a, 1200f, 0.8f, true);
			r.ChangeFavor (decRatio * (0.1f + 0.1f * pers.agreeableness));
			//Debug.Log ("Decratio: " + decRatio + " Favor Scaled: " + r.GetFavorScaled() + " F: " + r.favorability);
			float agreeableRating = 0.1f + (0.2f * pers.agreeableness) + (r.GetFavorScaled ()) - (0.1f * mChar.knowledgeBase.GetScaleRatio (a, 120f));
			//Debug.Log("Respond positively: " + agreeableRating);
			if (agreeableRating > 0) {
				mChar.AddProposal (respond, e, agreeableRating);
			} else {
				mChar.AddProposal (respondAnnoy, e, -agreeableRating);
			}
		} else {
			mChar.AddProposal (responseConfuse, e, 0.1f);
		}
	}

	void responseConfuse(Proposal p ) {
		EVExclamation evh = (EVExclamation)p.mEvent;
		mChar.speaker.EmitResponse(mChar.speaker.Convey ("confusion",evh.speaker));
	}
	void respond(Proposal p) {
		EVExclamation evh = (EVExclamation)p.mEvent;
		mChar.speaker.EmitResponse(mChar.speaker.Convey ("greeting",evh.speaker));
	}
	void respondAnnoy(Proposal p) {
		EVExclamation evh = (EVExclamation)p.mEvent;
		mChar.speaker.EmitResponse(mChar.speaker.Convey ("disinterest",evh.speaker));
	}
}
