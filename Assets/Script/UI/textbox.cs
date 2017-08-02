using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textbox : MonoBehaviour {

	public TextboxManager mManager;
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
		mManager.removeTextbox (gameObject);
		if (targetedObj.GetComponent<Character> ()) {
			targetedObj.GetComponent<Character> ().onTBComplete ();
		}
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
	void Update () {
		if (targetedObj != null) {
			transform.position += targetedObj.transform.position-lastPos;
			lastPos = targetedObj.transform.position;
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
							string num = "";
							lastCharacter++;
							nextChar = fullText.ToCharArray () [lastCharacter - 1];
							bool textSpeed = false;
							if (nextChar == 's') {
								lastCharacter++;
								nextChar = fullText.ToCharArray () [lastCharacter - 1];
								textSpeed = true;
							}
							while (nextChar != '`') {
								num += nextChar;
								lastCharacter++;
								nextChar = fullText.ToCharArray () [lastCharacter - 1];
							}
							if (textSpeed) {
								timeBetweenChar = float.Parse (num);
							} else { 
								pauseTime = float.Parse (num);
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
