using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPSpeaker : MonoBehaviour {

	//public string rpDatabasePath;
	DialogueParser dp;
	Character c;
	List<RPTemplate> fullRPs;
	List<RPTemplate> VerbRPs;
	List<RPEntry> ResponseLog;
	RPDatabase rpd;

	float FORGET_TIME = 500f;

	// Use this for initialization
	void Start () {
		c = GetComponent<Character> ();
		dp = GetComponent<DialogueParser> ();
		rpd = FindObjectOfType<RPDatabase> ();
		ResponseLog = new List<RPEntry> ();
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
		return Convey(a.Receivors[0],listener);
	}
	public Response Convey(string exclamation, Character listener) {
		Response r = new Response ();
		r.mChar = c;
		r.speaker = this;
		r.listener = listener;
		List<RPTemplate> fullR = rpd.GetMatches (exclamation,c);
		RPTemplate best = GetBestResponse (fullR,r);
		if (best != null) {
			r.ApplyTemplate (best);
		} else {
			r.SetString (exclamation);
		}
		return r;
	}
	public Response Convey(KNSubject s, Character listener) {
		Response r = new Response ();
		r.mChar = c;
		r.speaker = this;
		r.listener = listener;
		List<RPTemplate> fullR = rpd.GetMatches (s,c);
		RPTemplate best = GetBestResponse (fullR,r);
		if (best != null) {
			r.ApplyTemplate (best);
		} else {
			r.SetString (s.Convey ());
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
			r.ApplyTemplate (best);
		} else {
			r.SetString (v.Convey ());
		}
		return r;
	}
	public Response Convey(Assertion a,Character listener) {
		Debug.Log ("Conveying an assertion: " + a.GetID ());
		Response r = new Response ();
		r.mChar = c;
		r.speaker = this;
		r.listener = listener;
		r.SetAssertion (a);
		List<RPTemplate> fullR = rpd.GetMatches (a,c);
		RPTemplate best = GetBestResponse (fullR,r);
		if (best != null) {
			r.ApplyTemplate (best);
		} else {
			r.SetString (a.Convey ());
		}
		return r;
	}
	public void EmitResponse(Response r) {
		//Debug.Log ("emitting response!");
		RPEntry newEntry = new RPEntry ();
		newEntry.lastTime = GameManager.GameTime;
		newEntry.response = r;
		newEntry.Template = r.Template;
		ResponseLog.Add (newEntry);
		//Debug.Log ("Emitting string: " + r.ToString ());
		c.say (r.ToString ());
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

	public float RateResponse(RPTemplate rp,Response r) {return freshness(rp);}

	float freshness(RPTemplate rp, float decayScale = 1f, float randomScale = 1f) {
		float f = 1.0f;
		List<RPEntry> newLog = new List<RPEntry>();
		foreach(RPEntry re in ResponseLog) {
			if (re.Template.OutputTemplate == rp.OutputTemplate) {
				f -= decayScale * 0.1f + ((Random.value - 0.5f) * randomScale * 0.1f);
			}
		}
		return f;
	}
}
