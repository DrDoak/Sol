using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPSpeaker : MonoBehaviour {

	public string rpDatabasePath;
	DialogueParser dp;
	List<RPTemplate> fullRPs;
	List<RPTemplate> verbRPs;
	List<RPEntry> log;

	public Response convey(Assertion a) {
		dp.say (a.getID());
		Response r = new Response ();
		return r;
	}

	// Use this for initialization
	void Start () {}
	// Update is called once per frame
	void Update () {}

	public void initDatabase(string path) {
	}

	public void rateResponse() {
	}

	List<RPPiece> getFullOptions(Assertion a) {
		List<RPPiece> responses = new List<RPPiece> ();
		foreach (RPTemplate r in fullRPs) {
			if (r.match (a)) {
				responses.Add (r.toPiece(a));
			}
		}
		return responses;
	}
}
