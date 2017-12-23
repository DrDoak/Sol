using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : Interactable {

	public string displayText;
	public string eventName;
	public bool onStartup = false;
	public bool oneTime = true;
	public bool onContact = true;
	public float refresh = 60.0f;
	float currentInterval = 0.0f;
	WorldEvent we;
	CharacterManager cm;

	// Use this for initialization
	void Start () {
		init();
	}
	protected void init() {
		cm = FindObjectOfType<CharacterManager> ();
		currentInterval = 0f;
		if (onStartup) {
			triggerEvent ();
		}
	
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
		if (onContact && other.gameObject.GetComponent<Player> () && currentInterval <= 0.0f) {
			triggerEvent ();
		}
	}
	protected virtual void triggerEvent() {
		currentInterval = refresh;
		System.Type mType = System.Type.GetType (eventName + ",Assembly-CSharp");
		gameObject.AddComponent (mType);
		//cm.triggerEvent (we);
	}
	public override void onInteract(Character interactor) {
		if (!onContact) {
			triggerEvent ();
		}
	}
}
