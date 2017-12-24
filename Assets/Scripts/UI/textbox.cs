using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textbox : MonoBehaviour {

	public TextboxManager mManager;
	public DialogueUnit masterSequence;
	GameObject targetedObj;
	LineRenderer line;
	bool typing;
	Vector3 lastPos;
	string fullText;
	string currentText;
	public float timeBetweenChar = 0.01f;
	float sinceLastChar = 0f;
	float pauseTime = 0f;
	float timeSinceStop = 0f;
	int lastCharacter;
	public float pauseAfterType = 2f;
	Text mText;
	Color tC;
	public bool conclude = false;

	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer> ();
		line.sortingOrder = 0;
		line.transform.position = new Vector3 (transform.position.x, transform.position.y, -3);
		mText = GetComponentInChildren<Text> ();
		if (!typing) {
			mText.text = fullText;
		}
		initColor ();
	}
	void OnDestroy() {
		conclude = true;
		if (masterSequence != null) {
			masterSequence.parseNextElement ();
		}
		mManager.removeTextbox (gameObject);
		/*if (targetedObj.GetComponent<Character> ()) {
			targetedObj.GetComponent<Character> ().onTBComplete ();
		}*/
	}
	public void initColor() {
		GetComponentInChildren<Image> ().color = tC;
		GetComponentInChildren<Text> ().color = new Color(1.0f - tC.r,1.0f - tC.g, 1.0f - tC.b,tC.a + 0.5f);
		line.startColor = tC;
		line.endColor = tC;
	}
	public void setColor(Color C) {
		tC = C;
	}
	
	// Update is called once per frame
	/* Cutscene scripting guide:
	 *  Normal text is shown as dialogue for the starting character.
	 * 	Using the '|' character or the enter character will create a new textbox.
	 *  At the start of a new textbox if the colon character is found within the first 18 characters, the game will attempt to search
	 *  For the character and make the dialogue come from that character instead.
	 *  
	 *  The character ` surrounds a special block.
	 * A number will result in a pause for a certain amount of time.
	 * $ will change the text speed
	 * > will make a character walk to another character. Needs to be followed by the character name.
	 * < will make them walk away 
	 * ] will make them face a character
	 * [ will make them face away.
	 * Any text means the character would try to do an animation
	 * */
	void Update () {
		if (targetedObj != null) {
			transform.position += targetedObj.transform.position-lastPos;
			//transform.position = targetedObj.transform.position;
			//lastPos = targetedObj.transform.position;
			line.SetPosition (0, transform.position);
			line.SetPosition (1, targetedObj.transform.position);
		}
		if (typing ) {
			if (lastCharacter < fullText.Length) { 
				sinceLastChar += Time.deltaTime;
				if (sinceLastChar > timeBetweenChar) {
					if (pauseTime > 0f) {
						pauseTime -= Time.deltaTime;
					} else {
						lastCharacter++;
						char nextChar = fullText.ToCharArray () [lastCharacter - 1];
						if (nextChar == '`') {
						//	Debug.Log ("Start special section");
							string actStr = "";
							lastCharacter++;
							nextChar = fullText.ToCharArray () [lastCharacter - 1];
							//Debug.Log (nextChar);
							string action = "pause";
							float res;
							string test = "";
							test += nextChar;
							if (float.TryParse (test,out res)) {
							} else {
								//Debug.Log (nextChar);
								if (nextChar == ']') {
									action = "faceTowards";
								} else if (nextChar == '[') {
									//Debug.Log ("Correct Char");
									action = "faceAway";
								} else if (nextChar == '>') {
									action = "walkTowards";
								} else if (nextChar == '<') {
									action = "walkAway";
								} else if (nextChar == '$'){
									action = "textSpeed";
								} else {
									lastCharacter--;
									action = "animation";
								}
								lastCharacter++;
								nextChar = fullText.ToCharArray () [lastCharacter - 1];
							}
							bool numFound = false;
							string num = "";
							string targetChar = null;
							while (nextChar != '`') {
								if (nextChar == ':') {
									Debug.Log ("targeting: " + actStr);
									targetChar = actStr;
									actStr = "";
									lastCharacter++;
									nextChar = fullText.ToCharArray () [lastCharacter - 1];
								}
								else if ((action == "walkTowards" || action == "walkAway") && nextChar == '-') {
									numFound = true;
								} else {
									if (numFound == true) {
										num += nextChar;
									} else {
										actStr += nextChar;
									}
									lastCharacter++;
									nextChar = fullText.ToCharArray () [lastCharacter - 1];
								}

							}
							if (action == "walkTowards") {
								if (num.Length < 1) {
									masterSequence.walkToChar (targetChar, actStr, 1f);
								} else {
									masterSequence.walkToChar (targetChar, actStr, float.Parse(num));
								}
							} else if (action == "walkAway") {
							} else if (action == "faceTowards") {
								//Debug.Log ("Facing towards");
								masterSequence.turnTowards (targetChar, actStr, true);
							} else if (action == "faceAway") {
								//Debug.Log ("facing away");
								masterSequence.turnTowards (targetChar, actStr, false);
							} else if (action == "animation") {
								if (masterSequence != null) {
									masterSequence.setAnimation (targetChar, actStr);
								}
							} else if (action == "textSpeed") {
								timeBetweenChar = float.Parse (actStr);
							} else { 
								pauseTime = float.Parse (actStr);
							}
						} else {
							currentText += nextChar;
							mText.text = currentText;
							sinceLastChar = 0f;
						}
					}
				}
			} else {
				timeSinceStop += Time.deltaTime;
				if (timeSinceStop > pauseAfterType) {
					Destroy (gameObject);
				}
			}

		}
	}

	public void setTargetObj(GameObject gameObj) {
		targetedObj = gameObj;
		lastPos = gameObj.transform.position;
	}
	public void setTypeMode(bool type) {
		typing = type;
		if (type) {
			currentText = "";
			lastCharacter = 0;
		} else {
			currentText = fullText;
		}
	}
	public void setText(string text) {
		fullText = text;
	}
}
