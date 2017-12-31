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

	public RPTemplate Template;

	public void ApplyTemplate(RPTemplate rp) {
		fixedStr = false;
		Template = rp;
	}
	public void SetAssertion(Assertion a) {
		assertion = a;
		responseString = a.GetID ();
	}
	public void SetString(string s) {
		fixedStr = true;
		responseString = s;
	}
	public override string ToString() {
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
		string temp = Template.OutputTemplate;
		while (i < temp.Length) {
			char lastC = temp.ToCharArray ()[i];
			if (lastC == '$') {
				i++;
				char place = temp.ToCharArray ()[i];
				if (place == 'S') {
					//finalStr += assertion.getSubjectID ();
					finalStr += speaker.ConveySubject (assertion, listener).ToString();
				} else if (place == 'V') {
					finalStr += speaker.ConveyVerb (assertion, listener).ToString();
				} else if (place == 'R') {
					finalStr += speaker.ConveyReceivor (assertion, listener).ToString();
				} else if (place == 'L') {
					finalStr += (listener) ? listener.name : "";
				}

			} else {
				finalStr += lastC;
			}
			i++;
		}
		return finalStr;
	}
}