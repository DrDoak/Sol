using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxTrigger : Interactable {

	public string displayText;
	public bool typeText = true;
	public bool autoTrigger = true;
	float interval = 2.0f;
	float currentInterval = 0.0f;
	protected TextboxManager tm;

	// Use this for initialization
	void Start () {
		initTM ();
	}
	protected void initTM() {
		tm = FindObjectOfType<TextboxManager> ();
	}
	// Update is called once per frame
	void Update () {
		mUpdate ();
	}
	protected void mUpdate() {
		if (currentInterval > 0.0f) {
			currentInterval -= Time.deltaTime;
		}
	}
	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 0, 1, .5f);
		Gizmos.DrawCube (transform.position, transform.localScale);
	}
	internal void OnTriggerEnter2D(Collider2D other) {
		if (autoTrigger && other.gameObject.GetComponent<Player> () && currentInterval <= 0.0f) {
			triggerText ();
		}
	}
	protected virtual void triggerText() {
		currentInterval = interval;
		tm.addTextbox (displayText,gameObject,typeText);
	}
	public override void onInteract(Character interactor) {
		if (!autoTrigger) {
			triggerText ();
		}
	}
}
