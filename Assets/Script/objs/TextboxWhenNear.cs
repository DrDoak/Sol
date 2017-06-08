using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxWhenNear : MonoBehaviour {

	public string displayText;
	public bool typeText = true;
	float interval = 2.0f;
	float currentInterval = 0.0f;
	TextboxManager tm;

	// Use this for initialization
	void Start () {
		tm = FindObjectOfType<TextboxManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (currentInterval > 0.0f) {
			currentInterval -= Time.deltaTime;
		}
	}

	internal void OnTriggerEnter2D(Collider2D other) {
//		Debug.Log ("detection with obj: " + other.gameObject.GetComponent<Player> ().ToString());
		if (other.gameObject.GetComponent<Player> () && currentInterval <= 0.0f) {
			currentInterval = interval;
			tm.addTextbox (displayText,gameObject,typeText);
		}
	}
}
