using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextboxManager : MonoBehaviour {

	List<GameObject> textboxes;
	public GameObject textboxPrefab;
	Camera cam;
	bool type;
	Color TextboxColor;
	float timeAfter = 2f;
	float textSpeed = 0.05f;

	// Use this for initialization
	void Start () {
		textboxes = new List<GameObject> ();
		cam = FindObjectOfType<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {}
	public void addTextbox(string text,GameObject targetObj,bool type) {
		addTextbox (text, targetObj, type, textSpeed);
	}
	public void addTextbox(string text,GameObject targetObj,bool type,float textSpeed) {
		if (!textboxPrefab) {
		} else {
			Vector2 newPos = findPosition (targetObj.transform.position);
			GameObject newTextbox = Instantiate (textboxPrefab,newPos,Quaternion.identity);
			textbox tb = newTextbox.GetComponent<textbox> ();
			if (!type) {
				newTextbox.GetComponent<disappearing> ().duration = textSpeed * text.Length + timeAfter;
			}

			tb.setTypeMode (type);			
			tb.setText(text);
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
		}
	}

	public Vector2 findPosition(Vector2 startLocation) {
		//Vector2 newPos;
		float targetY = startLocation.y + (Screen.height)/100f;
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
