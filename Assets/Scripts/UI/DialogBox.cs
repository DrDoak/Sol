using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBox: MonoBehaviour {
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
	int currentSelection;
	public int maxSelections;
	bool optionsDisplayed = false;
	bool toSetColor = false;
	List<GameObject> options;
	public GameObject optionPrefab;
	Color textColor;
	List<DialogueOption> optList;
	float lastY;

	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer> ();
		line.sortingOrder = 0;
		line.transform.position = new Vector3 (transform.position.x, transform.position.y, -3);
		mText = GetComponentInChildren<Text> ();
		if (!typing) {
			mText.text = fullText;
		}
		options = new List<GameObject> ();
		optionsDisplayed = false;
		maxSelections = 0;
		lastY = 0f;
		initOptions ();
		mManager = FindObjectOfType<TextboxManager> ();
		if (toSetColor) {
			mSetColor (textColor);
		}
	}
	void OnDestroy() {
		mManager.removeTextbox (gameObject);
	}
	public void setColor(Color tC) {
		toSetColor = true;
		textColor = tC;
	}
	void mSetColor(Color tC) {
		GetComponentInChildren<Image> ().color = tC;
		textColor = new Color(1.0f - tC.r,1.0f - tC.g, 1.0f - tC.b,tC.a + 0.5f);
		GetComponentInChildren<Text> ().color = textColor;
		line.startColor = tC;
		line.endColor = tC;
	}

	// Update is called once per frame
	void Update () {
		if (targetedObj != null) {
			transform.position += targetedObj.transform.position-lastPos;
			lastPos = targetedObj.transform.position;
			line.SetPosition (0, transform.position);
			line.SetPosition (1, targetedObj.transform.position);
		}
		if (typing) {
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
				if (Input.GetButtonDown ("Submit")) {
					selectOption (optList[currentSelection]);
				}
				float inputY = Input.GetAxis ("Vertical");

				if (lastY != inputY) { 
					if (inputY < 0.0f) { 
						highlightOption (currentSelection + 1);
					} else if (inputY > 0.0f) { 
						highlightOption (currentSelection - 1);
					}
				}
				lastY = inputY;
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
	void displayOptions() {
		optionsDisplayed = true;
	}
	public void setOptions(List<DialogueOption> opt) {
		optList = opt;
		maxSelections = opt.Count;
	}
	void initOptions() {
		Vector2 sizeDelta = mText.GetComponent<RectTransform> ().sizeDelta;
		foreach (DialogueOption o in optList) {
			GameObject newTextOption = Instantiate (optionPrefab);
			RectTransform rt = newTextOption.GetComponent<RectTransform> ();
			rt.sizeDelta = sizeDelta;
			Vector3 pos = mText.rectTransform.position;
			pos.x -= 180;
			pos.y -=  50 + (maxSelections * sizeDelta.y);
			rt.position = pos;
			newTextOption.GetComponent<TextMeshProUGUI> ().text = o.text;
			newTextOption.GetComponent<TextMeshProUGUI> ().color = textColor;
			newTextOption.GetComponent<TextMeshProUGUI>().transform.SetParent(GetComponent<Canvas>().transform,false);
			maxSelections = maxSelections + 1;
			options.Add (newTextOption);
		}
	}
	public void selectOption(DialogueOption option) {
		//option = Mathf.Max (0, Mathf.Min (maxSelections, option));
		option.responseFunction(option);
		Destroy (gameObject);
	}

	void deselectCurrentOption() {
		options [currentSelection].GetComponent<TextMeshProUGUI> ().fontSize = 12;
		options [currentSelection].GetComponent<TextMeshProUGUI> ().color = textColor;
	}
	public void highlightOption(int option) {
		if (option < 0) {
			option = maxSelections - 1;
		} else if (option == maxSelections) {
			option = 0;
		}
		if (option < maxSelections) {
			deselectCurrentOption ();
			options [option].GetComponent<TextMeshProUGUI> ().color = Color.white;
			options [option].GetComponent<TextMeshProUGUI> ().fontSize = 14;
			currentSelection = option;
		}
	}
}
