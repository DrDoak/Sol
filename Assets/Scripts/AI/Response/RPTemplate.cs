using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPTemplate {
	public string template;
	public string speechGroup;
	public Assertion templateAssertion;
	Character mChar;

	public void setSpeaker(Character speaker) {
		//Debug.Log ("Template setting to: " + speaker);
		templateAssertion.SetOwner (speaker);
		mChar = speaker;
	}
	public bool match(Assertion other) {
		//Debug.Log ("Match me: " + templateAssertion.GetID () + " with " + other.GetID ());
		//Debug.Log ("Char is: " + a.mChar + " mine is: " + mChar);
		return templateAssertion.IsMatch (other);
	}
	public bool match(string exclamation) {
		return (templateAssertion.Subjects [0].GetID () == exclamation);
	}
	public bool match(KNSubject s) {
		return false;
	}
	public bool match(KNVerb v) {
		return false;
	}
}