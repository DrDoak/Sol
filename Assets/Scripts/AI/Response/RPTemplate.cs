using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPTemplate {
	public string OutputTemplate;
	public string speechGroup;
	public Assertion templateAssertion;
	protected Character mChar;
	protected Character listener;

	public void SetSpeaker(Character speaker) {
		//Debug.Log ("Template setting to: " + speaker);
		if (templateAssertion != null)
			templateAssertion.SetOwner (speaker);
		mChar = speaker;
	}
	public void SetListener(Character l) {
		listener = l;
	}
	public virtual bool match(Assertion other) {
		//Debug.Log ("Match me: " + templateAssertion.GetID () + " with " + other.GetID ());
		//Debug.Log ("Char is: " + a.mChar + " mine is: " + mChar);
		if (templateAssertion != null) {
			return templateAssertion.IsMatch (other);
		} else {
			return false;
		}
	}
	public virtual bool match(string exclamation) {
		return (templateAssertion.Subjects [0].GetID () == exclamation);
	}
	public virtual bool match(KNSubject s) {
		return false;
	}
	public virtual bool match(KNVerb v) {
		return false;
	}
}