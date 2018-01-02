using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextboxManager : MonoBehaviour {

	//public delegate void optionResponse(int r);
	List<GameObject> textboxes;
	public GameObject textboxPrefab;
	public GameObject dialogueBoxPrefab;
	public GameObject listPrefab;
	Camera cam;
	bool type;
	Color TextboxColor;
	float timeAfter = 2f;
	float textSpeed = 0.05f;

	// Use this for initialization
	void Start () {
		textboxes = new List<GameObject> ();
		cam = FindObjectOfType<Camera> ();
		TextboxColor = new Color (1.0f, 0.0f, 0.0f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {}
	public textbox addTextbox(string text,GameObject targetObj,bool typeText) {
		return addTextbox (text, targetObj, typeText, textSpeed);
	}
	public textbox addTextbox(string text,GameObject targetObj,bool typeText,float textSpeed) {
		Vector2 newPos = findPosition (targetObj.transform.position);
		GameObject newTextbox = Instantiate (textboxPrefab,newPos,Quaternion.identity);
		textbox tb = newTextbox.GetComponent<textbox> ();
		if (!type) {
			//Debug.Log ("displaying Textbox: " + text);
			newTextbox.GetComponent<disappearing> ().duration = textSpeed * text.Length + timeAfter;
			newTextbox.GetComponent<disappearing> ().toDisappear = true;
		}

		tb.setTypeMode (typeText);			
		tb.setText(text);
		tb.transform.position = newPos;
		tb.setTargetObj (targetObj);
		tb.pauseAfterType = timeAfter;
		tb.timeBetweenChar = textSpeed;
		tb.mManager = this;
		RectTransform[] transforms = newTextbox.GetComponentsInChildren<RectTransform> ();
		if (text.Length > 50) {
			Vector2 v = new Vector2 ();
			foreach (RectTransform r in transforms) {
				v.y = r.sizeDelta.y * 2f;
				v.x = r.sizeDelta.x;
				if (text.Length > 100) {
					v.x = r.sizeDelta.x * 1.5f;
				}
				r.sizeDelta = v;
			}
		}
		LineRenderer line = newTextbox.GetComponent<LineRenderer> ();
		line.SetPosition (0, new Vector3 (newPos.x, newPos.y, 0f));
		textboxes.Add (newTextbox);
		tb.setColor (TextboxColor);
		return tb;
	}
	public ListSelection addListOptions(string text, GameObject targetObj, List<DialogueOption> options) {
		GameObject newList = Instantiate (listPrefab);
		ListSelection nl = newList.GetComponent<ListSelection> ();

		nl.SetTitle (text);
		nl.addOptions (options);
		nl.transform.SetPositionAndRotation (new Vector3 (0f, -50f), Quaternion.identity);
		nl.transform.SetParent (FindObjectOfType<GameManager> ().gameObject.transform.Find ("UI"), false);

		return nl;
	}

	public DialogBox addDialogueOptions(string text,GameObject targetObj,List<DialogueOption> options) {
		Vector2 newPos = findPosition (targetObj.transform.position);
		GameObject newTextbox = Instantiate (dialogueBoxPrefab,newPos,Quaternion.identity);
		DialogBox tb = newTextbox.GetComponent<DialogBox> ();

		tb.setTypeMode (true);			
		tb.setText(text);
		tb.setTargetObj (targetObj);
		tb.timeBetweenChar = 0.02f;
		tb.mManager = this;
		RectTransform[] transforms = newTextbox.GetComponentsInChildren<RectTransform> ();

		int maxString = text.Length;
		foreach (DialogueOption d in options) {
			maxString = Mathf.Max (maxString, d.text.Length);
		}
		Vector2 v = new Vector2 ();
		if (maxString > 50) {
			foreach (RectTransform r in transforms) {
				v.y = r.sizeDelta.y * 2f;
				v.x = r.sizeDelta.x;
				if (maxString > 100) {
					v.x = r.sizeDelta.x * 1.5f;
				}
				r.sizeDelta = v;
			}
		}
		tb.setOptions (options);
		v.y *= tb.maxSelections;
		tb.transform.GetChild (0).gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2(200f,40 + 20f * tb.maxSelections);
		tb.transform.GetChild (0).gameObject.GetComponent<RectTransform> ().localPosition = new Vector2 (0f, 10f-10f * tb.maxSelections);
		LineRenderer line = newTextbox.GetComponent<LineRenderer> ();
		line.SetPosition (0, new Vector3 (newPos.x, newPos.y, 0f));
		textboxes.Add (newTextbox);
		if (TextboxColor != null) {
			tb.setColor (TextboxColor);
		}
		return tb;
	}

	public Vector2 findPosition(Vector2 startLocation) {
		//Vector2 newPos;
		float targetY = startLocation.y + 5f;
		//newPos.y = targetY;
		foreach (GameObject o in textboxes) {
		}
		return new Vector2 (startLocation.x, targetY);
	}
	public void setPauseAfterType(float time) {
		timeAfter = time;
	}
	public void setTextSpeed(float time ){
		textSpeed = time;
	}
	public void removeTextbox(GameObject go) {
		textboxes.Remove (go);
	}
}
