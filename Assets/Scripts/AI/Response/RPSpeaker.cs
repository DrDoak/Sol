using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPSpeaker : MonoBehaviour {

	//public string rpDatabasePath;
	DialogueParser dp;
	Character c;
	List<RPTemplate> fullRPs;
	List<RPTemplate> VerbRPs;
	List<RPEntry> log;
	RPDatabase rpd;

	// Use this for initialization
	void Start () {
		c = GetComponent<Character> ();
		dp = GetComponent<DialogueParser> ();
		rpd = FindObjectOfType<RPDatabase> ();
		log = new List<RPEntry> ();
	}
	public Response ConveySubject( Assertion a, Character listener) {
		if (!a.HasSubject)
			return new Response ();
		return Convey(a.Subjects[0],listener);
	}
	public Response ConveyVerb( Assertion a, Character listener) {
		if (!a.HasVerb)
			return new Response ();
		return Convey(a.Verb,listener);
	}
	public Response ConveyReceivor( Assertion a, Character listener) {
		if (!a.HasReceivor)
			return new Response ();
		Debug.Log ("DO: " + a.Receivors [0].GetID ());
		return Convey(a.Receivors[0],listener);
	}
	public Response Convey(KNSubject s, Character listener) {
		Response r = new Response ();
		r.mChar = c;
		r.speaker = this;
		r.listener = listener;
		List<RPTemplate> fullR = rpd.GetMatches (s,c);
		RPTemplate best = GetBestResponse (fullR,r);
		if (best != null) {
			r.applyTemplate (best);
		} else {
			r.setString (s.Convey ());
		}
		return r;
	}
	public Response Convey(KNVerb v, Character listener) {
		Response r = new Response ();
		r.mChar = c;
		r.speaker = this;
		r.listener = listener;
		List<RPTemplate> fullR = rpd.GetMatches (v,c);
		RPTemplate best = GetBestResponse (fullR,r);
		if (best != null) {
			r.applyTemplate (best);
		} else {
			r.setString (v.Convey ());
		}
		return r;
	}
	public Response Convey(Assertion a,Character listener) {
		Debug.Log ("Conveying an assertion: " + a.GetID ());
		Response r = new Response ();
		r.mChar = c;
		r.speaker = this;
		r.listener = listener;
		r.setAssertion (a);
		List<RPTemplate> fullR = rpd.GetMatches (a,c);
		RPTemplate best = GetBestResponse (fullR,r);
		if (best != null) {
			r.applyTemplate (best);
		} else {
			r.setString (a.Convey ());
		}
		return r;
	}
	public void EmitResponse(Response r) {
		//Debug.Log ("emitting response!");
		RPEntry newEntry = new RPEntry ();
		newEntry.response = r;
		log.Add (newEntry);
		c.say (r.toString ());
	}
	public RPTemplate GetBestResponse(List<RPTemplate> responses,Response r) {
		float max = 0f;
		RPTemplate bestRP = null;
		foreach (RPTemplate rp in responses) {
			float rating = RateResponse (rp,r);
			if (rating > max) {
				max = rating;
				bestRP = rp;
			}
		}
		return bestRP;
	}

	public float RateResponse(RPTemplate rp,Response r) {
		return 1.0f;
	}
}
