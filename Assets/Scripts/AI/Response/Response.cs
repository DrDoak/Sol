using System.Collections;
using System.Collections.Generic;

public class Response {
	public float lastTime;
	public Character mChar;
	public RPSpeaker speaker;
	public Character listener;
	public Assertion assertion;
	string responseString = "";
	bool fixedStr = true;

	RPTemplate mTemplate;

	public void applyTemplate(RPTemplate rp) {
		fixedStr = false;
		mTemplate = rp;
	}
	public void setAssertion(Assertion a) {
		assertion = a;
		responseString = a.GetID ();
	}
	public void setString(string s) {
		fixedStr = true;
		responseString = s;
	}
	public string toString() {
		if (fixedStr)
			return responseString;
		return parseTemplate ();
	}

	/* Format: Just a normal string to express the resulting string. For templates with gaps, you can use 
	 * $S for the first subject, $V for the first verb, and $D for the first direct object.
	 *  $L would get the listener
	*/
	string parseTemplate() {
		int i = 0;
		string finalStr = "";
		string temp = mTemplate.template;
		while (i < temp.Length) {
			char lastC = temp.ToCharArray ()[i];
			if (lastC == '$') {
				i++;
				char place = temp.ToCharArray ()[i];
				if (place == 'S') {
					//finalStr += assertion.getSubjectID ();
					finalStr += speaker.ConveySubject (assertion, listener).toString();
				} else if (place == 'V') {
					finalStr += speaker.ConveyVerb (assertion, listener).toString();
				} else if (place == 'R') {
					finalStr += speaker.ConveyReceivor (assertion, listener).toString();
				} else if (place == 'L') {
					finalStr += listener.name;
				}

			} else {
				finalStr += lastC;
			}
			i++;
		}
		return finalStr;
	}
}